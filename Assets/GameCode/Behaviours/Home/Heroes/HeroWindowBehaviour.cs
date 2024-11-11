using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using static Legacy.Client.HeroParamBehaviour;
using static SkillParametrBehavior;

namespace Legacy.Client
{
    public class HeroWindowBehaviour : WindowBehaviour
    {

        private BinaryHero BinaryHero;
        private BinaryEntity BinaryMinion;
        private MinionOffence HeroOffence;
        private MinionDefence HeroDefence;
        private MinionSkills HeroSkills;
        private PlayerProfileHero PlayerHero = null;
        private BaseBattleSettings settings;


        [SerializeField] private HeroWindowDownButtonsBehaviour DownButtons;
        [SerializeField] private Transform TabToContinue;
        [SerializeField] private HeroWindowSkillItemBehaviour skill1;
        [SerializeField] private HeroWindowSkillItemBehaviour skill2;

        [SerializeField] private Canvas Lock;

        [SerializeField] private HeroNameColorBehaviour Name;

        [SerializeField] private HeroLvlBehaviour HeroLvl;

        [SerializeField] private HeroParamBehaviour[] HeroParams;

        private ushort chosenHero;

        public ProfileInstance Profile { get; private set; }
        private List<SkillParamData> skillParams = new List<SkillParamData>();
        private Dictionary<ShortInfoSkillView, List<SkillParamData>> skills = new Dictionary<ShortInfoSkillView, List<SkillParamData>>();

        public override void Init(Action callback)
        {
            settings = Settings.Instance.Get<BaseBattleSettings>();
            Profile = ClientWorld.Instance.Profile;
            callback();
            Lock.sortingLayerName = "UI";
            Profile.PlayerProfileUpdated.AddListener(UpdateAll);
        }
        public void Skill()
        {
            //WindowManager.Instance.OpenWindow(childs_windows[0]);
            //PopupAlertBehaviour.ShowHomePopupAlert(Input.mousePosition, );
        }

        void InitData(ushort hero_index)
        {
            BinaryHero = default;
            PlayerHero = null;
            HeroOffence = default;
            HeroDefence = default;
            HeroSkills = default;

            if (Heroes.Instance.Get(hero_index, out BinaryHero binaryHero))
            {
                BinaryHero = binaryHero;
                DownButtons.InitButtons(binaryHero);
                HeroState hero_state = CheckHero(hero_index, out PlayerProfileHero _hero, out byte LockedByArenaNumber);
                if (hero_state == HeroState.Exists)
                {
                    PlayerHero = _hero;
                    DownButtons.Exists(_hero, Profile);
                    MarkHeroAsViewed(hero_index);
                }
                else if (hero_state == HeroState.NeedBuy)
                {
                    MarkHeroAsViewed(hero_index);
                    DownButtons.NeedBuy();
                }
                else
                {
                    DownButtons.NeedOpen(LockedByArenaNumber);
                }
               

                if (Entities.Instance.Get(BinaryHero.minion, out BinaryEntity binaryMinion))
                {
                    BinaryMinion = binaryMinion;
                    if (Components.Instance.Get<MinionOffence>()
                        .TryGetValue(BinaryMinion.index, out MinionOffence offence))
                    {
                        HeroOffence = offence;
                    }

                    if (Components.Instance.Get<MinionDefence>()
                        .TryGetValue(BinaryMinion.index, out MinionDefence defence))
                    {
                        HeroDefence = defence;
                    }

                    if (Components.Instance.Get<MinionSkills>()
                        .TryGetValue(BinaryMinion.index, out MinionSkills skills))
                    {
                        HeroSkills = skills;
                    }
                }
            }      
        }

        private void MarkHeroAsViewed(ushort hero_index)
        {
            if (Profile.ViewedHeroes.Contains(hero_index))
                return;

            Profile.ViewHero(hero_index);
        }

        public ushort GetCurrentHero()
        {
            return BinaryHero.index;
        }

        protected override void SelfClose()
        {
            ResetWindow();
            gameObject.SetActive(false);
            MenuHeroesBehaviour.Instance.ShowHero(Profile.SelectedHero);
        }

        private void ResetWindow()
        {
            ResetParams();
        }

        void LoadHero()
        {
            if (PlayerHero == null)
                if (Profile.GetPlayerHero(chosenHero, out PlayerProfileHero _hero))
                    PlayerHero = _hero;
        }

        private void ResetParams()
        {
            for (byte i = 0; i < HeroParams.Length; i++)
            {
                HeroParams[i].ResetParam();
            }
        }

        public ushort GetСhosenHero()
        {
            return chosenHero;
        }
        protected override void SelfOpen()
        {
            chosenHero = Profile.SelectedHero;
            if (parent.GetType() == typeof(HeroesWindowBehaviour))
            {
                chosenHero = (parent as HeroesWindowBehaviour).GetClickedHero();
            }
            /*if (Profile.HardTutorialState == 3 && HomeTutorialHelper.Instance.HardHomeTutorStep == 15)
            {
                WindowManager.Instance.ShowBack(false);
            }*/

            MenuHeroesBehaviour.Instance.ShowHero(chosenHero);
            InitData(chosenHero);
            BuildWindow();
            gameObject.SetActive(true);
        }


        /// <summary>
        /// CalledBy Arrow button
        /// </summary>
        public void Next()
        {
            if (GetNextHero(out ushort next))
            {
                if (Heroes.Instance.Get(next, out BinaryHero binaryHero))
                    if (binaryHero.type != BinaryHeroType.Player) return;
                chosenHero = next;
                MenuHeroesBehaviour.Instance.ShowHero(chosenHero);
                ResetWindow();
                InitData(next);
                BuildWindow();
            }
        }

        /// <summary>
        /// CalledBy ChooseButton
        /// </summary>
        public void ChooseHero()
        {
            Profile.SelectHero(BinaryHero);
            DownButtons.SetOrdersForChoosenHero(Profile);
        }

        /// <summary>
        /// CalledBy Arrow button
        /// </summary>
        public void Previous()
        {
            if (GetPreviousHero(out ushort previous))
            {
                if (Heroes.Instance.Get(previous, out BinaryHero binaryHero))
                    if (binaryHero.type != BinaryHeroType.Player) return;
                chosenHero = previous;
                MenuHeroesBehaviour.Instance.ShowHero(chosenHero);
                ResetWindow();
                InitData(previous);
                BuildWindow();
            }
        }

        public void UpdateAll()
        {
            // ResetWindow();
            InitData(chosenHero);
            SetLevel();
            SetSkills();
            SetButtons();
        }
        public void UpdateAfterBuy()
        {
            Profile?.PlayerProfileUpdated?.RemoveListener(UpdateAfterBuy);
            InitData(chosenHero);
            SetLevel();
            SetParams();
            SetSkills();
            SetButtons();
        }

        public void UpdateAfterUp()
        {
            /*if (Profile.HardTutorialState == 3 && HomeTutorialHelper.Instance.HardHomeTutorStep == 15)
            {
                TabToContinue.gameObject.SetActive(true);
                DownButtons.gameObject.SetActive(false);
                WindowManager.Instance.playerActivity.OnTapUpdate.AddListener(CloseOn);
                WindowManager.Instance.ShowBack(false);
                //  WindowManager.Instance.Home();
                return;
            }*/
            // ResetWindow();
            InitData(chosenHero);
            SetLevel();
            SetParams();
            SetSkills();
            SetButtons();
            DownButtons.UpgradeButtonVisible();
        }

        private void CloseOn()
        {
            //WindowManager.Instance.ShowBack(true);
            //WindowManager.Instance.playerActivity.OnTapUpdate.RemoveListener(CloseOn);
            WindowManager.Instance.Home();
        }

        public void TapToClose()
        {
            if (GetComponent<LevelUpHeroBehavior>().TapToContinueEnabled)
            {
                CloseOn();
			}
			else
			{
                GetComponent<LevelUpHeroBehavior>().SkipStep();
            }
        }

        private void BuildWindow()
        {
            SetName();
            SetLevel();
            SetParams();
            SetSkills();
            SetButtons();
            DownButtons.UpgradeButtonVisible();
        }

        private void SetLevel()
        {
            HeroLvl.gameObject.SetActive(PlayerHero != null);
            HeroLvl.SetData(PlayerHero?.level ?? 1);
        }

        private void SetSkills()
        {
            skill1.Init(HeroSkills.skill1, PlayerHero);
            skill2.Init(HeroSkills.skill2, PlayerHero);
        }

        internal bool GetNextHero(out ushort hero, int clickedIndex = 0)
        {
            hero = 0;

            if (clickedIndex == 0)
            {
                clickedIndex = Heroes.Instance.SortedByArenaList.IndexOf(chosenHero);
            }
            var nextIndex = clickedIndex + 1;
            if (nextIndex > Heroes.Instance.SortedByArenaList.Count - 1)
            {
                return false;
            }

            HeroState state = CheckHero(Heroes.Instance.SortedByArenaList[nextIndex], out PlayerProfileHero player_hero, out byte arenaNumber);
            if(state == HeroState.NeedOpen || state == HeroState.None)
            {
                if(GetNextHero(out ushort heroIndex, nextIndex))
                {
                    hero = Heroes.Instance.SortedByArenaList[heroIndex];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            hero = Heroes.Instance.SortedByArenaList[nextIndex];

            return true;
        }

        internal bool GetPreviousHero(out ushort hero, int clickedIndex = 0)
        {
            hero = 0;
            if (clickedIndex == 0)
            {
                clickedIndex = Heroes.Instance.SortedByArenaList.IndexOf(chosenHero);
            }
            var previousIndex = clickedIndex - 1;
            if (previousIndex < 0)
            {
                return false;
            }

            HeroState state = CheckHero(Heroes.Instance.SortedByArenaList[previousIndex], out PlayerProfileHero player_hero, out byte arenaNumber);
            if (state == HeroState.NeedOpen || state == HeroState.None)
            {
                if (GetPreviousHero(out ushort heroIndex, previousIndex))
                {
                    hero = Heroes.Instance.SortedByArenaList[heroIndex];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            hero = Heroes.Instance.SortedByArenaList[previousIndex];

            return true;

        }

        public enum HeroState : byte
        {
            None,
            Exists,
            NeedBuy,
            NeedOpen
        }

        private HeroState CheckHero(ushort hero_index, out PlayerProfileHero playerHero, out byte HeroArenaNumber)
        {
            playerHero = default;
            HeroArenaNumber = 0;
            if(Heroes.Instance.Get(hero_index, out BinaryHero binaryHero))
            {
                if (Profile.GetPlayerHero(hero_index, out PlayerProfileHero _hero))
                {
                    playerHero = _hero;
                    return HeroState.Exists;
                }
                else
                {
                    if (binaryHero.GetLockedByArena(out BinaryBattlefields binaryArena))
                    {
                        HeroArenaNumber = Settings.Instance.Get<ArenaSettings>().GetNumber(binaryArena.index);
                        if (Profile.CurrentArena.number >= HeroArenaNumber && !Profile.IsBattleTutorial)
                        {
                            return HeroState.NeedBuy;
                        }
                        else
                        {
                            return HeroState.NeedOpen;
                        }
                    }
                    else
                    {
                        return HeroState.NeedBuy;
                    }
                }
            }
            HeroArenaNumber++;//Арены считаем с ноля, но игрок видит их с еденицы, чтобы было красиво в интерфейсе
            return HeroState.None;
        }

        private void SetButtons()
        {
            if (GetNextHero(out ushort nextIndex))
            {
                if (Heroes.Instance.Get(nextIndex, out BinaryHero nextHero))
                {
                    DownButtons.EnableRightArrow(true);

                    Color nextColor = Color.white;
                    if (ColorUtility.TryParseHtmlString(nextHero.color, out Color _color))
                    {
                        nextColor = _color;
                    }

                    DownButtons.SetRightArrow(nextHero.title, nextColor);
                }
                else
                {
                    DownButtons.EnableRightArrow(false);
                }
            }            
            else
            {
                DownButtons.EnableRightArrow(false);
            }


            if (GetPreviousHero(out ushort previousHeroIndex))
            {
                if (Heroes.Instance.Get(previousHeroIndex, out BinaryHero previousHero))
                {
                    DownButtons.EnableLeftArrow(true);

                    Color previousColor = Color.white;
                    if (ColorUtility.TryParseHtmlString(previousHero.color, out Color _color))
                    {
                        previousColor = _color;
                    }

                    DownButtons.SetLeftArrow(previousHero.title, previousColor);
                }
                else
                {
                    DownButtons.EnableLeftArrow(false);
                }
            }            
            else
            {
                DownButtons.EnableLeftArrow(false);
            }

        }

        private void SetName()
        {
            if (ColorUtility.TryParseHtmlString(BinaryHero.color, out Color _color))
            {
                Name.SetName(BinaryHero.title, _color);
            }
        }

        public void SetCurrentParametr(byte paramIndex)
        {
            byte heroLevel = (byte)(PlayerHero == null ? 1 : PlayerHero.level);
            float damage = HeroOffence._damage(
                settings.minions.damage,
                heroLevel
            );
            UnitParamType valueType = (UnitParamType)paramIndex;
            HeroParams[paramIndex].UpdateParamView(valueType);
            float value = 0.0f;
            switch (valueType)
            {
                case UnitParamType.AttackSpeed:
                    value = HeroOffence.duration / 1000f;
                    HeroParams[paramIndex].SetAdditionalValue(CardWindowDataBehaviour.GetAttackSpeedPrefix(value));
                    break;
                case UnitParamType.AttackType:
                    value = HeroOffence.radius;
                    HeroParams[paramIndex].SetAdditionalValue(CardWindowDataBehaviour.GetAttackTypePrefix(HeroOffence.type));
                    break;
                case UnitParamType.DMG:
                    value = damage;
                    break;
                case UnitParamType.DPS:
                    value = damage * 1000 / HeroOffence.duration;
                    break;
                case UnitParamType.HP:
                    value = HeroDefence._health(settings.minions.health, heroLevel);
                    break;
                case UnitParamType.Targets:
                    value = HeroOffence.target;
                    break;
                default:
                    break;
            }

            HeroParams[paramIndex].SetValue(value, PlayerHero != null);
        }

        void SetParams()
        {
            byte heroLevel = PlayerHero?.level ?? 1;
            float damage = HeroOffence._damage(
                settings.minions.damage,
                heroLevel
            );
            for (byte i = 0; i < HeroParams.Length; i++)
            {
                HeroParams[i].gameObject.SetActive(true);
            }
            for (byte i = 0; i < HeroParams.Length; i++)
            {
                UnitParamType valueType = (UnitParamType)i;
                HeroParams[i].UpdateParamView(valueType);
                float value = 0.0f;
                switch (valueType)
                {
                    case UnitParamType.AttackSpeed:
                        value = (float)HeroOffence.duration / 1000;
                        HeroParams[i].SetAdditionalValue(CardWindowDataBehaviour.GetAttackSpeedPrefix(value));
                        break;
                    case UnitParamType.AttackType:
                        value = HeroOffence.radius;
                        HeroParams[i].SetAdditionalValue(CardWindowDataBehaviour.GetAttackTypePrefix(HeroOffence.type));
                        break;
                    case UnitParamType.DMG:
                        value = damage;
                        HeroParams[i].gameObject.SetActive(value > 1);
                        break;
                    case UnitParamType.DPS:
                        value = damage * 1000 / HeroOffence.duration;
                        HeroParams[i].gameObject.SetActive(value > 1);
                        break;
                    case UnitParamType.HP:
                        value = HeroDefence._health(settings.minions.health, heroLevel);
                        HeroParams[i].gameObject.SetActive(value > 1);
                        break;
                    case UnitParamType.Targets:
                        value = HeroOffence.target;
                        break;
                    default:
                        break;
                }

                HeroParams[i].SetValue(value, PlayerHero != null);
            }
        }

        public void UpgradeHero()
        {
            if (DownButtons.CanUpdate.Item2)
            {
                Profile.UpgradeHero(BinaryHero.index);
                AnalyticsManager.Instance.HeroUpgrade(BinaryHero.index, HeroLvl.Level + 1, (int)PlayerHero.UpdatePrice);
                if (DownButtons.CanUpdate.Item1)
                     DownButtons.UpgradeButtonVisible(false);

                //Так как игрок может улучшить героя до того как успеет начаться туториал улучшения героя - мы проходим тутор по первому же улучшению героя
                SoftTutorialManager.Instance.CompliteTutorial(SoftTutorial.SoftTutorialState.UpgradeHero);
            }
            /*if(HomeTutorialHelper.Instance.HardHomeTutorStep == 14)
            {
                HomeTutorialHelper.Instance.HardHomeTutorStep = 15;
            }*/
        /*    else
            {
                 WindowManager.Instance.OpenNotEnoughCoinsWindow(PlayerHero.UpdatePrice - Profile.Stock.GetCount(CurrencyType.Soft), OnPlayerProfileWait);
                //  WindowManager.Instance.Home();
                //  WindowManager.Instance.MainWindow.OpenShopWithSection(RedirectMenuSection.BankCoins);
            }*/
        }

        private void OnPlayerProfileWait()
        {
            UpdateAll();
        }

        void BuyRealHero(FixedString4096 receipt)
        {
            Profile.CreateHero(BinaryHero.index, receipt);
            SuccesBought();
        }

        private void SuccesBought()
        {
            //TODO: success buying Hero
        }

        public void BuyHero()
        {
            if (DownButtons.CanBuy)
            {
                if (BinaryHero.price.isReal)
                {
#if UNITY_EDITOR
                    Profile.CreateHero(BinaryHero.index);
#elif UNITY_ANDROID
                    IAPManager.Instance.BuyCustomKey(BinaryHero.price.store_key, (receipt) => {
                        BuyRealHero(receipt);
                    });
#else
                    Profile.CreateHero(BinaryHero.index);
#endif

                }
                else
                {
                    Profile.CreateHero(BinaryHero.index);
                }
                Profile.PlayerProfileUpdated.AddListener(UpdateAfterBuy);
            }
            else
            {
                
                WindowManager.Instance.Home();
                WindowManager.Instance.MainWindow.OpenShopWithSection(BinaryHero.price.isSoft ? RedirectMenuSection.BankCoins : RedirectMenuSection.BankGems);
            }
        }

        private void OnDestroy()
        {
            Profile?.PlayerProfileUpdated?.RemoveListener(UpdateAll);
            Profile?.PlayerProfileUpdated?.RemoveListener(UpdateAfterBuy);
        }
    }
}