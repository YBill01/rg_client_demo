using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class ArenaRatingEventBehaviour : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI RatingText;
        [SerializeField] TextMeshProUGUI PlayerName;
        [SerializeField] Image AvatarImage;
        [SerializeField] private GameObject RatingElementalLayout;

        public ushort rating;

        public struct RatingEventData
        {
            public ushort rating;
            public string name;
            public Sprite avatar;
        }

        private List<RatingEventData> players = new List<RatingEventData>();

        public void AddPlayer(RatingEventData data)
        {
            if(players.Count < 1)
            {
                rating = data.rating;
            }
            players.Add(data);
            UpdateEvent();
        }

        void SetData()
        {
            RatingText.text = players[0].rating.ToString();
            //PlayerName.text = players[0].name;
            //AvatarImage.sprite = players[0].avatar;
        }

        private void UpdateEvent()
        {
            if (players.Count > 1)
            {
                //TODO: Multiple Scenario
            }
            SetData();
        }

        [SerializeField, Range(0.0f, 1.0f)] float slideToPositionSpeed = 0.5f;

        private Vector2 CurrentAnchoredPosition;

        private RectTransform rect;

        public void Show(bool isShown)
        {
            RatingElementalLayout.SetActive(isShown);
        }

        public void SetPosition(float x)
        {
            CurrentAnchoredPosition.x = x;
        }

        void Update()
        {
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, CurrentAnchoredPosition, slideToPositionSpeed);
        }

        void Start()
        {
            rect = GetComponent<RectTransform>();
            CurrentAnchoredPosition = rect.anchoredPosition;
        }
    }
}
