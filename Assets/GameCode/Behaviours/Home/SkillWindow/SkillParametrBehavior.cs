using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SkillParametrBehavior : MonoBehaviour
{
    [Serializable]
    public struct SkillParamData
    {
        internal SkillParamType type;
        public string parametrValue;
    }

    [SerializeField] private Image icon;
    //[SerializeField] private TextMeshProUGUI shortTitle;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI parametrValue;

    private SkillParamData skillData;

    public void SetData(SkillParamData data)
    {
        skillData.type = data.type;
        skillData.parametrValue = data.parametrValue;
        ViewData(data);
    }

    public void ViewData(SkillParamData data)
    {
        var staticData = SkillParamsConfig.Instance.GetSkillParamByType(data.type);

        //if (shortTitle) shortTitle.text = staticData.ShortTitle;
        if (title) title.text = Locales.Get(staticData.FullTitle);
        if (icon) icon.sprite = staticData.icon;
        //if (helpButton) helpButton.SetActive(staticData.needDelails);
        if (parametrValue) parametrValue.text = skillData.parametrValue + " " + Locales.Get(staticData.additionalTailToValue);
    }
    
    public float PrevValue()
    {
        Regex regex = new Regex(@"\w*");

        MatchCollection matches = regex.Matches(parametrValue.text);
        foreach (Match match in matches)
        {
            float prevValue;
            var succes = float.TryParse(match.Value, out prevValue);
            return prevValue;
        }
        return 0;
    }

    public float NextValue()
    {
        float nextValue;
        var succes = float.TryParse(skillData.parametrValue, out nextValue);
        return nextValue;
    }

    public string GetStrAddData()
    {
        var staticData = SkillParamsConfig.Instance.GetSkillParamByType(skillData.type);
        return Locales.Get(staticData.additionalTailToValue);
    }

}
