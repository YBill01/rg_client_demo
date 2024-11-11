using DG.Tweening;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Legacy.Client {
    public class NextRewardBehaviour : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Transform content;
        [SerializeField] private Transform view;
        [SerializeField] private Transform titleTransform;
        [SerializeField] private Image img;

        private ProfileInstance profile;
        private LootBoxViewBehaviour boxView;
        private NextRewardData nextRewardData;
        [Header("Animation")]
        [SerializeField, Range(0.0f, 1.0f)] private float ScaleSpeesUp;
        [SerializeField, Range(0.0f, 1.0f)] private float ScaleSpeesDown;
        [SerializeField] private Ease vUpType;
        [SerializeField] private Ease vDownType;
        [SerializeField] private Vector3 vUp;
        [SerializeField] private Vector3 vDown;
        [SerializeField] private Vector3 miniPosition;


        [SerializeField] private Image imgView;
        [SerializeField] private Sprite backGroundmini;
        [SerializeField] private Sprite backGroundBig;
        [SerializeField] private Sprite backGroundBigGold;

        //    [SerializeField] private bool isHideTitle=false;
        private bool _isShow = false;
        private bool _isHide = true;

        private bool _isVisible = true;
        private ushort _index = 0;
        private bool anim = false;
        public void SetVisible(bool v)
        {
            _isShow = false;
        }

        private void Start()
        {
            profile = ClientWorld.Instance.Profile;
            nextRewardData = GetComponent<NextRewardData>();
            OnVisible(false);
            if (nextRewardData)
            {
                nextRewardData.Init();
            }
        }

        private void OnShow(bool v)
        {
            if (v)
            {
                nextRewardData.GetShow();
            }
        }


        public void OnVisible(bool v=true)
        {
            if (v )
            {
                OnShow(v);
                //view.gameObject.SetActive(v);
            }
            else
            {
                view.gameObject.SetActive(false);
                /*  if (anim)
                  {
                      StopCoroutine(WaitAnim());
                  }*/
               // isShow();
            }
        }


        private Sequence sequence;
        public void isShow()
        {
           // anim = false;
      
         //  _isShow = v;
            if (_bigSize)
            {

                titleTransform.gameObject.SetActive(true);
                if (view.localScale != vUp)
                    view.DOScale(vUp, ScaleSpeesUp).SetEase(vUpType).OnComplete(() => {});
            }
            else        
            {
                titleTransform.gameObject.SetActive(false);
                if (view.localScale != vDown)
                  view.DOScale(vDown, ScaleSpeesDown).SetEase(vDownType).OnComplete(()=> {
                      imgView.sprite = backGroundmini;
                      content.transform.localPosition = miniPosition; 
                    /*  RectTransform rect = content.GetComponent<RectTransform>();
                      rect.position.x
                      imgView.transform.position.x
                      if (rect)
                      {
                          Debug.Log("asd");
                      }*/
                  });
            }
        }
        private bool _isAllBig = false;
        private bool _isOpenArena = false;
        public void ShowReward(ushort index, bool isArena = false,bool allBig=false)
        {
           // title.text = Locales.Get("locale:6138");
            title.text = Locales.Get("locale:6320");
            if (!allBig)
            {
                title.text = Locales.Get("locale:6313");
                imgView.sprite = backGroundBig;
            }
            else
            {
                _isOpenArena = false;
                imgView.sprite = backGroundBigGold;
            }
            if(isArena)
                title.text = Locales.Get("locale:6306");
            _isAllBig = allBig;
            if (_index == 0 || index != _index)
            {
                _bigSize = true;
                _index = index;
                _isOpenArena = false;
                InitReward(index, isArena);
            }
            else
                _bigSize = false;

            if(_isAllBig && !_isOpenArena)
                _bigSize = true;
            view.gameObject.SetActive(true);
            view.localScale = Vector3.zero;
            isShow();
            // StartCoroutine(WaitAnim());
        }

         
        public void OnArenaWindow()
        {
            _isOpenArena = true;
        }
      /*  IEnumerator WaitAnim()
        {
            anim = true;
            yield return new WaitForSeconds(0f);
            //OnVisible(true);
           
        }*/
        private bool _bigSize = true;
        public void InitReward(ushort index, bool isArena)
        {
            img.enabled = false;
       /*     if (boxView)
            {
                Destroy(boxView.gameObject);
            }*/
            if (isArena)
            {
                //index++;
                img.enabled = true;
                img.sprite= VisualContent.Instance.GetArenaVisualData(index).TitleImage;
            }
            else
            if (Rewards.Instance.Get(index, out BinaryReward binaryReward))
            {
                if (binaryReward.cards.Count > 0)
                {
                    img.enabled = true;
                    img.sprite = VisualContent.Instance.GetMiniCardIconRarity(binaryReward.cards[0].rarity_card.rarity);
                }
                else if (binaryReward.soft > 0)
                {
                    img.enabled = true;
                    img.sprite = VisualContent.Instance.GetCurrencyIcon(CurrencyType.Soft);

                }
                else if (binaryReward.hard > 0)
                {
                    img.enabled = true;
                    img.sprite = VisualContent.Instance.GetCurrencyIcon(CurrencyType.Hard);

                }

                else if (binaryReward.lootbox > 0)
                {
                    if (Loots.Instance.Get(binaryReward.lootbox, out loot))
                    {
                        img.enabled = true;
                        img.sprite = VisualContent.Instance.GetLootBoxIcon(loot.prefab);
                    }
                      //  InitLootBox(binaryReward.lootbox, onLootBoxLoad);
                }

            }
        }

        private void onLootBoxLoad()
        {
            boxView.DefaultScaleMultiplier = .18f;
            boxView.ResetScale();
            boxView.GetComponent<UnityEngine.Rendering.SortingGroup>().enabled=true;

        }


        private BinaryLoot loot;
        //old variant  = до иконок лутт боксов
     /*   internal void InitLootBox(ushort lootbox, Action onLoadCompleet = null)
        {
            if (Loots.Instance.Get(lootbox, out loot))
            {//
                SetChest(loot.prefab, onLoadCompleet);
            }
        }*/

    /*    private void SetChest(string prefab, Action onLoadCompleet = null)
        {
            var loaded = Addressables.InstantiateAsync($"Loots/{prefab}LootBox.prefab", content);
            loaded.Completed += (AsyncOperationHandle<GameObject> async) =>
            {
                boxView = async.Result.GetComponent<LootBoxViewBehaviour>();
                boxView.Init(LootBoxBehaviour.BoxState.SlotsFull, loot);
                boxView.SetScaleMultiplier(.7f);
                if (onLoadCompleet != null)
                    onLoadCompleet();
            };
        }*/
        public void OnClick()
        {
            WindowManager.Instance.MainWindow.Arena();
         //_isShow = !_isShow;
         //   isShow(_isShow);
       }
    }
}