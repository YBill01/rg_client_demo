using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class DaylicsWindowBehaviour : WindowBehaviour
    {
        ProfileInstance Profile;

        [SerializeField] MainGiftBehaviour MainGift;
        [SerializeField] TimeGiftBehaviour TimeGift;

        [SerializeField] QuestBehaviour Quest1;
        [SerializeField] QuestBehaviour Quest2;
        [SerializeField] QuestBehaviour Quest3;
        [SerializeField] QuestBehaviour HeroQuest;

        [SerializeField] GameObject QuestPrefab;
        [SerializeField] GameObject HeroQuestPrefab;

        public override void Init(Action callback)
        {
            Profile = ClientWorld.Instance.Profile;
            callback();
        }

        protected override void SelfClose()
        {
            gameObject.SetActive(false);
        }

        protected override void SelfOpen()
        {
            InitData();
            gameObject.SetActive(true);
        }

        public void InitData()
        {

        }
    }
}
