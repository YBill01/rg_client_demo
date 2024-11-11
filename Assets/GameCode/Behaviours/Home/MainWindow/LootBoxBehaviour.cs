using System;
using Legacy.Client;
using Legacy.Database;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Legacy.Client.LootBoxWindowBehaviour;

public class LootBoxBehaviour : MonoBehaviour
{
    public enum BoxState
    {
        Empty,
        JumpToSlot,
        Closed,
        Opening,
        InQueue,
        Explosion,
        Opened,
        SlotsFull,
        GetOneLoot
    }

    private BoxState state;

    [SerializeField] GameObject LootBoxPrefab;

    public RectTransform BoxContainer;
    public RectTransform BoxTransform;

    public LootBoxViewBehaviour BoxView = null;

    [SerializeField]
    private GameObject MainBack;

    [SerializeField]
    private float ChestHeight;

    [SerializeField]
    private float TotalHeight;

    [SerializeField]
    private GameObject DownButton;
    [SerializeField]
    private GameObject ChestText;
    [SerializeField]
    private TextMeshProUGUI ClosedText;

    [SerializeField]
    private GameObject OpenText;
    [SerializeField]
    private GameObject HardText;
    [SerializeField]
    private TextMeshProUGUI HardPriceText;

    [SerializeField]
    private Sprite button_wait;
    [SerializeField]
    private Sprite button_open_now;

    [SerializeField]
    private Sprite opened_bg;
    [SerializeField]
    private Sprite regular_bg;
    [SerializeField]
    private Sprite opening_bg;
    [SerializeField]
    private Sprite opening_boost_bg;

    [SerializeField]
    private Color32 OpeningTimerBackground;
    [SerializeField]
    private Color32 OpeningTimerBoosterBackground;

    [SerializeField]
    private RectTransform rect;
    [SerializeField]
    private LayoutElement layoutElement;

    [SerializeField]
    private UITimerBehaviour Timer;
    [SerializeField]
    private Image TimerBackground;

    public BinaryLoot BinaryBox;

    [SerializeField]
    private GameObject OpenedText;

    public MainWindowBehaviour MainWindow;

    public byte number;
    public byte indexInLoots;

    private PlayerProfileLoots loots;

    public PlayerProfileLootBox PlayerBox { get { return loots.boxes[indexInLoots]; } }
    public LootSettings settings;

    public DateTime FinishTime => Timer.FinishTime;

    private PlayerProfileArenaBoosterTime arenaBoosterTime;

    internal void Init(PlayerProfileLoots loots, MainWindowBehaviour mainWindow, byte number)
    {
        MainWindow = mainWindow;

        arenaBoosterTime = ClientWorld.Instance.Profile.arenaBoosterTime;

        settings = Settings.Instance.Get<LootSettings>();
        this.loots = loots;
        this.number = number;
        indexInLoots = (byte)(number - 1);
        var box = loots.boxes[indexInLoots];
        GetComponent<LegacyButton>().localeAlert = Locales.Get($"locale:1684");

        if (box.index > 0)
        {
            if (Loots.Instance.Get(box.index, out BinaryLoot binaryBox))
            {
                BinaryBox = binaryBox;
                if (box.isOpenedForUI)
                {
                    OpenedBox();
                }
                else
                {
                    if (box.arrived)
                    {
                        if (loots.index == number)
                            OpeningBox();
                        else if (loots.nextIndex == number)
                            BoxInQueue();
                        else if (binaryBox.time == 0)// Сундук за первый туторный бой
                            OpenedBox();
                        else
                            BoxClosed();
                    }
                    else
                    {
                        if (box.index > 0)
                        {
                            BoxArriving();
                        }
                    }
                }
                InitBoxView();
            }
        }
        else
        {
            EmptyBox();
            state = BoxState.Empty;
        }

        if (!BattleDataContainer.Instance.CheckForNewArenaForChanged())
        {
            CheckBooster();
        }
    }
    public bool isJump = false;
    public void InitBoxView()
    {
        if (state != BoxState.Empty)
        {
            if (BoxView == null)
            {
                CreateBoxFromPrefab();
            }
            else
            {
                BoxView.ChangeState(state);
                BoxView.SetScaleMultiplier(.5f);
            }
        }
    }

    
    public void CheckBooster()
    {
        if (state == BoxState.Opening)
		{
            if (arenaBoosterTime.IsActive && PlayerBox.isBoostered == false)
			{
                var em = ClientWorld.Instance.EntityManager;

                var message = new NetworkMessageRaw();
                message.Write((byte)ObserverPlayerMessage.UserCommand);
                message.Write((byte)UserCommandType.LootUpdate);
                message.Write((byte)LootCommandType.Boost);
                message.Write(number);

                var messageEntity = em.CreateEntity();
                em.AddComponentData(messageEntity, message);
                em.AddComponentData(messageEntity, default(ObserverMessageTag));
            }
        }

        if (arenaBoosterTime.IsActive)
        {
            ApplyBooster();
        }
    }
    private bool isBoost = false;
    public void ApplyBooster()
    {
        isBoost = true;

        switch (state)
        {
            case BoxState.Opening:
                TimerBackground.color = OpeningTimerBoosterBackground;
                break;
            case BoxState.InQueue:
            case BoxState.Closed:
                Timer.SetStaticTime(arenaBoosterTime.GetSecondsToOpen(BinaryBox.time), true);
                MainBack.GetComponent<Image>().sprite = opening_boost_bg;
                break;
        }
    }
    public void DiscardBooster()
	{
        isBoost = false;

        switch (state)
        {
            case BoxState.Opening:
                TimerBackground.color = OpeningTimerBackground;
                break;
            case BoxState.InQueue:
            case BoxState.Closed:
                Timer.SetStaticTime(BinaryBox.time, true);
                MainBack.GetComponent<Image>().sprite = regular_bg;
                break;
        }
    }

    private void CreateBoxFromPrefab()
    {
        var loaded = Addressables.InstantiateAsync($"Loots/{BinaryBox.prefab}LootBox.prefab", BoxContainer);
        loaded.Completed += (AsyncOperationHandle<GameObject> async) =>
        {
            BoxView = async.Result.GetComponent<LootBoxViewBehaviour>();
            BoxTransform = BoxView.GetComponent<RectTransform>();
            BoxView.LootBehaviour = this;
            BoxView.Init(state, BinaryBox);
            BoxView.ChangeState(state);
        };
    }

    private void BoxArriving()
    {
        isJump = true;
         BoxClosed();
   /*     if (LootBehaviour)
        {
            LootBehaviour.Arrived();
        }*/
        state = BoxState.JumpToSlot;
        SoundsPlayerManager.Instance.PlaySound(SoundName.Chest_to_slot);
    }

    public void Arrived()
    {
        isJump = false;
        if (BinaryBox.time < 1)
        {
            ChangeState(BoxState.Opened);
        }
        else 
        {
            ChangeState(BoxState.Closed);
        }
        var em = ClientWorld.Instance.EntityManager;

        var message = new NetworkMessageRaw();
        message.Write((byte)ObserverPlayerMessage.UserCommand);
        message.Write((byte)UserCommandType.LootUpdate);
        message.Write((byte)LootCommandType.Arrive);
        message.Write(number);

        var messageEntity = em.CreateEntity();
        em.AddComponentData(messageEntity, message);
        em.AddComponentData(messageEntity, default(ObserverMessageTag));

    //    ChangeState(BoxState.Closed);
    //    var box = loots.boxes[number];
    //    box.arrived = true;
    }


    [SerializeField] LootCardType ParticlesType;
    public void Click()
    {
        switch (state)
        {
            case BoxState.Opened:
                ClickOpenedBox();
                break;
            case BoxState.Opening:
            case BoxState.InQueue:
            case BoxState.Closed:
                OpenLootPopUp();
                break;
            case BoxState.Empty:
                if (!ClientWorld.Instance.Profile.IsBattleTutorial) {
                    CloseMessage();
                    MainWindow.menuTutorialPointer.popupMessage.ShowTextAtLeftFrom(Locales.Get("locale:1162"), MainWindow.GetBattleButton, 3);
                }
                break;
            default:
                //RewardParticlesBehaviour.Instance.Drop(GetComponent<RectTransform>().position, 10, ParticlesType);
                //AnalyticsManager.Instance.RealPaymentTest();
                break;
        }
    }

    private void CloseMessage()
    {
        if(MainWindow.menuTutorialPointer != null && MainWindow.menuTutorialPointer.popupMessage != null)
             MainWindow.menuTutorialPointer.popupMessage.Hide();
    }

    private void OpenLootPopUp()
    {
        MainWindow.OpenLootPopUp(this);
    }

    public void ClickOpenedBox()
    {
        MainWindow.GetLootBox(this);
    }

    void ChangeState(BoxState newState)
    {
        if (newState == state)
            return;

        if (newState == BoxState.Empty)
        {
            EmptyBox();
        }
        else
        {
            switch (newState)
            {
                case BoxState.Opened:
                    OpenedBox();
                    break;
                case BoxState.Opening:
                    OpeningBox();
                    break;
                case BoxState.Closed:
                    BoxClosed();
                    break;
                default:
                    break;
            }

            BoxView.ChangeState(state);
        }
        state = newState;

        if (!BattleDataContainer.Instance.CheckForNewArenaForChanged())
        {
            CheckBooster();
        }
    }

    private void OpenedBox()
    {
        state = BoxState.Opened;
        layoutElement.minHeight = TotalHeight;
        MainBack.GetComponent<Image>().sprite = opened_bg;
        DownButton.SetActive(false);
        ClosedText.gameObject.SetActive(false);
        OpenedText.SetActive(true);
        Timer.gameObject.SetActive(false);
        GetComponent<LegacyButton>().interactable = true;
        GetComponent<LegacyButton>().isLocked = false;
    }

    private void BoxInQueue()
    {
        state = BoxState.InQueue;
        layoutElement.minHeight = TotalHeight;
        Timer.SetStaticTime(BinaryBox.time, true);
        MainBack.GetComponent<Image>().sprite = regular_bg;
        DownButton.SetActive(false);
        ChestText.SetActive(false);
        ClosedText.gameObject.SetActive(true);
        ClosedText.text = Locales.Get("locale:1348");
        HardText.SetActive(false);
        OpenedText.SetActive(false);
        GetComponent<LegacyButton>().interactable = true;
        GetComponent<LegacyButton>().isLocked = false;
    }

    internal void SetTimer(uint secondsToOpen)
    {
        Timer.TimerFinished.AddListener(OnTimerEnds);
        Timer.SetFinishedTime(secondsToOpen);
    }

    void OnTimerEnds()
    {
        ChangeState(BoxState.Opened);
        Timer.OnTimerUpdate.RemoveListener(UpdatePrice);
        NetworkMessageHelper.RequestForUpdatedProfile();
        GetComponent<LegacyButton>().interactable = true;
        GetComponent<LegacyButton>().isLocked = false;
    }

    private void OnDestroy()
    {
        Timer.OnTimerUpdate.RemoveListener(UpdatePrice);
        CloseMessage();
    }

    private void OpeningBox()
    {
        state = BoxState.Opening;
        layoutElement.minHeight = TotalHeight;
        ChestText.gameObject.SetActive(false);
        OpenedText.SetActive(false);
        DownButton.GetComponent<Image>().sprite = button_open_now;
        DownButton.SetActive(true);
        OpenText.SetActive(false);
        HardText.SetActive(true);
        ClosedText.gameObject.SetActive(false);
        MainBack.GetComponent<Image>().sprite = opening_bg;
        TimerBackground.color = OpeningTimerBackground;
        SetTimer(PlayerBox.secondsToOpen);
        if (ClientWorld.Instance.Profile.IsBattleTutorial)
        {
            GetComponent<LegacyButton>().interactable = false;
            GetComponent<LegacyButton>().isLocked = true;
        }
        Timer.OnTimerUpdate.AddListener(UpdatePrice);
        UpdatePrice(PlayerBox.secondsToOpen);

        // Отправка push-notification
        //PushNotifications.Instance.ChestOpeningLocalNotificationStart(PlayerBox.secondsToOpen);
    }

    private void UpdatePrice(uint secondsToOpen)
    {
        PlayerBox.secondsToOpen = secondsToOpen;
        HardPriceText.text = Loots.PriceToSkip(secondsToOpen, settings).ToString();

		if (isBoost && arenaBoosterTime.IsActive == false)
		{
			DiscardBooster();
		}
	}


    private void EmptyBox()
    {
        layoutElement.minHeight = ChestHeight;
        ChestText.gameObject.SetActive(true);
        DownButton.SetActive(false);
        OpenText.SetActive(false);
        OpenedText.SetActive(false);
        ClosedText.gameObject.SetActive(false);
        MainBack.GetComponent<Image>().sprite = regular_bg;
        Timer.gameObject.SetActive(false);
        if (BoxView != null)
        {
            BoxView.Remove();
            BoxView = null;
        }
    }

    private void BoxClosed()
    {
        state = BoxState.Closed;
        layoutElement.minHeight = TotalHeight;
        Timer.SetStaticTime(BinaryBox.time, true);
        MainBack.GetComponent<Image>().sprite = regular_bg;
        ChestText.SetActive(false);
        ClosedText.gameObject.SetActive(true);
        ClosedText.text = Locales.Get("locale:1027");
        HardText.SetActive(false);
        OpenedText.SetActive(false);
    }
}
