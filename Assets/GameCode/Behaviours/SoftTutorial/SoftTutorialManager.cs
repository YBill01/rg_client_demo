using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Legacy.Client
{
	class SoftTutorialManager : MonoBehaviour
	{
		public static SoftTutorialManager Instance { get; private set; }
		public bool IsAnyTutorActive => currentTutorial != null;

		[SerializeField]
		public MenuTutorialPointerBehaviour MenuTutorialPointer;

		private ProfileInstance profile;
		private SoftTutorialBehaviour currentTutorial;

		public bool ClickedOnBattleWith6Cards;

		private void Awake()
		{
			Instance = this;
			profile = ClientWorld.Instance.Profile;
			profile.PlayerProfileUpdated.AddListener(CheckTutorialsForCurrentWindowFast);
		}

		private void OnDestroy()
		{
			profile.PlayerProfileUpdated.RemoveListener(CheckTutorialsForCurrentWindowFast);
		}

		public void OnGoToBattle()
		{
			currentTutorial = null;
		}

		/// <summary>
		/// Метод вызывается при каждой смене окна
		/// </summary>
		public void CheckTutorialsForCurrentWindow()
		{
			StartCoroutine(CheckSoftTutorialCoroutine());
		}

		/// <summary>
		/// Метод вызывается при каждом обновлении профиля
		/// </summary>
		public void CheckTutorialsForCurrentWindowFast()
		{
			StartCoroutine(CheckSoftTutorialCoroutine(true));
		}

		private IEnumerator CheckSoftTutorialCoroutine(bool immediately = false)
		{
			if (immediately)
				yield return null;
			else 
				yield return new WaitForSeconds(1);

			CheckSoftTutorial();
		}

		private void CheckSoftTutorial()
		{
			var window = WindowManager.Instance.CurrentWindow;
			var tutorials = window.GetComponents<SoftTutorialBehaviour>();

			var orderedTutorials = tutorials.OrderBy(x => x.Priority);

			foreach (var tutor in orderedTutorials)
			{
				Debug.Log($"Is CanStartTutorial {tutor.GetType()}");
				if (tutor.CanStartTutorial())
				{
					if (currentTutorial == tutor && !currentTutorial.CanBeRestarted)
						return;

					// Выключаем действующий туториал
					if (IsAnyTutorActive)
					{
						Debug.Log($"StopTutorial {currentTutorial.GetType()}");
						currentTutorial.StopTutorial();
					}

					Debug.Log($"StartTutorial {tutor.GetType()}");
					currentTutorial = tutor;
					tutor.StartTutorial();
					break;
				}
			}
		}

		/// <summary>
		/// Иногда туториал моэет захотеть сам себя остановить, об этом нужно сообщить менеджеру
		/// </summary>
		/// <param name="tutor"></param>
		public void TutorialSelfStoped(SoftTutorialBehaviour tutor)
		{
			if (currentTutorial == tutor)
				currentTutorial = null;
		}

		public void OnTutotorComplite(SoftTutorialBehaviour window)
		{
			currentTutorial = null;
			CompliteTutorial((SoftTutorial.SoftTutorialState)window.TutorialState);
			CheckTutorialsForCurrentWindowFast();
		}

		public void CompliteTutorial(SoftTutorial.SoftTutorialState tutorial)
		{
			if (profile.HasSoftTutorialState(tutorial) || (int)tutorial == 0)
				return;

			Debug.Log($"CompliteTutorial {tutorial}");

			ushort newMenuState = (ushort)(profile.MenuTutorialState | (ushort)tutorial);
			profile.MenuTutorialState = newMenuState;
			NetworkMessageHelper.UpdateTutorialState(profile.HardTutorialState, 0, newMenuState, 0, 0, profile.index);
		}
	}
}
