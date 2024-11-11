using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Legacy.Client.HeroParamBehaviour;

public class CardMiniWindowBehaviour : WindowBehaviour
{
    [SerializeField] private Animator WindowAnimator;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform parentContainer;
    [SerializeField] private RectTransform popUpConteiner;
    [SerializeField] private TextMeshProUGUI title;
    private HeroParamBehaviour[] Params;
    private ProfileInstance profile;
 //   private BaseBattleSettings _settings;
    /*public BinaryEntity BinaryMinion { get; private set; }
    public MinionOffence Offence { get; private set; }
    public MinionDefence Defence { get; private set; }
    public MinionSkills Skills { get; private set; }
    public MinionMovement Movement { get; private set; }
*/
    private byte level;
    private int indexUnit;
    private CardParams cardParams;
    public override void Init(Action callback)
    {
        cardParams = new CardParams();
        callback();
    }

    protected override void SelfClose()
    {
        foreach (Transform child in parentContainer)
        {
            Destroy(child.gameObject);
        }
        WindowAnimator.Play("Close");
    }

    protected override void SelfOpen()
    {
        /*foreach (Transform child in parentContainer)
        {
            Destroy(child.gameObject);
        }*/
        float _y = float.Parse(settings["y"]);
        popUpConteiner.localPosition = new Vector3(popUpConteiner.localPosition.x, _y-85, popUpConteiner.localPosition.z);
        level = Convert.ToByte(settings["level"]);
        indexUnit = Convert.ToInt32(settings["index"]);
        title.text = Locales.Get("entities:" + indexUnit + ":title");// cardParams.GetEntityName((ushort)indexUnit)); 
        gameObject.SetActive(true);
        SetParams();
    }

    public void MissClick()
    {

        WindowManager.Instance.ClosePopUp();
    }
    
    void SetParams()
    {
        float value = 0.0f;
        float dmg = 0.0f;
        string stringValue;
        bool ch = false;
       
        foreach (UnitParamType valueType in Enum.GetValues(typeof(UnitParamType)))
        {
            value = 0.0f;
            stringValue = "";
            UnitParamType _valueType = valueType;
            switch (valueType)
            {
                case UnitParamType.HP:
                    value = cardParams.SetEntetyParams((ushort)indexUnit, valueType,level);
                    break;
                case UnitParamType.DMG:

                    value = cardParams.SetEntetyParams((ushort)indexUnit, valueType, level);
                    if (value == 0)
                    {
                        _valueType = UnitParamType.Splash;
                        value = cardParams.SetEntetyParams((ushort)indexUnit, UnitParamType.Splash, level);
                    }
                    if (value == 0)
                    {
                        _valueType = UnitParamType.Piercing_DMG;
                        value = cardParams.SetEntetyParams((ushort)indexUnit, UnitParamType.Piercing_DMG, level);
                    }
                    dmg = value;
                    break;
                case UnitParamType.DPS:
                    if(dmg > 0f)
                       value = cardParams.SetEntetyParams((ushort)indexUnit, valueType, level, dmg);
                    break;
                case UnitParamType.AttackSpeed:
                    value = cardParams.SetEntetyParams((ushort)indexUnit, valueType, level);
                    break;
                case UnitParamType.AttackType:
                    stringValue = cardParams.GetAttackType((ushort)indexUnit);
                    break;
                case UnitParamType.Targets:
                    value = cardParams.SetEntetyParams((ushort)indexUnit, valueType, level);
                    break;
                case UnitParamType.Movement:
                    stringValue = cardParams.GetMovement((ushort)indexUnit);
                   

                    break;              
            }

            if (value > 0f || stringValue.Length > 0)
            {
                GameObject param = Instantiate(prefab, parentContainer);
                HeroParamBehaviour hpb = param.GetComponent<HeroParamBehaviour>();
                hpb.UpdateParamView(_valueType);
                if (value > 0)
                {
                    hpb.SetValue(value, false);
                }
                else
                {
                    hpb.SetStringValue(stringValue);
                }
                hpb.ShowBack(ch);
                ch = !ch;
              /*  RectTransform rt = hpb.GetComponent<RectTransform>();
                rt.position = new Vector3(rt.position.x- dx, rt.position.y, rt.position.z);
                dx+=5f;*/
            }

        }
        byte dx = 0;
        foreach (Transform child in parentContainer)
        {
            VerticalLayoutGroup vlg = child.GetComponent<VerticalLayoutGroup>();
            vlg.padding.left = -dx;
            vlg.padding.right = dx;
            dx +=2;
        }
    }

}
