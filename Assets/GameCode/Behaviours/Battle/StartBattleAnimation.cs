using Legacy.Client;
using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;



public enum StartBattleStates
{
    ShowArenaInfo,
    EmptyState,//disable animator//already
    PlayCameraZoom,//start zoom camera
    FadeArenaInfo,//play music
    ShowUpPanel,
    ShowStartText,
    ShowDownPanel,
    HandCardsState,
    InitHeroHPBars,
    StopCamera,
    HideStartText,
    PreLastState,
    LastState,
    DoNothing
}


public class StartBattleAnimation : MonoBehaviour
{
    public static StartBattleAnimation instance;

    public TimerBehaviour TimerText;
    public GameObject textViewer;

    [SerializeField] private Image blockImage;
    [SerializeField] private StartBattleStates currentState;
    [SerializeField] private StartBattleStates previousState;
    [SerializeField] private GameObject ArenaViewer;
    [SerializeField] private TextMeshProUGUI ArenaName;
    [SerializeField] private TextMeshProUGUI ArenaNumber;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip battleOpenClip;

    [SerializeField] private HandBehaviour HandBehaviour;
    private float waitTime = 0;
    private StartBattleStates tempState = StartBattleStates.ShowArenaInfo;
    private ProfileInstance Profile;
    private EventArenaData arenaData;
    private float stateTime = 0;
    private float timer = 2f;
    private bool canShowText = false;
    private StateMachineSystem _state_machine_system;

    private const float minTimeToShowArena = 0.5f;
    private const float fadeTime = 0.3f;
    private const float anyTimeToEnd = 3.2f;
    private const float timeToShowInterface = 1f;
    private const float handGetCardsDelay = 0.75f;
    void Awake()
    {
        instance = this;
        Profile = ClientWorld.Instance.Profile;
        arenaData = Settings.Instance.Get<ArenaSettings>().GetArenaData((ushort)Profile.Rating.current);
        LoadingGroup.Instance.LoadingDisabled.AddListener(OnScreenVisible);
        _state_machine_system = ClientWorld.Instance.GetOrCreateSystem<StateMachineSystem>();
    }

    private void OnEnable()
    {
        SwitchState(StartBattleStates.ShowArenaInfo);
        waitTime = 0;
    }

    private void Start()
    {
        if (_state_machine_system.IsConnectedTooExistedBattle)
        {
            BattleInstanceInterface.instance.CanUpdateHand();
        }
        if(Profile.HardTutorialState == 0)
        {
            BattleInstanceInterface.instance.Skill1.viewBehaviour.HideSkillView();
            BattleInstanceInterface.instance.Skill2.viewBehaviour.HideSkillView();

        }
        else if(Profile.HardTutorialState == 1)
        {
            BattleInstanceInterface.instance.Skill2.viewBehaviour.HideSkillView();
        }
    }

    public void SwitchState(StartBattleStates state, float waitTimeBeforeSwitch = 0)
    {
        if (waitTimeBeforeSwitch == 0 || stateTime >= waitTimeBeforeSwitch && currentState != state)
        {
            previousState = currentState;
            currentState = state;
            stateTime = 0;
            waitTime = waitTimeBeforeSwitch;
        }
    }

    public void NextState(float waitTime = 0)
    {
        var nextState = currentState;
        SwitchState(++nextState, waitTime);
    }

    public void NextStateOnce()
    {
        if (currentState != StartBattleStates.DoNothing)
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
            currentState = StartBattleStates.DoNothing;
        }
    }

    private void SkipStep()
    {
        stateTime = 0;
        waitTime = 0f;
        currentState++;
        //
    }

    //for tutorial
    public void ShowInterface()
    {
        waitTime = 0;
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
            case StartBattleStates.ShowArenaInfo:
                waitTime = 0;
                SetArenaInfo();
                NextStateOnce();
                break;
            case StartBattleStates.EmptyState:
                break;
            case StartBattleStates.PlayCameraZoom:
                waitTime = timer - fadeTime;
                // Если у нас не туториал и не песочница
                if (!canShowText) 
                    waitTime = 100000000;// Анимация продолжится оооооочень не скоро. По факту - когда мы скажем waitTime = 0 в методе ShowInterface
                if (ClientWorld.Instance.isSandbox) 
                    waitTime = 0;
                if (canShowText)
                {
                    MainCameraMoveBehaviour.instance.StartZoomCamera(4f, 75f);
                }
                else
                {
                    MainCameraMoveBehaviour.instance.StartZoomCamera(0.25f, 75f);
                }
                NextStateOnce();
                break;
            case StartBattleStates.FadeArenaInfo:
                if (canShowText)
                {
                    waitTime = fadeTime;
                }
                ArenaViewer.GetComponent<Animator>().Play("fade");
                NextStateOnce();
                break;
            case StartBattleStates.ShowUpPanel:
                if (canShowText)
                {
                    waitTime = 0.5f;
                }
                SoundManager.Instance.PlayBattleMusic();
                BattleInstanceInterface.instance.ShowUpPanel();
                NextStateOnce();
                break;
            case StartBattleStates.ShowStartText:
                if (canShowText)
                {
                    waitTime = 0.5f;
                }
                if (!canShowText)
                {
                    MainCameraMoveBehaviour.instance.StartZoomCamera(0.3f, 100f);
                }
                TimerText.SetStartBattleAnimationTime();
                ShowStartText();
                NextStateOnce();
                break;
            case StartBattleStates.ShowDownPanel:
                waitTime = 0.5f;
                BattleInstanceInterface.instance.ShowDownPanel();
                NextStateOnce();
                break;
            case StartBattleStates.HandCardsState:
                waitTime = 0.75f;
                BattleInstanceInterface.instance.CanUpdateHand();
                BattleInstanceInterface.instance.UpdateHand();
                NextStateOnce();
                break;
            case StartBattleStates.InitHeroHPBars:
                waitTime = 0.5f;
                foreach (var bar in BattleInstanceInterface.instance.canvas.GetComponentsInChildren<HeroDamageBar>())
                {
                    bar.ScaleInitHpBar();
                }
                NextStateOnce();
                break;
            case StartBattleStates.StopCamera:
                waitTime = anyTimeToEnd;
                NextStateOnce();
                break;
            case StartBattleStates.HideStartText:
                waitTime = 0;
                textViewer.GetComponent<Animator>().enabled = (true);
                textViewer.GetComponent<Animator>().Play("fade");
                NextStateOnce();
                break;
            case StartBattleStates.PreLastState:
                waitTime = 0;
                NextStateOnce();
                break;
            case StartBattleStates.LastState:
                waitTime = 0;
                NextStateOnce();
                break;
            case StartBattleStates.DoNothing:
                NextStateOnce();
                break;
            default:
                break;
        }
    }

    private void OnScreenVisible()
    {
        canShowText = !Profile.IsBattleTutorial && !ClientWorld.Instance.isSandbox && !_state_machine_system.IsConnectedTooExistedBattle;
        StartCoroutine(OnScreenVisibleCoroutine());
    }

    private IEnumerator OnScreenVisibleCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        ArenaViewer.SetActive(true);//show arena
        ArenaViewer.GetComponent<Animator>().enabled = (true);
        ArenaViewer.GetComponent<Animator>().Play("start");

        SwitchState(StartBattleStates.PlayCameraZoom);

        audioSource.clip = battleOpenClip;
        audioSource.Play();

        if (timer - fadeTime < minTimeToShowArena || !canShowText)
        {
            ArenaViewer.SetActive(false);
        }
    }

    private void ShowStartText()
    {
        textViewer.GetComponent<CanvasGroup>().alpha = 0;
        if (!canShowText)
            textViewer.SetActive(false);
        else
        {
            textViewer.SetActive(true);
            StartCoroutine(AddAlpha());
        }
    }

    private void SetArenaInfo()
    {
        if (Settings.Instance.Get<ArenaSettings>().RatingBattlefield((ushort)Profile.Rating.current, out BinaryBattlefields binaryArena))
        {
            ArenaName.text = Locales.Get(binaryArena.title);
            ArenaNumber.text = Locales.Get("locale:1360", arenaData.number + 1);
        }
    }

    public void SetTimeBeforeTimer(int time)
    {
        timer = (float)time / (float)1000f;
    }

    private IEnumerator AddAlpha()
    {
        var elapsTime = 0f;
        while (elapsTime < 0.34f)
        {
            var value = Mathf.Lerp(0, 1, elapsTime * 3);
            textViewer.GetComponent<CanvasGroup>().alpha = value;
            elapsTime += Time.deltaTime;
            yield return null;
        }
    }
}
