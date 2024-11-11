using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class BattleStartWindowBehaviour : WindowBehaviour
{
    private ProfileInstance profile;
    private StateMachineSystem stateMachineSystem;

	[SerializeField]
	private MainWindowBehaviour mainWindow;

	[SerializeField]
    private Image PlayerAvatar;
    [SerializeField]
    private Image EnemyAvatar;
    [SerializeField]
    private Image PlayerHeroIcon;
    [SerializeField]
    private Image EnemyHeroIcon;
    [SerializeField]
    private TextMeshProUGUI PlayerRating;
    [SerializeField]
    private TextMeshProUGUI EnemyRating;
    [SerializeField]
    private TextMeshProUGUI PlayerName;
    [SerializeField]
    private TextMeshProUGUI EnemyName;
    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private SilhuetteBehaviour SilhuatteChanger;
    private Action callback = null;

    private float windowOpenedTime;

    public Sprite GetHeroSprite(ushort index)
    {
        return VisualContent.Instance.GetHeroVisualData(index).StartBattleIcon;
    }
    public override void Init(Action callback)
    {
        profile = ClientWorld.Instance.GetExistingSystem<HomeSystems>().UserProfile;
        stateMachineSystem = ClientWorld.Instance.GetExistingSystem<StateMachineSystem>();
        callback();
        stateMachineSystem.OpponentFoundEvent.AddListener(OnOpponentFound);
    }

    public void InitPlayerData()
    {
        PlayerRating.text = LegacyHelpers.FormatByDigits(profile.Rating.current.ToString());
        PlayerName.text = profile.name;
        if (ClientWorld.Instance.Profile.IsBattleTutorial)
            PlayerName.text = Locales.Get("locale:1639");
        PlayerHeroIcon.sprite = GetHeroSprite(profile.SelectedHero);
    }
    /// <summary>
    /// Called From animator
    /// </summary>
    private bool isChack = false;
    public void EnableSilhuattes()
    {
        if (!isChack)
        {
            isChack = true;
            SilhuatteChanger.Enabled = true;
        }
    }

    public void SetEnemy(ObserverBattlePlayer enemy)
    {
        source.Play();
        EnemyName.text = Locales.Get(enemy.profile.name.ToString());
        EnemyRating.text = LegacyHelpers.FormatByDigits(enemy.profile.rating.current.ToString());
        isChack = true;
        DisableSilhuattes();
        EnemyHeroIcon.sprite = GetHeroSprite(enemy.profile.hero.index);
        GetComponent<Animator>().SetTrigger("Found");
      
        AnalyticsManager.Instance.BattleStart(enemy, (int)(Time.time - windowOpenedTime));
    }

    public void DisableSilhuattes()
    {
        SilhuatteChanger.DisableSilhuatte();
    }

    protected override void SelfClose()
    {        
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called from BattleStarWindowAnimator
    /// </summary>
    public void ShakeCamera()
    {
        WindowManager.Instance.ShakeCamera();
    }

    protected override void SelfOpen()
    {
        isChack = false;
        InitPlayerData();
        if (ClientWorld.Instance.Profile.HardTutorialState < 4)
            cancelButton.gameObject.SetActive(false);
        windowOpenedTime = Time.time;
        SoundManager.Instance.MuteMusic(true);
     //   InitPlayerData();
        gameObject.SetActive(true);
    }

    /// <summary>
    /// CalledFromAnimator BattlestartWindow on end Clip "Found"
    /// </summary>
   public void CallbackCall()
    {
        if (callback != null)
        {
            callback();
            callback = null;
        }
    }

    internal void SetEnemies(ObserverBattlePlayer[] enemies, Action callback)
    {
        StartCoroutine(WaitAnimationEnimy(enemies));//ждем начала анимации поиска. Иначе враг появится до героя
        this.callback = callback;
    }

    private IEnumerator WaitAnimationEnimy(ObserverBattlePlayer[] enemies)
    {
        while (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Searching"))
        {
              yield return new WaitForSeconds(.5f);
        }

        SetEnemy(enemies[0]);
        this.callback = callback;
    }

    internal void DelayedSetEnemies(float delay, ObserverBattlePlayer[] enemies, Action callback)
    {
        StartCoroutine(DelayedSetEnemiesCore(enemies, callback, delay));
    }

    private IEnumerator DelayedSetEnemiesCore(ObserverBattlePlayer[] enemies, Action callback, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetEnemies(enemies, callback);
    }

    /// <summary>
    /// Calles from Animator in Cancel animation Clip
    /// </summary>
	public void OnCanelAnimationDone()
	{	
        WindowManager.Instance.Back();
    }

    public LegacyButton cancelButton;
	public void OnCancelClick()
	{
		stateMachineSystem.BattleCancelEvent.AddListener(OnCancelApproved);
		NetworkMessageHelper.CancelBattle1x1();
	}

	private void OnCancelApproved()
	{
		stateMachineSystem.OpponentFoundEvent.RemoveListener(OnOpponentFound);
		stateMachineSystem.BattleCancelEvent.RemoveListener(OnCancelApproved);
        SoundManager.Instance.PlayMenuMusic();
		GetComponent<Animator>().SetTrigger("Cancel");
	}

	private void OnOpponentFound(ObserverBattlePlayer miniProfile)
	{
		cancelButton.interactable = false;
		stateMachineSystem.BattleCancelEvent.RemoveListener(OnCancelApproved);
		stateMachineSystem.OpponentFoundEvent.RemoveListener(OnOpponentFound);
    }

	private void OnDestroy()
	{
		stateMachineSystem.OpponentFoundEvent.RemoveListener(OnOpponentFound);
		stateMachineSystem.BattleCancelEvent.RemoveListener(OnCancelApproved);
	}
}
