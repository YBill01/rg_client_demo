using Legacy.Database;
using System;
using UnityEngine;

namespace Legacy.Client
{
	/// <summary>
	/// Если у игрока есть герой для улучшения - предлагаем перейти в окно списка героев
	/// </summary>
	class OpenHeroesList : SoftTutorialBehaviour
	{
		[SerializeField]
		RectTransform HeroesButton;

		public override int Priority => (int)SoftTutorialBehaviour.MainWindowPriority.UpgradeHero;

		public override bool CanStartTutorial()
		{
			if (profile.HardTutorialState < 3)
				return false;

			if (profile.HasSoftTutorialState(SoftTutorial.SoftTutorialState.UpgradeHero))
				return false;

			if (!CanUpgradeHero(profile))
				return false;

			return true;
		}

		public override void StartTutorial()
		{
			var button = HeroesButton.GetComponent<LegacyButton>();

			SoftTutorialManager.Instance.MenuTutorialPointer.PointerToRect(HeroesButton, button);
		}

		static public bool CanUpgradeHero(ProfileInstance profile)
		{
			if (profile.Level.level < 2)
				return false;

			var heroes = profile.heroes;
			var canUpgrade = false;
			foreach (var hero in heroes)
			{
				canUpgrade = hero.Value.level < profile.Level.level;
				if (profile.Stock.CanTake(CurrencyType.Soft, hero.Value.UpdatePrice))
					return true;
			}

			return false;
		}
	}
}


