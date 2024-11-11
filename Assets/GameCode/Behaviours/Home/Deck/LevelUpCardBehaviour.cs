using Legacy.Client;
using Legacy.Database;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using System;

namespace Legacy.Client
{
    public enum LvlUpCardStates
    {
        EmptyStartState,
        SetCardAnimation,
        PlayMainEffect,
        ProgressBarToZero,
        LevelDigitPun_k,
        CameraShake,
        SpawnParametr1,
        LerpParametr1,
        SpawnParametr2,
        LerpParametr2,
        SpawnParametr3,
        LerpParametr3,
        SpawnParametr4,
        LerpParametr4,
        CalmState,
        PreLastState,
        DoNothing
    }

    public class LevelUpCardBehaviour : MonoBehaviour, IPointerClickHandler
    {
        public static LevelUpCardBehaviour Instance;
        public float waitTime = 0;
        [SerializeField] private LvlUpCardStates currentState;
        [SerializeField] private LvlUpCardStates previousState;
        [SerializeField] private CardLevelUpAnimationsController animationsController;
        [SerializeField] private PlayableDirector MainCardEffectDirector;
        [SerializeField] private GameObject tapToContinue;
        [SerializeField] private CardProgressBarBehaviour progressLvlBehaviour;
        [SerializeField] private CardUpgradeWindowBehavior cardUpgradeWindow;
        [SerializeField] private GameObject heroEnlargedParam;
        [SerializeField] private Transform paramParent;
        [SerializeField] private Image blockImage;
        [SerializeField] private AnimationCurve shakeCurve;
        [SerializeField] private HeroParamBehaviour heroParamHp;
        [SerializeField] private HeroParamBehaviour heroParamDmg;
        [SerializeField] private HeroParamBehaviour heroParamDps;
        [SerializeField] private HeroParamBehaviour heroParamFmgSturm;
        [SerializeField] private Transform mainEffectParent;
        [SerializeField] private Animator cardIcon;
        [SerializeField, Range(1.0f, 3.0f)] float mainEffectTime = 1.5f;
        [SerializeField] private AudioSource CoinCountLoopSource;
        [SerializeField] private AudioSource CardUpgradeSource;

        private float stateTime = 0;
        private float elapsedTimeShake = 0;
        private LvlUpCardStates tempState = LvlUpCardStates.EmptyStartState;

        private const float shakeYAmptitude = 12;
        private const float shakeXAmptitude = 4;
        private float timer = 1;


        private void OnEnable()
        {
            Instance = this;
            SwitchState(LvlUpCardStates.EmptyStartState);
            //blockImage.GetComponent<Canvas>().sortingLayerName = "CrossScene";
            waitTime = 0;

            // for dev
            animationsController.SetStart();
            blockImage.enabled = false;
            TapToContinue(false);
            progressLvlBehaviour.ProgressValue.AnimationToZero(0.75f);
            CardLevelUpAnimationsController.StartLevelAnimation();
        }

        private void SwitchState(LvlUpCardStates state, float waitTimeBeforeSwitch = 0)
        {
            if (waitTimeBeforeSwitch == 0 || stateTime >= waitTimeBeforeSwitch && currentState != state)
            {
                previousState = currentState;
                currentState = state;
                stateTime = 0;
                waitTime = 0;
            }
        }

        public void NextState(float waitTime = 0)
        {
            var nextState = currentState;
            SwitchState(++nextState, waitTime);
        }

        public void NextStateOnce()
        {
            if (currentState != LvlUpCardStates.DoNothing)
                tempState = currentState + 1;
            if (waitTime == 0 || stateTime >= waitTime)
            {
                previousState = currentState;
                currentState = tempState;
                stateTime = 0;
                waitTime = 0;
            }

            if (stateTime < waitTime)
            {
                currentState = LvlUpCardStates.DoNothing;
            }
        }

        public void SkipStep()
        {
            timer = 0.0f;
            //CardLevelUpAnimationsController.NextStartAnimation(false, 0.1f);
            //animationsController.Lerp();

            //TapToContinue(true);

            /*stateTime = 0;
            timer = 0;
            waitTime = 0f;
            elapsedTimeShake = 1;
            CardLevelUpAnimationsController.SetAnimationToEnd();*/
        }

        private void Update()
        {
            StatesOrders();
            CheckMainEffectTimeline();
            stateTime += Time.deltaTime;
        }

        private void CheckMainEffectTimeline()
        {
            if (MainCardEffectDirector.state == PlayState.Playing)
            {
                if (MainCardEffectDirector.time > mainEffectTime)
                {
                    MainCardEffectDirector.Pause();                    
                }
            }
        }

        private bool MouseButtonDown;
        private void StatesOrders()
        {
            if (Input.GetMouseButtonDown(0) && !MouseButtonDown)
            {
                MouseButtonDown = true;
                SkipStep();
			}
			else
			{
                MouseButtonDown = false;
            }

            /*switch (currentState)
            {
                case LvlUpCardStates.EmptyStartState:
                    CardUpgradeSource.Play();
                    SoundManager.Instance.FadeOutMenuMusic(CardUpgradeSource.clip.length);
                    animationsController.SetStart();
                    TapToContinue(false);
                    blockImage.enabled = false;
                    NextState();
                    break;
                case LvlUpCardStates.SetCardAnimation:
                    waitTime = 0f;
                    NextStateOnce();
                    break;
                case LvlUpCardStates.PlayMainEffect:
                    waitTime = 0.3f;                    
                    //MainCardEffectDirector.Play();
                    elapsedTimeShake = 0;
                    NextStateOnce();
                    break;
                case LvlUpCardStates.ProgressBarToZero: //+
                    waitTime = 0.4f;
                    progressLvlBehaviour.ProgressValue.AnimationToZero(0.75f);
                    NextStateOnce();
                    break;
                case LvlUpCardStates.LevelDigitPun_k: //+
                    CardLevelUpAnimationsController.NextStartAnimation(false, 0.1f);
                    break;
                case LvlUpCardStates.CameraShake:
                    waitTime = 1f;
                    // StartCoroutine(ShakeCoroutine(1f, 4, 1.5f));
                    NextStateOnce();
                    break;
                case LvlUpCardStates.SpawnParametr1:
                    CardLevelUpAnimationsController.NextStartAnimation(false, 0.6f);
                    break;
                case LvlUpCardStates.LerpParametr1:
                    waitTime = 0f;
                    NextStateOnce();
                    break;
                case LvlUpCardStates.SpawnParametr2:
                    CardLevelUpAnimationsController.NextStartAnimation(false, 0.6f);
                    break;
                case LvlUpCardStates.LerpParametr2:
                    waitTime = 0f;
                    NextStateOnce();
                    break;
                case LvlUpCardStates.SpawnParametr3:
                    CardLevelUpAnimationsController.NextStartAnimation(false, 0.6f);
                    break;
                case LvlUpCardStates.LerpParametr3:
                    waitTime = 0f;
                    NextStateOnce();
                    break;
                case LvlUpCardStates.SpawnParametr4:
                    CardLevelUpAnimationsController.NextStartAnimation(false, 0.6f);
                    break;
                case LvlUpCardStates.LerpParametr4:
                    waitTime = 0f;
                    NextStateOnce();
                    break;
                case LvlUpCardStates.CalmState:
                    waitTime = 0;
                    blockImage.enabled = true;
                    TapToContinue(true);
                    NextStateOnce();
                    break;
                case LvlUpCardStates.PreLastState:
                    waitTime = 0;
                    NextStateOnce();
                    break;
                case LvlUpCardStates.DoNothing:
                    NextStateOnce();
                    break;
                default:
                    break;
            }*/
        }

        public void StartNextLerp()
        {
            animationsController.Lerp();
        }

        public void UpdateProgreessBar()
        {
            progressLvlBehaviour.ProgressValue.AnimationToCurrentValue(0.75f);
        }

        public void TapToContinue(bool flag)
        {
            CoinCountLoopSource.Stop();
            blockImage.enabled = flag;
            tapToContinue.SetActive(flag);
        }
        public bool TapToContinueEnabled
        {
            get => tapToContinue.activeInHierarchy;
        }

        public IEnumerator ShakeCoroutine(float time = 1, float XAmplitude = shakeXAmptitude,
            float YAmplitude = shakeYAmptitude)
        {
            elapsedTimeShake = 0;

            do
            {
                elapsedTimeShake += Time.deltaTime;
                Shake(elapsedTimeShake, XAmplitude, YAmplitude);
                yield return null;
            } while (elapsedTimeShake < time);
        }

        private void Shake(float elapsedTime, float XAmplitude, float YAmplitude)
        {
            //var heroWindowPositionY = shakeCurve.Evaluate(elapsedTime * Mathf.PI) * XAmplitude * Mathf.Sqrt((1 - elapsedTime));
            //var heroWindowPositionX = shakeCurve.Evaluate(elapsedTime * Mathf.PI) * YAmplitude * Mathf.Sqrt((1 - elapsedTime));
            //this.transform.localPosition = new Vector3(heroWindowPositionX, heroWindowPositionY, 0);
        }

        public IEnumerator LerpCoroutine(int startValue, int startAddValue, int endValue, int endAddValue,
            TextMeshProUGUI tmp, TextMeshProUGUI tmpAdd, float time = 0.5f)
        {
            var soundCounter = 0;
            timer = time;
            if (endValue - startValue != startAddValue)     
                endValue = startValue + startAddValue;
            while (timer > 0)
            {
                if (soundCounter == 3)
                {
                    CoinCountLoopSource.Play();
                    soundCounter = 0;
                }
                else
                {
                    soundCounter++;
                }

                var value = (int) Mathf.Lerp(startValue, endValue, 1 - GetAnchorPersentage(timer));
                tmp.text = value.ToString();
                var value2 = (int)Mathf.Lerp(startAddValue, 0, 1 - GetAnchorPersentage(timer));
                if(value2>0)
                 tmpAdd.text = "+" + value2.ToString();
                timer -= Time.deltaTime;
                yield return null;
            }
            if(tmp.text != (endValue).ToString()) {
                tmp.text = endValue.ToString();
            }
            timer = time;

			if (CardLevelUpAnimationsController.NextStartAnimation(false, 0.1f))
			{
                animationsController.Lerp();
            }

            //CoinCountLoopSource.Stop();
            
             /*while (timer > 0)
             {
                 var value = (int) Mathf.Lerp(startAddValue, endAddValue, 1 - GetAnchorPersentage(timer));
                 tmpAdd.text = "+" + value.ToString();
                 timer -= Time.deltaTime;
                 yield return null;
             }*/
            if (tmpAdd.text != (endAddValue).ToString())
            {
                tmpAdd.text = "";// endAddValue.ToString();
            }
        }

        private float GetAnchorPersentage(float remainTime)
        {
            var t = remainTime / 1;
            return t;
        }





        private BinaryHero BinaryHero;
        private BinaryEntity BinaryMinion;
        private MinionOffence HeroOffence;
        private MinionDefence HeroDefence;
        private PlayerProfileHero PlayerHero = null;
        private BaseBattleSettings settings;

        private void GetHeroParamsData()
        {
            var hero_index = this.GetComponent<HeroWindowBehaviour>().GetСhosenHero();
            BinaryHero = default;
            PlayerHero = null;
            HeroOffence = default;
            HeroDefence = default;
            if (ClientWorld.Instance.Profile.GetPlayerHero(hero_index, out PlayerProfileHero _hero))
            {
                PlayerHero = _hero;
            }

            if (Heroes.Instance.Get(hero_index, out BinaryHero binaryHero))
            {
                BinaryHero = binaryHero;

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
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {

        }
    }
}