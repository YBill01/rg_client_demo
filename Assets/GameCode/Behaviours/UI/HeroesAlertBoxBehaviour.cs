using UnityEngine;
using Legacy.Client;
using Legacy.Database;
using System.Linq;

public class HeroesAlertBoxBehaviour : MonoBehaviour
{
    [SerializeField]
    AlertBoxBehaviour alertBox;

    private void Start()
    {
        var profile = ClientWorld.Instance.Profile;
        profile.PlayerProfileUpdated.AddListener(UpdateAlertBox);
        /*if (profile.IsBattleTutorial)
            HomeTutorialHelper.Instance.Update.AddListener(UpdateAlertBox);*/
        UpdateAlertBox();
    }

    private void OnDestroy()
    {
        var profile = ClientWorld.Instance.Profile;
        profile.PlayerProfileUpdated.RemoveListener(UpdateAlertBox);
        //HomeTutorialHelper.Instance.Update.RemoveListener(UpdateAlertBox);
    }

    private void UpdateAlertBox()
    {
        alertBox.HideAll();

        var profile = ClientWorld.Instance.Profile;

        int countOfNew = 0;
        bool heroForUpdate = false;
        bool heroForUpdateNotSoft = false;
        //bool skillForUpdate = false;

        var allHeroes = Heroes.Instance.Get();
        var heroes = profile.heroes;
        /*if (profile.HardTutorialState <= 2)
            return;*/
        if (profile.HardTutorialState < 3/* || profile.HardTutorialState == 3 && HomeTutorialHelper.Instance.HardHomeTutorStep != 14*/)
            return;

        foreach (var hero in allHeroes)
        {
            if (hero.type != BinaryHeroType.Player)
                continue;

            if (!hero.GetLockedByArena(out BinaryBattlefields binaryArena))
                if (!hero.GetLockedTutorByArena(out BinaryBattlefields binaryArena2))
                    continue;

            byte number = Settings.Instance.Get<ArenaSettings>().GetNumber(binaryArena.index);

            if (profile.GetPlayerHero(hero.index, out var heroData))
            {
                if (heroData.level < profile.Level.level &&
                heroData.UpdatePrice <= profile.Stock.GetCount(CurrencyType.Soft))
                {
                    heroForUpdate = true;
                }
                else if (heroData.level < profile.Level.level)
                {
                    heroForUpdateNotSoft = true;
                    /*foreach (var skillLevel in heroData.skills.Values)
                    {
                        if (skillLevel < heroData.level)
                        {
                            var _skill_cost = _hero_settings.GetSkillShards((byte)skillLevel);

                            if (_skill_cost <= profile.Stock.GetCount(CurrencyType.Shards))
                            {
                                skillForUpdate = true;
                            }
                        }
                    }*/
                }
            }
            else
            {
                if (profile.CurrentArena.number >= number && !profile.ViewedHeroes.Contains(hero.index))
                {
                    countOfNew++;
                }
            }
        }

        //if (countOfNew != 0 && !profile.IsBattleTutorial)
        //{
        //    alertBox.ShowRedAlert(countOfNew.ToString());
        //    return;
        //}
        if (heroForUpdate /*|| skillForUpdate*/)
            alertBox.ShowArrowAlert("", heroForUpdate);
        else if(heroForUpdateNotSoft)
            alertBox.ShowArrowAlert("", false);
    }
}
