using Legacy.Client;
using Legacy.Database;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

using UnityEngine;

using UnityEngine.UI;

public enum SkillUpgradeStates
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
    CalmState,
    PreLastState,
    LastState,
    DoNothing
}
public class SkillUpgradeBehaviour : MonoBehaviour
{
    public float waitTime = 0;
    public List<Animator> skillParams = new List<Animator>();
    [SerializeField] private SkillUpgradeStates currentState;
    [SerializeField] private SkillUpgradeStates previousState;
    [SerializeField] private SkillUpgradeNimationsController animationsController;
    [SerializeField] private GameObject MainSkillEffect;
    [SerializeField] private GameObject ButtonSkillEffect;
    [SerializeField] private Image blockImage;
    [SerializeField] private AnimationCurve shakeCurve;
    [SerializeField] private FullSkillInfoBehavior skill;
    [SerializeField] private AudioClip mainEffectClip;

    private float stateTime = 0;
    private float elapsedTimeShake = 0;
    private SkillUpgradeStates tempState = SkillUpgradeStates.EmptyStartState;

    private const float shakeYAmptitude = 12;
    private const float shakeXAmptitude = 4;
    private float timer = 1;
    private bool isAnimation;

    private void Awake()
    {
        var Profile = ClientWorld.Instance.Profile;
        Profile.PlayerProfileUpdated.AddListener(PlayUpgrade);//определять индекс скилла или фулл скила и сетить жффект 1му скиллу
    }
    private void OnEnable()
    {
        blockImage.gameObject.SetActive(false);
        SwitchState(SkillUpgradeStates.LastState);
        blockImage.GetComponent<Canvas>().sortingLayerName = "CrossScene";
        waitTime = 0;
       this.GetComponentsInChildren<SkillUpgradeNimationsController>().ToList().ForEach(x => x.heroWindow = this);
    }

    public void SwitchState(SkillUpgradeStates state, float waitTimeBeforeSwitch = 0)
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
        if (currentState != SkillUpgradeStates.DoNothing)
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
            currentState = SkillUpgradeStates.DoNothing;
        }
    }

    public void PlayUpgrade()
    {
        if(!isAnimation)
            SwitchState(SkillUpgradeStates.EmptyStartState);
    }

    private void SkipStep()
    {
        stateTime = 0;
        waitTime = 0.2f;
        elapsedTimeShake = 1;
        animationsController.SetAnimationToEnd();
    }

    private void Update()
    {
        StatesOrders();
        stateTime += Time.deltaTime;
    }

    private void StatesOrders()
    {
        switch (currentState)
        {
            case SkillUpgradeStates.EmptyStartState:
                waitTime = 0f;
                isAnimation = true;
                skillParams = skill.GetParams();
                animationsController.SetStart();
                NextStateOnce();
                break;
            case SkillUpgradeStates.SetCardAnimation:
                waitTime = 0f;
                blockImage.gameObject.SetActive(true);
                NextStateOnce();
                break;
            case SkillUpgradeStates.PlayMainEffect:
                waitTime = 0.3f;
                PlayClip(mainEffectClip);
                MainSkillEffect.SetActive(true);
                ButtonSkillEffect.SetActive(false);
                elapsedTimeShake = 0;
                NextStateOnce();
                break;
            case SkillUpgradeStates.ProgressBarToZero://+
                waitTime = 0f;
                NextStateOnce();
                break;
            case SkillUpgradeStates.LevelDigitPun_k://+
                animationsController.NextStartAnimation(false, 0.1f);
                break;
            case SkillUpgradeStates.CameraShake:
                waitTime = 1f;
                NextStateOnce();
                break;
            case SkillUpgradeStates.SpawnParametr1:
                animationsController.NextStartAnimation(false, 0.6f);
                break;
            case SkillUpgradeStates.LerpParametr1:
                waitTime = 0f;
                NextStateOnce();
                break;
            case SkillUpgradeStates.SpawnParametr2:
                animationsController.NextStartAnimation(false, 0.6f);
                break;
            case SkillUpgradeStates.LerpParametr2:
                waitTime = 0f;
                NextStateOnce();
                break;
            case SkillUpgradeStates.SpawnParametr3:
                animationsController.NextStartAnimation(false, 0.6f);
                break;
            case SkillUpgradeStates.LerpParametr3:
                waitTime = 0f;
                NextStateOnce();
                break;
            case SkillUpgradeStates.CalmState:
                waitTime = 0;
                blockImage.gameObject.SetActive(false);
                MainSkillEffect.SetActive(false);
                ButtonSkillEffect.SetActive(true);
                NextStateOnce();
                break;
            case SkillUpgradeStates.PreLastState:
                waitTime = 0;
                isAnimation = false;
                this.enabled = false;
                NextStateOnce();
                break;
            case SkillUpgradeStates.LastState:
                waitTime = 0;
                NextStateOnce();
                break;
            case SkillUpgradeStates.DoNothing:
                NextStateOnce();
                break;
            default:
                break;
        }
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
        //var heroWindowPositionY = shakeCurve.Evaluate(elapsedTime * Mathf.PI) * XAmplitude * Mathf.Sqrt((1 - elapsedTime));
        //var heroWindowPositionX = shakeCurve.Evaluate(elapsedTime * Mathf.PI) * YAmplitude * Mathf.Sqrt((1 - elapsedTime));
        //this.transform.localPosition = new Vector3(heroWindowPositionX, heroWindowPositionY, 0);
    }

    private void PlayClip(AudioClip clip)
    {
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
    }

    public IEnumerator LerpCoroutine(int startValue, int endValue, TextMeshProUGUI tmp, string addData, float time = 0.5f)
    {
        timer = time;
        while (timer > 0)
        {
            var value = Mathf.Round(Mathf.Lerp(startValue, endValue, 1 - GetAnchorPersentage(timer, time)));
            tmp.text = value.ToString() +" "+ addData;
            timer -= Time.deltaTime;
            yield return null;
        }
            yield return null;
    }

    private float GetAnchorPersentage(float remainTime, float allTime)
    {
        var t = remainTime / allTime * 1.0f;
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

}
