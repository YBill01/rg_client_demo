using Legacy.Database;
using System;
using UnityEngine;

namespace Legacy.Client
{
	/// <summary>
	/// Показываем игроку на кнопку улучшить героя
	/// </summary>
	class UpgradeHero : SoftTutorialBehaviour
	{
		[SerializeField]
		RectTransform UpgradeButton;

		// Нам не нужно отмечать выполнение этого тутора.
		// Это сделает кнопка улучшить героя, так как игрок может улучшить героя, до того как этот туториал успеет отработать
		// По факту оно оказалось в методе HeroWindowBehaviour.UpgradeHero
		//public override ushort TutorialState => (ushort)SoftTutorial.SoftTutorialState.UpgradeHero;

		public override int Priority => 0;

		public override bool CanStartTutorial()
		{
			if (profile.HardTutorialState < 3)
				return false;

			if (profile.HasSoftTutorialState(SoftTutorial.SoftTutorialState.UpgradeHero))
				return false;

			var heroIndex = (WindowManager.Instance.CurrentWindow as HeroWindowBehaviour).GetCurrentHero();
			if (!profile.GetPlayerHero(heroIndex, out PlayerProfileHero hero))
			{
				Debug.LogError("Да мы получили это сообщение когда перешли на не открытого героя. Это сообщение можно удалять - все работает как надо");
				return false;
			}

			if(hero.level >= profile.Level.level || !profile.Stock.CanTake(CurrencyType.Soft, hero.UpdatePrice))
				return false;

			return true;
		}

		public override void StartTutorial()
		{
			var button = UpgradeButton.GetComponent<LegacyButton>();

			SoftTutorialManager.Instance.MenuTutorialPointer.PointerToRect(UpgradeButton, button, OnClick);
		}

		private void OnClick()
		{
            SoftTutorialManager.Instance.MenuTutorialPointer.ReleasePointer();
			SoftTutorialManager.Instance.OnTutotorComplite(this);
		}
	}
}



