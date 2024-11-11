using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Legacy.Client;

namespace Legacy.Client
{
    public class HeroParamBehaviour : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Title;
        [SerializeField] private TextMeshProUGUI ValuePrefix;
        [SerializeField] private TextMeshProUGUI Value;
        [SerializeField] private TextMeshProUGUI AdditionalValue;
        [SerializeField] private TextMeshProUGUI PlusValue;
        [SerializeField] private Image Icon;

        [SerializeField] private float SlowAttackBound;
        [SerializeField] private float AttackBound;
        [SerializeField] private bool ShortTitle;
        [SerializeField] private LegacyButton InfoButton;
        [SerializeField] private Transform back;
        private UnitParamData data;

        UnitParamData GetData(UnitParamType type)
        {
            for (byte i = 0; i < data_list.Count; i++)
            {
                if (data_list[i].type == type)
                {
                    return data_list[i];
                }
            }

            return data_list[0];
        }

        [SerializeField] private List<UnitParamData> data_list;


        [Serializable]
        public struct UnitParamData
        {
            public UnitParamType type;
            public string Title;
            public string Description;
            public byte AfterCommaCount;
            public bool isProgressable;
            public Sprite Icon;
            public string ending;
            public string prefix;

            public float ProgressPercent
            {
                get
                {
                    var minionsSettings = Settings.Instance.Get<BaseBattleSettings>().minions;
                    return type == UnitParamType.HP ? minionsSettings.health : minionsSettings.damage;
                }
            }
        }


        [Serializable]
        public enum UnitParamType
        {
            HP = 0,
            DMG = 1,
            DPS = 2,
            AttackType = 3,
            AttackSpeed = 4,
            Targets = 5,
            Quantity = 6,
            AreaDamage = 7,
            Movement = 8,
            Splash = 9,
            Knockback = 10,
            Sturm_Damage = 11,
            DPS_splash = 12,
            Life_Time=13,
            Piercing_DMG=14,
            Summon=15
        }

        public UnitParamType GetType()
        {
            return data.type;
        }
        private float value = 0;
        private float plusValue = 0;
        private BinaryEntity binaryMinion;

        internal void CloseInfoButtom()
        {
            if (InfoButton)
                InfoButton.gameObject.SetActive(false);
        }
        internal void SetValue(float value, bool PlayerHas, float plus = 0.0f)
        {
            string result = "";
            switch (data.type)
            {
                case UnitParamType.Targets:
                    foreach (MinionLayerType targetType in Enum.GetValues(typeof(MinionLayerType)))
                    {
                        if (((byte)targetType & (byte)value) > 0)
                        {
                            result += " " + CardWindowDataBehaviour.GetTargetTypeLocales(targetType);
                        }
                    }

                    break;
                case UnitParamType.Summon:
                    if (Entities.Instance.Get((ushort)value, out BinaryEntity binaryMinion))
                    {
                        result += Locales.Get("entities:" + binaryMinion.index + ":title"); //Locales.Get("binaryMinion.title");
                        if (InfoButton)
                            InfoButton.gameObject.SetActive(true);
                    }
                    break;
                default:
                    result = LegacyHelpers.GetNiceValue(value, data.AfterCommaCount).ToString() + Locales.Get(data.ending);
                    break;
            }

            Value.text = result;

            if (data.prefix.Length > 0)
            {
                ValuePrefix.text = data.prefix;
                ValuePrefix.gameObject.SetActive(true);
            }

            if (data.isProgressable && PlayerHas)
            {
                if(plus>0 && plus > value)
                    SetPlusValue((plus- value));
                else
                    SetPlusValue(value * data.ProgressPercent / 100.0f);
            }
            this.value = value;
            this.plusValue = plus;
        }

        internal void SetStringValue(string value)
        {
            Value.text = value;
        }

        internal void ResetParam()
        {
            AdditionalValue.gameObject.SetActive(false);
            PlusValue.gameObject.SetActive(false);
        }

        internal void SetPlusValue(float value)
        {
            PlusValue.text = "+" + LegacyHelpers.GetNiceValue(value, data.AfterCommaCount).ToString();
            PlusValue.gameObject.SetActive(true);
        }

        internal void SetAdditionalValue(string value)
        {
            AdditionalValue.gameObject.SetActive(true);
            AdditionalValue.text = value;
        }

        internal void UpdateParamView(UnitParamType type)
        {
            data = GetData(type);
            Icon.sprite = data.Icon;
            Title.text = Locales.Get(ShortTitle? data.Title : data.Description);// data.Title;
            //Description.text = ;
        }
        internal void SetDefaultValue()
        {
            value = 0f;
            plusValue = 0f;
            PlusValue.gameObject.SetActive(false);
        }
        internal float GetLvlValue()
        {
            return value;
        }
        internal float GetPlusLvlValue()
        {
            return plusValue;
        }
        internal float GetNextLvlValue()
        {
            var difference = LegacyHelpers.GetNiceValue(value * data.ProgressPercent / 100.0f, data.AfterCommaCount);
               return Mathf.Round(value) + difference;
           // return value + difference;
        }
        internal float GetNextNextLvlValue()
        {
            var next = GetDifferenceValue();
            var difference = LegacyHelpers.GetNiceValue((value + next) * data.ProgressPercent / 100.0f, data.AfterCommaCount);
            return Mathf.Round(value+ next) + difference;
            // return value + difference;
        }
        internal float GetNextDifferenceValue()
        {
            var diff = Mathf.Round(GetNextNextLvlValue() -GetNextLvlValue());

            return diff;
        }

        internal float GetDifferenceValue()
        {
            //  var diff = LegacyHelpers.GetNiceValue(value * data.ProgressPercent / 100.0f, data.AfterCommaCount) + Mathf.Round((GetNextLvlValue() - GetLvlValue()) * data.ProgressPercent / 100.0f);
            var diff = Mathf.Round(GetNextLvlValue() - GetLvlValue());
            return diff;
        }


        public void OnInfoButtonClick()
        {
            if (WindowManager.Instance.CurrentWindow is CardWindowBehaviour)
            {
                Dictionary<string, string> settings = new Dictionary<string, string>();
                settings.Add("index", value.ToString());

                var window = WindowManager.Instance.CurrentWindow as CardWindowBehaviour;
                settings.Add("level", window.ClickedCard.GetPlayerCard().level.ToString());
               
             //   settings.Add("x",Input.mousePosition.x.ToString());
                settings.Add("y",Input.mousePosition.y.ToString());
              //  settings.Add("y",InfoButton.GetComponent<Transform>().localPosition.y.ToString());


                WindowManager.Instance.OpenWindow(WindowManager.Instance.Windows[20], settings);
            } 
        }

        public void ShowBack(bool v=true)
        {
            back.gameObject.SetActive(v);
        }
	}
}