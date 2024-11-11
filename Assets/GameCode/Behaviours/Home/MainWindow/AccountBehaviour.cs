using Legacy.Client;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountBehaviour : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerName;
    [SerializeField]
    private Image clanIcon;

    public ArenaButtonBehaviour ArenaButton;

    [SerializeField]
    private TextMeshProUGUI levelText;

    [SerializeField] FadeElementBehaviour AccountFade;
    [SerializeField] FadeElementBehaviour ArenaFade;
    [SerializeField] NextRewardBehaviour NexRewardFade;

    private ProfileInstance profile;
    private byte _userLevel=0;


    public void NextLevel()
    {
        _userLevel = profile.Level.level;
    }
    public void Init()
    {
        if (profile == null)
            profile = ClientWorld.Instance.Profile;
        if (_userLevel == 0) NextLevel();
        levelText.text = _userLevel.ToString();
        SetName();
        profile.NameUpdateEvent.AddListener(SetName);

        RewardParticlesBehaviour.Instance.OnParticleCame.AddListener(ParticlesCame);
    }

    private void ParticlesCame()
    {
        if (_userLevel > 0 && _userLevel < profile.Level.level ||(levelText.text!= profile.Level.level.ToString()))
        {
            WindowManager.Instance.IsCliCkBack = false;
            NextLevel();
            WindowManager.Instance.LevelUpPlayerWindow();
            levelText.text = profile.Level.level.ToString();
        }
        
    }


    void SetName()
    {
        playerName.text = profile.name;
    }

    internal void EnableNeReward(bool v)
    {
        if(profile==null)
            profile = ClientWorld.Instance.Profile;
        if (profile.IsBattleTutorial)
        {
            NexRewardFade.OnVisible(false);
        }
        else
        {
            NexRewardFade.OnVisible(v);
        }
    }
    internal void Enable(bool v)
    {
        AccountFade.Enable(v);
        ArenaFade.Enable(v);
        //  NexRewardFade.Enable(v);
        
    }
}
