using Legacy.Database;
using System;
using UnityEngine;

namespace Legacy.Client
{
	/// <summary>
	/// Если у игрока есть герой для обновления - предлагаем перейти в окно героя
	/// </summary>
	class OpenHeroWindow : SoftTutorialBehaviour
	{
		[SerializeField]
		RectTransform HeroesContainer;

		public override int Priority => (int)MainWindowPriority.UpgradeHero;

		public override bool CanStartTutorial()
		{
			if (profile.HardTutorialState < 3)
				return false;

			if (profile.HasSoftTutorialState(SoftTutorial.SoftTutorialState.UpgradeHero))
				return false;

			if (!OpenHeroesList.CanUpgradeHero(profile))
				return false;

			return true;
		}

		public override void StartTutorial()
		{
			var button = GetHeroToUpgrade();
			var hero = button.GetComponent<RectTransform>();

			SoftTutorialManager.Instance.MenuTutorialPointer.PointerToRect(hero, button);
		}

		private LegacyButton GetHeroToUpgrade()
		{
			foreach (RectTransform child in HeroesContainer)
			{
				var heroBehaviour = child.GetComponent<HeroPanelBehaviour>();
				var hero = heroBehaviour.PlayerHero;

				if (hero.level < profile.Level.level && profile.Stock.CanTake(CurrencyType.Soft, hero.UpdatePrice))
					return heroBehaviour.HeroButton;
			}

			throw new Exception("Can't find Hero to upgrade");
		}

		public override void StopTutorial()
		{
			SoftTutorialManager.Instance.MenuTutorialPointer.ReleasePointer();
		}
	}
}


