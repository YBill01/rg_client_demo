using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class MainMenuArenaChangeBehaviour : MonoBehaviour
{
    public static MainMenuArenaChangeBehaviour Instance { get; internal set; }

    public static event Action ChangeArenaDone;

    [SerializeField]
	private MainWindowBehaviour MainWindow;

    [SerializeField]
	private RectTransform ArenaChangeEffect;

    [SerializeField]
    [Range(0f, 8f)]
    private float delayStartChangeEffect;

    [SerializeField]
    private RectTransform ArenaContainer;
    //[SerializeField]
    //private RectTransform ArenaTempContainer;

    [Space(10)]
    [SerializeField]
    private RectTransform ArenaHero;
    //[SerializeField]
    //private RectTransform ArenaHeroContainer;

    [Space(10)]
    [SerializeField]
    private RectTransform ArenaNameText;

    [Space(10)]
    [SerializeField, Range(0.0f, 3.0f)]
    private float backgroundChangeTime;

    [Header("Raycaster control objects")]
    [SerializeField]
    private RectTransform[] RaycasterObjects;

    [Header("Masks containers")]
    [SerializeField]
    private RectTransform TransMaskFromLeft;
    [SerializeField]
    private RectTransform MaskFromLeft;
    [SerializeField]
    private RectTransform TransMaskFromRight;
    [SerializeField]
    private RectTransform MaskFromRight;
    [SerializeField]
    private RectTransform TransMaskToLeft;
    [SerializeField]
    private RectTransform MaskToLeft;
    [SerializeField]
    private RectTransform TransMaskToRight;
    [SerializeField]
    private RectTransform MaskToRight;

    [Header("Masks data")]
    [SerializeField]
    private List<MasksData> MasksDataList;

    //public bool IsTutorial;

    [System.Serializable]
    public struct MasksData
    {
        public ushort ArenaId;

        [Header("Orientation masks")]
        public Vector2 TransMaskLeftPosition;
        public Vector3 TransMaskLeftScale;
        public Vector2 MaskLeftPosition;
        public Vector3 MaskLeftScale;
        public Vector2 TransMaskRightPosition;
        public Vector3 TransMaskRightScale;
        public Vector2 MaskRightPosition;
        public Vector3 MaskRightScale;

        [Header("Masks content")]
        public Sprite TransMaskLeftTexture;
        public Material TransMaskLeftMaterial;
        public Sprite TransMaskRightTexture;
        public Material TransMaskRightMaterial;
    }

    public MasksData GetMasksData(ushort arenaId)
    {
        foreach (MasksData masksData in MasksDataList)
        {
            if (masksData.ArenaId != arenaId) continue;
            return masksData;
        }
        return MasksDataList[0];
    }

    private MenuArenaBehaviour prevArena;
    private MenuArenaBehaviour nextArena;

    private bool IsForced = false; // for tested...

    void Start()
    {
        Instance = this;
    }

    public void Enable()
    {
        if ((!ClientWorld.Instance.Profile.IsBattleTutorial && BattleDataContainer.Instance.CheckForNewArenaForChanged()) || IsForced)
		{
            foreach (RectTransform item in RaycasterObjects)
			{
                item.gameObject.GetComponent<GraphicRaycaster>().enabled = false;
			}
           
            MenuHeroesBehaviour.Instance.PlayIdle(ClientWorld.Instance.Profile.SelectedHero, 10.0f);

            BattlefieldInfo activeArenaInfo = MenuArenasBehaviour.Instance.GetActiveArenaInfo();

            ArenaNameText.gameObject.GetComponent<TMP_Text>().text = Locales.Get(activeArenaInfo.binary.title);

            ushort prevArenaId = activeArenaInfo.binary.index == 1 ? MenuArenasBehaviour.TUTORIAL_ARENA_ID : (ushort)(activeArenaInfo.binary.index - 1);

            prevArena = MenuArenasBehaviour.Instance.CreateArenaModel(prevArenaId, false);
            prevArena.gameObject.transform.SetParent(ArenaContainer);
            prevArena.Enable(true);

            nextArena = MenuArenasBehaviour.Instance.GetActiveArena();
            nextArena.gameObject.SetActive(false);
            
            PlayableDirector playableDirector = ArenaChangeEffect.gameObject.GetComponentInChildren<PlayableDirector>();

            var outputs = playableDirector.playableAsset.outputs;
			foreach (var item in outputs)
			{
				switch (item.streamName)
				{
                    case "PrevArena":
						{
                            playableDirector.SetGenericBinding(item.sourceObject, prevArena.gameObject);
						}
                        break;
                    case "NextArena":
						{
                            playableDirector.SetGenericBinding(item.sourceObject, nextArena.gameObject);
						}
                        break;
                }
            }

            MasksData masksDataFrom = GetMasksData(prevArenaId);
            MasksData masksDataTo = GetMasksData(activeArenaInfo.binary.index);

            TransMaskFromLeft.GetComponent<ParticleSystem>().GetComponent<Renderer>().material = masksDataFrom.TransMaskLeftMaterial;
            MaskFromLeft.GetComponent<SpriteMask>().sprite = masksDataFrom.TransMaskLeftTexture;
            TransMaskFromRight.GetComponent<ParticleSystem>().GetComponent<Renderer>().material = masksDataFrom.TransMaskRightMaterial;
            MaskFromRight.GetComponent<SpriteMask>().sprite = masksDataFrom.TransMaskRightTexture;

            TransMaskFromLeft.anchoredPosition = masksDataFrom.TransMaskLeftPosition;
            TransMaskFromLeft.localScale = masksDataFrom.TransMaskLeftScale;
            TransMaskFromRight.anchoredPosition = masksDataFrom.TransMaskRightPosition;
            TransMaskFromRight.localScale = masksDataFrom.TransMaskRightScale;
            MaskFromLeft.anchoredPosition = masksDataFrom.MaskLeftPosition;
            MaskFromLeft.localScale = masksDataFrom.MaskLeftScale;
            MaskFromRight.anchoredPosition = masksDataFrom.MaskRightPosition;
            MaskFromRight.localScale = masksDataFrom.MaskRightScale;


            TransMaskToLeft.GetComponent<ParticleSystem>().GetComponent<Renderer>().material = masksDataTo.TransMaskLeftMaterial;
            MaskToLeft.GetComponent<SpriteMask>().sprite = masksDataTo.TransMaskLeftTexture;
            TransMaskToRight.GetComponent<ParticleSystem>().GetComponent<Renderer>().material = masksDataTo.TransMaskRightMaterial;
            MaskToRight.GetComponent<SpriteMask>().sprite = masksDataTo.TransMaskRightTexture;

            TransMaskToLeft.anchoredPosition = masksDataTo.TransMaskLeftPosition;
            TransMaskToLeft.localScale = masksDataTo.TransMaskLeftScale;
            TransMaskToRight.anchoredPosition = masksDataTo.TransMaskRightPosition;
            TransMaskToRight.localScale = masksDataTo.TransMaskRightScale;
            MaskToLeft.anchoredPosition = masksDataTo.MaskLeftPosition;
            MaskToLeft.localScale = masksDataTo.MaskLeftScale;
            MaskToRight.anchoredPosition = masksDataTo.MaskRightPosition;
            MaskToRight.localScale = masksDataTo.MaskRightScale;

            //HomeTutorialHelper.Instance.IsLockTutorialStart = true;

			if ((ClientWorld.Instance.Profile.HardTutorialState == Tutorial.Instance.TotalCount() && ClientWorld.Instance.Profile.battleStatistic.battles == 0) || IsForced)
			{
                WindowManager.Instance.MainWindow.GetBattlePassButton.gameObject.SetActive(true);
                WindowManager.Instance.MainWindow.GetLeftContainer.SetActive(false);
            }

            playableDirector.stopped += OnOPlayableDirectorStop;

            StartCoroutine(ActivateChange());
        }
    }

    IEnumerator ActivateChange()
    {
    //    yield return new WaitForSeconds(delayStartChangeEffect);

        BattleDataContainer.Instance.NewArenaForChangedShown();

        ArenaChangeEffect.gameObject.SetActive(true);
        yield return null;
    }

    public void OnEnableBgSignal()
	{
        MenuHeroesBehaviour.Instance.PlayHelloIdle(ClientWorld.Instance.Profile.SelectedHero);

        MainBGBehaviour.Instance.SetChangeTime(backgroundChangeTime);
        nextArena.Enable(true);
        MainBGBehaviour.Instance.ResetChangeTime();
    }

    public void OnEnableChangeLootboxSignal()
    {
        List<LootBoxBehaviour> boxes = MainWindow.GetLoots.GetBoxes();
        foreach (LootBoxBehaviour box in boxes)
        {
            box.CheckBooster();
        }
    }

	private void Update() // for tested...
	{
		/*if (Input.GetKeyDown(KeyCode.A))
		{
			HomeTutorialHelper.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.EnterName);
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			HomeTutorialHelper.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.AfterTutorBattle4);
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			HomeTutorialHelper.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.GoIntoBattle, true);
		}*/


		//if (IsTutorial)
		//{
            //WindowManager.Instance.MainWindow.GetBattlePassButton.SetCollectEffect(false);
            //WindowManager.Instance.MainWindow.GetBattlePassButton.gameObject.SetActive(false);
        //}

		/*if (Input.GetKeyDown(KeyCode.Keypad0))
		{
			//ClientWorld.Instance.Profile.arenaBoosterTime.ApplyNewTime();
			ClientWorld.Instance.Profile.arenaBoosterTime.BoosterEndTime = ClientWorld.Instance.Profile.arenaBoosterTime.BoosterEndTime.AddSeconds(10 * 60).ToUniversalTime();

			Debug.Log(ClientWorld.Instance.Profile.arenaBoosterTime.BoosterEndTime);
		}
		if (Input.GetKeyDown(KeyCode.Keypad1))
		{
			Debug.Log(ClientWorld.Instance.Profile.arenaBoosterTime.BoosterEndTime);
		}

		if (Input.GetKeyDown(KeyCode.Keypad2))
		{

		}
		if (Input.GetKeyDown(KeyCode.Keypad3))
		{

		}

		if (Input.GetKeyDown(KeyCode.Keypad4))
		{

		}
		if (Input.GetKeyDown(KeyCode.Keypad5))
		{

		}*/
	}

	public void OnEnableInteractiveSignal()
    {
        OnEnableInteractive();
    }

    /*IEnumerator OnEnableInteractiveWait()
    {
        yield return new WaitForSeconds(4.0f);

        OnEnableInteractive();
    }*/
    public void OnEnableInteractive()
    {
        foreach (RectTransform item in RaycasterObjects)
        {
            item.gameObject.GetComponent<GraphicRaycaster>().enabled = true;
        }

        ChangeArenaDone?.Invoke();
    }

    private void OnOPlayableDirectorStop(PlayableDirector playableDirector)
    {
        nextArena.gameObject.SetActive(true);
        WindowManager.Instance.MainWindow.GetLeftContainer.SetActive(true);
        Destroy(prevArena.gameObject);
        prevArena = null;

        ArenaChangeEffect.gameObject.SetActive(false);

        playableDirector.stopped -= OnOPlayableDirectorStop;

        //HomeTutorialHelper.Instance.IsLockTutorialStart = false;
        //HomeTutorialHelper.Instance.OnTutorialStart();

        WindowManager.Instance.MainWindow.Arena();
        if (!ClientWorld.Instance.Profile.HasSoftTutorialState(SoftTutorial.SoftTutorialState.OpenArena))
            SoftTutorialManager.Instance.MenuTutorialPointer.ShowHeroMessage();
    }
}