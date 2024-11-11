using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Legacy.Database;
using TMPro;

namespace Legacy.Client
{
    public class AfterUpdateVersionRewardWindowBeh : MonoBehaviour
    {
        [SerializeField] private float _showUpLasting = 0.5f;
        [SerializeField] private RectTransform buttonPos;
        [SerializeField] private TMP_Text rewardCountText;

        private RectTransform _rect;

        private void Start()
        {
            GetComponent<Canvas>().sortingLayerName = "PopUpWindows";
            _rect = GetComponent<RectTransform>();
            var startScale = _rect.localScale;
            _rect.localScale = Vector3.zero;
            _rect.DOScale(startScale, _showUpLasting).SetEase(Ease.OutQuart);
            var _gameSetting = Settings.Instance.Get<BaseGameSettings>();
            rewardCountText.text += _gameSetting.appVersionUpdateReward.ToString();
        }

        public void ReceiveReward()
        {
            RewardParticlesBehaviour.Instance.Drop(buttonPos.position, 10, LootBoxWindowBehaviour.LootCardType.Hard);
            ClientWorld.Instance.Profile.UpdatePlayerAppVersion();
            CloseWindow();
        }

        private void CloseWindow()
        {
            _rect.DOScale(Vector3.zero, _showUpLasting).SetEase(Ease.InQuart).OnComplete(() => Destroy(gameObject));
        }
    }
}