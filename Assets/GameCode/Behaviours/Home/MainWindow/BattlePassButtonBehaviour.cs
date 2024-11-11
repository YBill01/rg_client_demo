using System;
using System.Collections;
using Legacy.Database;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class BattlePassButtonBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private ProgressBarChangeValueBehaviour progressBar;

        [SerializeField] ParticleSystem ActiveVFX;
        [SerializeField] ParticleSystem CanCollectVFX;
        [SerializeField] ParticleSystem ShiftVFX;

        [SerializeField, Range(0.0f, 3.0f)] float ShiftTime;

        [SerializeField]
        private GameObject ActivePass;
        [SerializeField]
        private GameObject CollectPass;
        [SerializeField]
        private GameObject TimerPass;
        [SerializeField]
        private GameObject noActiveEventText;
        
        [SerializeField]
        private GameObject ProgressBar;
        [SerializeField]
        private TMP_Text TextName;

        [SerializeField]
        private TMP_Text levelText;
        [SerializeField] 
        private LegacyButton button;

        private BinaryBattlePass battlePass;
        private ProfileInstance profile;
        private int holdValue;
        private byte currentLevel;
        private byte starsInCurrentLevel;

        public void Init()
        {
            profile = ClientWorld.Instance.Profile;
            battlePass = Shop.Instance.BattlePass.GetCurrent();

            if (battlePass != null)
            {
                progressBar.FullBarEvent.RemoveListener(CollectEffect);
                progressBar.FullBarEvent.AddListener(CollectEffect);
                //SetState();
            }
            else
            {
                SetActiveBattlePass(false);
            }
        }

        private void CollectEffect()
        {
            progressBar.StopUpdating(true);
            StartCoroutine(ActivateCollect());
        }

        private void CalculateLevelStars()
        {
            if (battlePass != null)
            {
                holdValue = progressBar.GetHoldValue();
                //Debug.Log($"holdValue: {holdValue}");
                //Debug.Log($"profile.battlePass.stars: {profile.battlePass.stars}");
                currentLevel = (byte)(Mathf.Floor((profile.battlePass.stars - holdValue) / 10) + 1);
                starsInCurrentLevel = (byte)(profile.battlePass.stars - holdValue - (currentLevel - 1) * 10);
                //Debug.Log($"starsInCurrentLevel: {starsInCurrentLevel}");

                if (currentLevel >= battlePass.tresures.Count)
                {
                    starsInCurrentLevel = 10;
                }
            }
        }

        private void SetState()
        {
            CalculateLevelStars();

            SetActiveBattlePass(Shop.Instance.BattlePass.GetCurrent() != null);
                
            SetProgressBar();
        }

        private IEnumerator ActivateCollect()
        {
            ShiftVFX.gameObject.SetActive(true);
            yield return new WaitForSeconds(ShiftTime);
            progressBar.StopUpdating(false);
            SetState();
        }

        private void SetActiveBattlePass(bool isActive = true)
        {
            button.isLocked = !isActive;
            button.gameObject.SetActive(isActive);
            button.interactable = isActive;
            noActiveEventText.SetActive(false);
            ActivePass.SetActive(false);
            CollectPass.SetActive(false);

            if (isActive)
            {
                var collectableRewards = CollectableLevelsCount();
                if (collectableRewards >0/* (holdValue > 0 ? 1 : 0)*/ || starsInCurrentLevel > 9)
                {
                    CollectPass.SetActive(true);
                }
                else
                {
                    levelText.text = (currentLevel).ToString();
                  //  levelText.text = (starsInCurrentLevel).ToString();
                    ActivePass.SetActive(true);
                }
            }
            else
            {
                button.localeAlert = Locales.Get("locale:1291");
                TextName.text = Locales.Get("locale:1291");
                DisableShiftEffect();
                DisableActiveEffect();
                ProgressBar.SetActive(false);
                ActivePass.SetActive(true);
                button.gameObject.SetActive(true);
            }            
        }

        private void SetProgressBar()
        {
			if (holdValue > 0)
			{
                //ushort holdValueModule = (ushort)(holdValue % 10);
                ushort starsValueModule = (ushort)(profile.battlePass.stars % 10);

                ushort starsCount = (ushort)(profile.battlePass.stars - holdValue);
                int holdValueResult = starsValueModule - starsCount;
				if (holdValueResult < 0)
				{
                    holdValueResult += 10;
                    if (holdValueResult < 0)
                    {
                        holdValueResult = profile.battlePass.stars - starsCount;
                    }
                       // holdValueResult = profile.battlePass.stars - starsCount;
				}

                progressBar.SetHoldValue((uint)(holdValueResult));
                //   progressBar.Set((byte)(profile.battlePass.stars % 10), true, 10);// - в 30 = 0;
                if (starsValueModule == 0)
                    starsValueModule = 10;
                progressBar.Set((byte)(starsValueModule), true, 10);
            }
			else
			{
                progressBar.Set((byte)(profile.battlePass.stars % 10), true, 10);
            }

            //progressBar.Set((byte)(starsInCurrentLevel + holdValue), true, 10);
        }
        
        public void UpdateView()
        {
            SetState();
        }
        public int CollectableLevelsCount()
        {
            int countToCollect = 0;

            if (battlePass != null)
            {
                if (currentLevel >= battlePass.tresures.Count)
                    currentLevel = (byte)(battlePass.tresures.Count - 1);

                byte openedLevels = (byte)(DateTime.Now - battlePass.timeStart.ToLocalTime()).Days;

                /*if (currentLevel > openedLevels)
                    currentLevel = openedLevels;*/


                for (byte i = 0; i < currentLevel; i++)
                {
                    if (!profile.battlePass.HasFreeReward(i))
                    {
                        countToCollect++;
                        continue;
                    }
                    else
                    {
                        if (profile.battlePass.isPremiumBought)
                        {
                            if (!profile.battlePass.HasPremiumReward(i))
                            {
                                countToCollect++;
                            }
                        }
                    }
                }
            }
            return countToCollect;
        }

        internal void DisableShiftEffect()
        {
            ShiftVFX.gameObject.SetActive(false);
        }
        internal void DisableActiveEffect()
        {
            ActiveVFX.gameObject.SetActive(false);
        }
        
        public void SetCollectEffect(bool value)
        {
            CanCollectVFX.gameObject.SetActive(value);
        }
    }
}