using System;
using System.Collections.Generic;
using Legacy.Database;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Legacy.Client
{
    public enum BattleResult : byte
    {
        Defeat = 0,
        Victory = 1
    }
    public class BattleDataContainer : MonoBehaviour
    {
        public static BattleDataContainer Instance;
        
        private BattleRatingResultReward battleRatingResultReward;
        private bool isNewArena;
        private bool isNewArenaForChanged;
        public byte starsWeGot;
        public byte starsWeGotInBatte;

        public int RatingDelta => (int)(battleRatingResultReward.rating - PreviousRating);
        //public int HeroExpDelta => (int)(battleRatingResultReward.hero.exp - previousHeroExp);
        public ushort BattleLasting => battleRatingResultReward.result.battleLasting;

        public uint PreviousRating { get; private set; }
        public uint PreviousMaxRating { get; private set; }

        //private int previousHeroExp;

        public ushort Soft => battleRatingResultReward.soft;
        public bool isVictory => battleResult > 0;
        private BattleResult battleResult;

        public EventArenaData PreviousArena { get; private set; }

        void Start()
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(this);
        }

        public void CheckRewardParticles(Vector3 position)
        {            
            if (starsWeGot > 0)
            {
                RewardParticlesBehaviour.Instance.Queue(position, starsWeGot, LootBoxWindowBehaviour.LootCardType.Star);
                starsWeGot = 0;
            }
            /*if (HeroExpDelta > 0)
            {
                RewardParticlesBehaviour.Instance.Queue(position, (byte)HeroExpDelta, LootBoxWindowBehaviour.LootCardType.HeroExp);
                previousHeroExp = (int)battleRatingResultReward.hero.exp;
            }*/
            if (battleRatingResultReward.soft > 0)
            {
                RewardParticlesBehaviour.Instance.Queue(position, (byte)battleRatingResultReward.soft, LootBoxWindowBehaviour.LootCardType.Soft);
                battleRatingResultReward.soft = 0;
            }
            if (RatingDelta > 0)
            {
                RewardParticlesBehaviour.Instance.Queue(position, (byte)RatingDelta, LootBoxWindowBehaviour.LootCardType.Rating);
                PreviousRating = battleRatingResultReward.rating;
            }
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var gs = scene.GetRootGameObjects();
            foreach(var g in gs)
            {
                if(g.GetComponent<BattleDataContainer>())
                {
                    DestroyImmediate(g);
                }
            }
        }

        public void SetBattleData(BattleRatingResultReward battleResult)
        {
            battleRatingResultReward = battleResult;

            var profile = ClientWorld.Instance.Profile;
            var current = profile.Rating.max;
            PreviousRating = profile.Rating.current;
            PreviousMaxRating = profile.Rating.max;
            //previousHeroExp = (int)profile.CurrentHero.exp;
            var currentArena = Settings.Instance.Get<ArenaSettings>().GetArenaData((ushort) current);
            var newRating =  battleRatingResultReward.rating;
            var newArena = Settings.Instance.Get<ArenaSettings>().GetArenaData((ushort) newRating);
            starsWeGotInBatte = battleRatingResultReward.startInBattle;

            this.battleResult = battleResult.isWinner ? BattleResult.Victory : BattleResult.Defeat;
            if (profile.HardTutorialState < 4)
            {
                starsWeGot = 0;
            }
            else
            {
				if (Shop.Instance.BattlePass.GetCurrent() != null)
				{
                    //  starsWeGot = (byte)profile.battlePass.stars;
                    starsWeGot = starsWeGotInBatte;
                }
				else
				{
                    starsWeGot = 0;
				}
            }

            PreviousArena = currentArena;
            isNewArena = currentArena.IsTutorial | currentArena.number < newArena.number;
            isNewArenaForChanged = isNewArena;

            if (currentArena.number < newArena.number)
            {
                AnalyticsManager.Instance.ArenaOpen(newArena.number);
            }
            else if (profile.HardTutorialState == 3 && currentArena.IsTutorial && battleResult.isWinner)
            {
                AnalyticsManager.Instance.ArenaOpen(newArena.number + 1);
            }

            Debug.Log($"<color=green>Set battle data current </color>" + current + " current arena " + currentArena + " new rating " + newRating + " new arena " + newArena + "is new " + isNewArena);
        }

        public bool CheckForNewArena()
        {
            return isNewArena;
        }
        public void NewArenaShown()
        {
            isNewArena = false;
        }

        public bool CheckForNewArenaForChanged()
        {
            return isNewArenaForChanged;
        }
        public void NewArenaForChangedShown()
        {
            isNewArenaForChanged = false;
        }

        internal ushort GetLootIndex()
        {
            return battleRatingResultReward.lootbox.index;
        }

        internal byte GetRewardResourcesSlotsCount()
        {
            byte count = 0;
            if (RatingDelta != 0)
            {
                count++;
            }
            /*if (HeroExpDelta > 0)
            {
                count++;
            }*/
            if(battleRatingResultReward.soft > 0)
            {
                count++;
            }
            return count;
        }
    }
}