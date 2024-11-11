using Legacy.Client;
using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static Legacy.Client.HeroParamBehaviour;

public enum LvlUpHeroStates
{
    EmptyStartState,
    SetHeroAnimation,
    SkillsDriveOff,
    ParametrsDriveOff,
    PlayMainEffect,
    ProgressBarToZero,
    LevelDigitPun_k,
    CameraShake,
    SpawnParametr1,
    SpawnParametr2,
    SpawnParametr3,
    HideSkills,
    HideParametrs,
    PreLastState,
    DoNothing
}

public class LevelUpHeroBehavior : MonoBehaviour
{
    public static LevelUpHeroBehavior Instance;

    // тайменги для протапывания анимации для timelineController
    private static readonly float[] TIMELINE_TIMES = { 2.59f, 3.46f, 4.31f, 5.30f };

    public float waitTime = 0;
    [SerializeField] private LvlUpHeroStates currentState;
    [SerializeField] private LvlUpHeroStates previousState;
    [SerializeField] private HeroLevelUpAnimationsController animationsController;
    [SerializeField] private GameObject timelineController;
    [SerializeField] public float stateTime = 0;
    [SerializeField] private GameObject GalahardLevelUpEffect;
    [SerializeField] private HeroLvlBehaviour progressLvlBehaviour;
    [SerializeField] private GameObject heroEnlargedParam;
    [SerializeField] private Transform paramParent;
    [SerializeField] private Image blockImage;
    [SerializeField] private AnimationCurve shakeCurve;
    [SerializeField] private HeroParamBehaviour heroParamHp;
    [SerializeField] private HeroParamBehaviour heroParamDmg;
    [SerializeField] private HeroParamBehaviour heroParamDps;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip upHeroClip;

    private GameObject param1;
    private GameObject param2;
    private GameObject effect;
    private Transform mainEffectParent;
    private float elapsedTime = 0;
    private float elapsedTimeShake = 0;
    private ushort previousLevel;
    private LvlUpHeroStates tempState = LvlUpHeroStates.EmptyStartState;

    private const float shakeYAmptitude = 12;
    private const float shakeXAmptitude = 4;
    private float timer = 1;
    private void OnEnable()
    {
        Instance = this;
        SwitchState(LvlUpHeroStates.PreLastState);
        blockImage.GetComponent<Canvas>().sortingLayerName = "CrossScene";
    }

    private void Reset()
    {
        previousState = LvlUpHeroStates.EmptyStartState;
        SwitchState(LvlUpHeroStates.EmptyStartState);
        mainEffectParent = MenuHeroesBehaviour.Instance.GetHeroContainer();
        waitTime = 0;
    }
    private void SwitchState(LvlUpHeroStates state, float waitTimeBeforeSwitch = 0)
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
        if (currentState != LvlUpHeroStates.DoNothing)
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
            currentState = LvlUpHeroStates.DoNothing;
        }
    }

    public void SkipStep()
    {
        PlayableDirector playableDirector = timelineController.GetComponent<PlayableDirector>();
		foreach (float time in TIMELINE_TIMES)
		{
			if (playableDirector.time < time)
			{
                playableDirector.time = time;

				break;
            }
		}
        
        timer = 0;
        stateTime = 0;
        waitTime = 0.0f;
        elapsedTime = 1;
        elapsedTimeShake = 1;
        /*HeroLevelUpAnimationsController.SetAnimationToEnd();*/
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SkipStep();
        }

        StatesOrders();
        stateTime += Time.deltaTime;
    }

    public void SwitchUpgradeEffect()
    {
        GetHeroParamsData();
        if (
            PlayerHero.UpdatePrice <= ClientWorld.Instance.Profile.Stock.getItem(CurrencyType.Soft).Count
            && PlayerHero.level < ClientWorld.Instance.Profile.Level.level)
        {
            Reset();
            StatesOrders();
        }
    }

    private void StatesOrders()
    {
        switch (currentState)
        {
            case LvlUpHeroStates.EmptyStartState://+
                animationsController.SetStart();
                GalahardLevelUpEffect.gameObject.SetActive(false);
                NextState();
                break;
            case LvlUpHeroStates.SetHeroAnimation://-
                waitTime = 0f;
                MenuHeroesBehaviour.Instance.PlayHelloIdle(this.GetComponent<HeroWindowBehaviour>().GetСhosenHero());
                blockImage.enabled = true;
                NextStateOnce();
                break;
            case LvlUpHeroStates.SkillsDriveOff://+
                HeroLevelUpAnimationsController.NextStartAnimation(false, 0.05f);
                break;
            case LvlUpHeroStates.ParametrsDriveOff://+
                HeroLevelUpAnimationsController.NextStartAnimation(true, 0.05f);
                break;
            case LvlUpHeroStates.PlayMainEffect://+
                waitTime = 0.1f;
                source.clip = upHeroClip;
                SoundManager.Instance.FadeOutMenuMusic(upHeroClip.length);
                source.Play();
                GalahardLevelUpEffect.gameObject.SetActive(true);
                elapsedTimeShake = 0;
                NextStateOnce();
                break;
            case LvlUpHeroStates.ProgressBarToZero://+
                waitTime = 1.3f;
                progressLvlBehaviour.AnimationToZero(waitTime);
                NextStateOnce();
                break;
            case LvlUpHeroStates.LevelDigitPun_k://+
                HeroLevelUpAnimationsController.NextStartAnimation(false, 0.1f);
                //GetHeroParamsData();
                //byte heroLevel = (byte)(PlayerHero == null ? 1 : PlayerHero.level);
                //byte heroExp = (byte)(PlayerHero == null ? 0 : PlayerHero.exp);
                //progressLvlBehaviour.SetData(heroLevel, heroExp);
                break;
            case LvlUpHeroStates.CameraShake://-
                waitTime = 1.1f;
                NextStateOnce();
                break;
            case LvlUpHeroStates.SpawnParametr1://+
                waitTime = 1.1f;
                NextStateOnce();
              //  HeroLevelUpAnimationsController.NextStartAnimation(false, 1.15f);
                break;
            case LvlUpHeroStates.SpawnParametr2://+
                waitTime = 1.1f;
                NextStateOnce();
              //  HeroLevelUpAnimationsController.NextStartAnimation(false, 1.15f);
                break;
            case LvlUpHeroStates.SpawnParametr3://+
                waitTime = 1.1f;
                NextStateOnce();
             //   HeroLevelUpAnimationsController.NextStartAnimation(false, 0f);
                break;
            case LvlUpHeroStates.HideSkills://+
                waitTime = 2f;
                NextStateOnce();
                //    HeroLevelUpAnimationsController.NextStartAnimation(true, 0f);
                break;
            case LvlUpHeroStates.HideParametrs://+
                waitTime = 1f;
                NextStateOnce();
                //   HeroLevelUpAnimationsController.NextStartAnimation(true, 0f);
                //blockImage.enabled = false;
                break;
            case LvlUpHeroStates.PreLastState://+
                NextStateOnce();
                break;
            case LvlUpHeroStates.DoNothing://+
                NextStateOnce();
                break;
            default:
                break;
        }
    }

    public bool TapToContinueEnabled
    {
        get => !blockImage.enabled;
    }

    public IEnumerator LerpCoroutine(int startValue, int startAddValue, int endValue, int endAddValue,
           TextMeshProUGUI tmp, TextMeshProUGUI tmpAdd, float time = 0.5f)
    {
        timer = time;
        yield return new WaitForSeconds(1f);
        while (timer > 0)
        {
            var value = (int)Mathf.Lerp(startValue, endValue, 1 - GetAnchorPersentage(timer));
            tmp.text = value.ToString();
            timer -= Time.deltaTime;

            var value2 = (int)Mathf.Lerp(startAddValue, 0, 1 - GetAnchorPersentage(timer));
            tmpAdd.text = "+" + value2.ToString();

            yield return null;
        }
        if (tmp.text != (endValue).ToString())
        {
            tmp.text = endValue.ToString();
        }
        if (tmpAdd.text != (endAddValue).ToString())
        {
            tmpAdd.text = "";
        }
          timer = time;
      //  var nextNeed =  (int)heroParam.GetDifferenceValue()
        yield return new WaitForSeconds(1.0f);

        tmpAdd.text = "+" + endAddValue.ToString();

        this.GetComponent<HeroWindowBehaviour>().UpdateAfterUp();
        blockImage.enabled = false;

        StopAllCoroutines();

        /* while (timer > 0)
         {
             var value = (int)Mathf.Lerp(startAddValue, endAddValue, 1 - GetAnchorPersentage(timer));
             tmpAdd.text = "+" + value.ToString();
             timer -= Time.deltaTime;
             yield return null;
         }*/
    }
    private float GetAnchorPersentage(float remainTime)
    {
        var t = remainTime / 1;
        return t;
    }

    public IEnumerator ShakeCoroutine(float time = 1, float XAmplitude = shakeXAmptitude, float YAmplitude = shakeYAmptitude)
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
        var heroWindowPositionY = shakeCurve.Evaluate(elapsedTime * Mathf.PI) * XAmplitude * Mathf.Sqrt((1 - elapsedTime));
        var heroWindowPositionX = shakeCurve.Evaluate(elapsedTime * Mathf.PI) * YAmplitude * Mathf.Sqrt((1 - elapsedTime));
        this.transform.localPosition = new Vector3(heroWindowPositionX, heroWindowPositionY, 0);
    }

    public void HideEffect()
    {
                GalahardLevelUpEffect.gameObject.SetActive(false);

    }
    private void OnDisable()
    {
        HideEffect();
    }
    private BinaryHero BinaryHero;
    private BinaryEntity BinaryMinion;
    private MinionOffence HeroOffence;
    private MinionDefence HeroDefence;
    private PlayerProfileHero PlayerHero;
    private BaseBattleSettings settings;
    private void GetHeroParamsData()
    {
        var hero_index = this.GetComponent<HeroWindowBehaviour>().GetСhosenHero();
        BinaryHero = default;
        PlayerHero = default;
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
}
