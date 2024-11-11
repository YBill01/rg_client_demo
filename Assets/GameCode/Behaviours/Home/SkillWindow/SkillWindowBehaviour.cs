using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static SkillParametrBehavior;

public class SkillWindowBehaviour : WindowBehaviour
{
    public UnityEvent updateSkill;

    private BinaryHero BinaryHero;
    private MinionSkills HeroSkills;
    private BinaryEntity BinaryMinion;
    private PlayerProfileHero PlayerHero = null;
    private BaseBattleSettings settings;

    private ushort chosenHero;
    [SerializeField]
    private Animator WindowAnimator;
    public ProfileInstance Profile { get; private set; }

    [SerializeField] private TextMeshProUGUI mainTitle;

    [SerializeField] private HeroWindowSkillItemBehaviour skill1;

    [SerializeField] private HeroWindowSkillItemBehaviour skill2;

    [SerializeField] private List<FullSkillInfoBehavior> fullSkills;

    private float? damage = null;
    private float? description = null;
    private float? title = null;
    private float? cooldown = null;
    private float? duration = null;
    private float? lvl = null;
    private void SetSkills()
    {
        skill1.Init(HeroSkills.skill1, PlayerHero);
        skill2.Init(HeroSkills.skill2, PlayerHero);

        skill1.GetComponent<Canvas>().sortingLayerName = "PopUpWindows";
        skill2.GetComponent<Canvas>().sortingLayerName = "PopUpWindows";
    }
    public override void Init(Action callback)
    {
        settings = Settings.Instance.Get<BaseBattleSettings>();
        Profile = ClientWorld.Instance.Profile;
        callback();
        Profile.PlayerProfileUpdated.AddListener(UpdateAll);
    }

    void InitData(ushort hero_index)
    {
        BinaryHero = default;
        PlayerHero = null;
        HeroSkills = default;

        if (Heroes.Instance.Get(hero_index, out BinaryHero binaryHero))
        {
            BinaryHero = binaryHero;
            if (Profile.GetPlayerHero(hero_index, out PlayerProfileHero _hero))
            {
                PlayerHero = _hero;
            }
            if (Entities.Instance.Get(BinaryHero.minion, out BinaryEntity binaryMinion))
            {
                BinaryMinion = binaryMinion;

                if (Components.Instance.Get<MinionSkills>()
                    .TryGetValue(BinaryMinion.index, out MinionSkills skills))
                {
                    HeroSkills = skills;
                }
            }
        }
        mainTitle.text = BinaryHero.GetTitle();
    }

    private void UpdateAll()
    {
        InitData(BinaryHero.index);
        SetSkills();
        SkillUpdAdditionalData();
    }

    protected override void SelfOpen()
    {
        chosenHero = Profile.SelectedHero;
        if (parent.GetType() == typeof(HeroWindowBehaviour))
        {
            chosenHero = (parent as HeroWindowBehaviour).GetCurrentHero();
        }
        InitData(chosenHero);
        BuildWindow();
        gameObject.SetActive(true);
    }

    private void BuildWindow()
    {
        SetSkills();
        SkillAdditionalData();
    }

    public void UpdateSkill1()
    {
        if (fullSkills[0].CanUpdate)
        {
            //skill1.ClickSkill();
        }
        else
        {
            WindowManager.Instance.Home();
            WindowManager.Instance.MainWindow.OpenShopWithSection(RedirectMenuSection.BankLoots);
        }
    }
    public void UpdateSkill2()
    {
        if (fullSkills[1].CanUpdate)
        {
            //skill2.ClickSkill();
        }
        else
        {
            WindowManager.Instance.Home();
            WindowManager.Instance.MainWindow.OpenShopWithSection(RedirectMenuSection.BankLoots);
        }
    }

    public void SkillAdditionalData()
    {
        for (byte k = 0; k < MinionSkills.Count; ++k)//count-1
        {
            if (HeroSkills.Get(k, out ushort skill))
            {
                if (Skills.Instance.Get(skill, out BinarySkill bSkill))
                {
                    AddSkillParams(fullSkills[k], bSkill, PlayerHero.level);

					fullSkills[k].SetData(bSkill.title, bSkill.description, PlayerHero);
                }
            }
        }
    }

    public void SkillUpdAdditionalData()
    {
        for (byte k = 0; k < MinionSkills.Count; ++k)//count-1
        {
            if (HeroSkills.Get(k, out ushort skill))
            {
                if (Skills.Instance.Get(skill, out BinarySkill bSkill))
                {
                    AddSkillParams(fullSkills[k], bSkill, PlayerHero?.level ?? 1);

                    fullSkills[k].Upd(PlayerHero?.level ?? 1);
                }
            }
        }
    }

    private void AddSkillParams(FullSkillInfoBehavior skillInfoBehavior, BinarySkill bSkill, byte lvl)
    {
        var skillParams = HeroSkillsParamConfig.Instance.GetSkillParamValues(bSkill, lvl);

        foreach (var skill in skillParams)
        {
            skillInfoBehavior.AddSkillParametr(skill.Key, skill.Value);
        }
    }

    protected override void SelfClose()
    {
        WindowAnimator.Play("Close");
    }


    /// <summary>
    /// Called from animator Event
    /// </summary>
    public void ClosedAnimationFinish()
    {
        gameObject.SetActive(false);
    }

    public void MissClick()
    {
        WindowManager.Instance.ClosePopUp();
    }

}
