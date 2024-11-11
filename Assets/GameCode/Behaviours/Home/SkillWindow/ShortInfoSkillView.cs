using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using static SkillParametrBehavior;

public class ShortInfoSkillView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI CoolDownText;
    [Space(10)]
    [SerializeField] private GameObject parametersLayout;
    [SerializeField] private GameObject SkillParameterPrefab;

    private BinarySkill binarySkill;
    private byte level;

    private Dictionary<SkillParamType, string> parameters = new Dictionary<SkillParamType, string>();

    public void SetData(BinarySkill binarySkill, byte level)
    {
        this.level = level;
        this.binarySkill = binarySkill;
        ClearPreviousSkills();
        SetSkillTitle();
        SetParams();
    }

    private void SetParams()
    {
        parameters = HeroSkillsParamConfig.Instance.GetSkillParamValues(binarySkill, level);

        foreach (var param in parameters)
        {
            SetSkillParametr(new SkillParamData
            {
                type = param.Key,
                parametrValue = param.Value
            }, parametersLayout.transform as RectTransform);
        }
    }

    public void SetSkillParametr(SkillParamData data, RectTransform parent)
    {
        var skill = Instantiate(SkillParameterPrefab, parent).GetComponent<SkillParametrBehavior>();
        skill.SetData(data);
    }

    private void ClearPreviousSkills()
    {
        var objsToDestroy = parametersLayout.GetComponentsInChildren<SkillParametrBehavior>().ToList();
        foreach (var objToDestroy in objsToDestroy)
        {
            Destroy(objToDestroy.gameObject);
        }
    }

    private void AddSkillParametr(SkillParamType key, string value)
    {
        parameters.Add(key, value);
    }

    private void SetSkillTitle()
    {
        Regex regex = new Regex(@"(?<=\[)(.*?)(?=\])");

        MatchCollection matches = regex.Matches(binarySkill.title);
        foreach (Match match in matches)
        {
            title.text = Locales.Get(match.Value);
        }
    }
}
    
