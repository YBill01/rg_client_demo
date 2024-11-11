using Legacy.Client;
using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class HeroWindowSkillItemBehaviour : MonoBehaviour
    {
        [SerializeField] private SkillViewBehaviour view;

        [SerializeField] private TextMeshProUGUI title;

        //[SerializeField] private GameObject updateFrame;

        [SerializeField] ShortInfoSkillView shortInfo;
        [SerializeField] GameObject Lock;
        [SerializeField] GameObject paramsLayout;

        internal void Init(ushort index, PlayerProfileHero playerHero)
        {
            Lock.SetActive(playerHero == null);
            paramsLayout.SetActive(playerHero != null);
            if (Skills.Instance.Get(index, out BinarySkill binarySkill))
            {
                title.text = Locales.Get(binarySkill.GetTitle());
                view.Init(binarySkill);
                view.MakeGray(playerHero == null);

                if (playerHero == null)
                {
                    //updateFrame.SetActive(false);
                }
                else
                {
                    //updateFrame.SetActive(playerHero.level < ClientWorld.Instance.Profile.Level.level);
                    shortInfo.SetData(binarySkill, playerHero.level);
                }
            }
        }
    }
}