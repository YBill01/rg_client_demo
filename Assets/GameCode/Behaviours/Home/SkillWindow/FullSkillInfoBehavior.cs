using Legacy.Client;
using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using static SkillParametrBehavior;

public class FullSkillInfoBehavior : MonoBehaviour
{
    public bool CanUpdate//level price
    {
        get
        {
            return ClientWorld.Instance.Profile.Level.level > heroLevel;
        }
        set { }
    }

    [Header("MainData")]
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Transform parametrsParent;
    [SerializeField] private GameObject skillPrefab;
    [SerializeField] private HeroWindowSkillItemBehaviour skill;
    [Header("LeftData")]
    [SerializeField] private ButtonWithPriceViewBehaviour upgradeButton;
    [SerializeField] private GameObject upgradeEffect;
    [SerializeField] private TextMeshProUGUI freeText;
    [SerializeField] private TextMeshProUGUI levelText;

    private Dictionary<SkillParamType, string> parametrs = new Dictionary<SkillParamType, string>();
    private List<Animator> paramAnimators = new List<Animator>();
    private List<SkillParametrBehavior> tempParams = new List<SkillParametrBehavior>();
    private ushort heroLevel;
    private string titleStr;
    private string descriptionStr;

    public void SetData(string title, string description, PlayerProfileHero playerHero)
    {
        this.heroLevel = playerHero?.level ?? 1;

        titleStr = Locales.Get(title);
        descriptionStr = Locales.Get(description);
        SetSkillTitle(title);
        SetSkillDescription(descriptionStr);

        SetAllSkillParametrs();
        UpdateButtonView(CanUpdate);
        upgradeButton.gameObject.SetActive(playerHero != null);
        if (playerHero == null)
            freeText.text = "";
    }

    public void Upd(byte heroLevel)
    {
        this.heroLevel = heroLevel;

        SetSkillTitle(titleStr);
        SetSkillDescription(descriptionStr);

        foreach (var param in parametrs)
            UpdateSkillParams(new SkillParamData
            {
                type = param.Key,
                parametrValue = param.Value
            }, parametrs.ToList().IndexOf(param));
        UpdateButtonView(CanUpdate);
        parametrs.Clear();
    }
    private void SetAllSkillParametrs()
    {
        ClearPreviousSkills();
        paramAnimators.Clear();
        tempParams.Clear();
        foreach (var param in parametrs)
        {
            SetSkillParametr(new SkillParamData
            {
                type = param.Key,
                parametrValue = param.Value
            }, parametrsParent);
        }
        parametrs.Clear();

    }
    private void UpdateSkillParams(SkillParamData data, int index)
    {
        tempParams[index].SetData(data);
    }

    public SkillParametrBehavior SetSkillParametr(SkillParamData data, Transform parent)
    {
        var skill = Instantiate(skillPrefab, parent).GetComponent<SkillParametrBehavior>();
        skill.SetData(data);
        //paramAnimators.Add(skill.GetAnimator());
        tempParams.Add(skill);
        return skill;
    }

    public List<Animator> GetParams()
    {
        return paramAnimators;
    }
    private void ClearPreviousSkills()
    {
        var objsToDestroy = parametrsParent.GetComponentsInChildren<SkillParametrBehavior>().ToList();
        foreach (var objToDestroy in objsToDestroy)
        {
            Destroy(objToDestroy.gameObject);
        }
    }

    internal void AddSkillParametr(SkillParamType paramType, string value)
    {
        parametrs.Add(paramType, value);
    }

    internal Dictionary<SkillParamType, string> GetSkillParametrs()
    {
        return parametrs;
    }


    private void UpdateButtonView(bool canUpdate)
    {
        upgradeButton.GetComponent<LegacyButton>().interactable = CanUpdate && CanUpdate || !CanUpdate;
        upgradeButton.SetGrayMaterial(!CanUpdate);
        upgradeEffect.SetActive(canUpdate);
        SetTexts(CanUpdate);
    }

    private void SetSkillTitle(string title)
    {
        Regex regex = new Regex(@"(?<=\[)(.*?)(?=\])");

        MatchCollection matches = regex.Matches(title);
        foreach (Match match in matches)
        {
            this.title.text = match.Value;
        }
    }
    private void SetSkillDescription(string description)
    {
        var sentences = description.Split('.').ToList();
        sentences = sentences.Take(2).ToList();
        this.description.text = sentences.First() + "\n" + sentences.Last();
    }

    private void SetTexts(bool canUpdate)
    {
        if (canUpdate)
            freeText.text = Locales.Get("locale:1090");
        else
            freeText.text = Locales.Get("locale:892");
    }

}
