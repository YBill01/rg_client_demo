using UnityEngine;
using UnityEditor;
using Legacy.Client;
using TMPro;
using UnityEngine.Playables;
using Unity.Entities;
using Legacy.Database;
using DG.Tweening;
using UnityEngine.UI;

public class ManaBottleBehaviour : MonoBehaviour
{
    [SerializeField]
    PlayableDirector bottleTimeline;
    [SerializeField]
    RectTransform fluid;
    [SerializeField]
    RectTransform drop;
    [SerializeField]
    TextMeshProUGUI manaText;
    [SerializeField]
    Image shadow;
    [SerializeField] BottleBlickBehaviour bottleBlick;
    [SerializeField] AudioClip etherFill;
    [SerializeField] AudioClip etherSpend;
    [SerializeField] TextMeshProUGUI etherText;

    private const float FluidTopPosition = -38;
    private const float FluidBottomPosition = -332;
    private const float FluidDelta = FluidTopPosition - FluidBottomPosition;

    bool playNow = false;
    int lastMana = 0;
    private EntityQuery battleInstanceQuery;
    private BaseManaSettings settings;
    private Sequence shaker;
    bool alreadyShowManaSpeedUpMessage = false;

    private void Start()
    {
        battleInstanceQuery = ClientWorld.Instance.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<BattleInstance>());
        settings = Settings.Instance.Get<BaseBattleSettings>().mana;
    }

    void Update()
    {
        if (battleInstanceQuery.IsEmptyIgnoreFilter)
            return;

        var battleInsance = battleInstanceQuery.GetSingleton<BattleInstance>();


        if (battleInsance.status == BattleInstanceStatus.Pause && playNow)
        {
            bottleTimeline.Stop();
            playNow = false;
            bottleBlick.Pause();
        }

        if (battleInsance.status != BattleInstanceStatus.Playing && battleInsance.status != BattleInstanceStatus.Prepare)
            return;

        bool isManaX2 = (!battleInsance.isAdditionalTime && !alreadyShowManaSpeedUpMessage && battleInsance.timer < settings.lastmin * 1000 && battleInsance.status != BattleInstanceStatus.Prepare && battleInsance.status < BattleInstanceStatus.FastKillingHeroes);
        bool isManaX3 = (alreadyShowManaSpeedUpMessage && battleInsance.isAdditionalTime && battleInsance.timer < settings.lastmin * 1000 && battleInsance.status != BattleInstanceStatus.Prepare && battleInsance.status<BattleInstanceStatus.FastKillingHeroes);
        if (isManaX2 || isManaX3)
        {
            var screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            if (!battleInsance.isAdditionalTime)//manax2
            {
                etherText.gameObject.SetActive(true);
                SetAmount("2");
                AlertsBehaviour.Instance.ShowAlertQueue(new string[2] { "locale:1114", "locale:1108" }, new int[2] { 60, 0 }, 2f);

                alreadyShowManaSpeedUpMessage = true;
            }
            if (battleInsance.isAdditionalTime)//manax3in additional time
            {
                etherText.gameObject.SetActive(true);
                SetAmount("3");
                AlertsBehaviour.Instance.ShowAlertQueue(new string[2] { "locale:1114", "locale:1111" }, new int[2] { 60, 0 }, 2f);
                alreadyShowManaSpeedUpMessage = false;
            }

        }
       if( battleInsance.status > BattleInstanceStatus.FastKillingHeroes)
        {
            AlertsBehaviour.Instance.StopAlerts();
        }

        var intMana = Mathf.FloorToInt(ManaUpdateSystem.PlayerMana);
        var partOfMana = ManaUpdateSystem.PlayerMana - intMana;

        manaText.text = intMana.ToString();

        if (battleInsance.status != BattleInstanceStatus.Prepare && !ClientWorld.Instance.Profile.IsTutorial ||
           ClientWorld.Instance.Profile.IsTutorial)
            WhenManaChanged(intMana);

        if (intMana > 9)
        {
            if (playNow)
            {
                bottleTimeline.Stop();
                bottleTimeline.time = bottleTimeline.duration;
                playNow = false;

                shaker = DOTween.Sequence();
                shaker.Append(drop.DORotate(new Vector3(0, 0, 2f), 0.1f));
                shaker.Append(drop.DORotate(new Vector3(0, 0, -2f), 0.1f));
                shaker.SetLoops(-1);
            }
            return;
        }
        else
        {
            if (!playNow && battleInsance.status != BattleInstanceStatus.Prepare)
            {
                playNow = true;
                bottleTimeline.Play();
                bottleBlick.Play();

                shaker?.Kill();
                drop.localEulerAngles = Vector3.zero;
            }
        }

        float newTime = (float)bottleTimeline.duration * partOfMana;
        var delta = Mathf.Abs((float)bottleTimeline.time - newTime);

        if (delta > 0.1f)
        {
            Debug.LogWarning("Correct");
            bottleTimeline.time = newTime;
        }
    }

    private void WhenManaChanged(int mana)
    {
        if (mana == lastMana)
        {
            return;
        }

        if (mana != 10)
            bottleBlick.Play();

        if (lastMana < mana) SetClip(etherFill);
        else SetClip(etherSpend);

        lastMana = mana;

        var pos = fluid.localPosition;
        pos.y = FluidBottomPosition + FluidDelta / 10f * mana;
        fluid.localPosition = pos;

        shadow.color = new Color(1, 1, 1, ((float)mana) / 10f);
    }
    private void SetClip(AudioClip clip)
    {
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();

    }
    protected void SetAmount(string text)
    {
        etherText.text = "<size=70%>x</size>" + LegacyHelpers.FormatByDigits(text);
    }
}