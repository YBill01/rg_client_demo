using Legacy.Database;
using System;
using UnityEngine;

namespace Legacy.Client
{
	/// <summary>
	/// Если у игрока есть герой для обновления - предлагаем перейти в окно героя
	/// </summary>
	class ScrollAllCards : SoftTutorialBehaviour
	{
		public override int Priority => 0;

		public override ushort TutorialState => (ushort)SoftTutorial.SoftTutorialState.ShowCards;

		public override bool CanStartTutorial()
		{
			if (profile.HasSoftTutorialState(SoftTutorial.SoftTutorialState.ShowCards))
				return false;

			return true;
		}

		public override void StartTutorial()
		{
			gameObject.GetComponent<DecksWindowBehaviour>().ScrollAllDeckIfTutorial(OnFinish);
		}

		private void OnFinish()
		{
			SoftTutorialManager.Instance.OnTutotorComplite(this);
		}
	}
}



