using Legacy.Database;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using static LootBoxBehaviour;

namespace Legacy.Client {
    public class LootBoxViewBehaviour : MonoBehaviour
    {

        [SerializeField] public SkeletonAnimation Back;
        [SerializeField] SkeletonAnimation Front;

        [SerializeField] PlayableDirector ExplosionDirector;
        [SerializeField] PlayableDirector JumpToSlotDirector;
        [SerializeField] PlayableDirector GetOneLootDirector;
        [SerializeField] ParticleSystem OneLootAppearParticleSystem;

        [SerializeField] ParticleSystem OpenedEffect;
        [SerializeField] ParticleSystem OpeningEffect;
        [SerializeField] ParticleSystem ClosedEffect;
        [SerializeField] GameObject JumpToSlotContainer;
        [SerializeField] GameObject ExplosionContainer;

        public LootBoxBehaviour LootBehaviour;

        public BinaryLoot BinaryData;

        private BoxState state = BoxState.Empty;
        public SortingGroup SortBox;

        private RectScaleToBehaviour BoxScaler;

        public RectPositionToBehaviour BoxPositionBehaviour;

        public Action explosionCallback;
        public LootCardBehaviour currentLootCard;
        public Action getOneLootCallback;

        public void SetPopUpLayer(bool value)
        {
            SortBox.enabled = value;
        }        

        public void ResetScale()
        {
            BoxScaler.SetScaleMultiplier(DefaultScaleMultiplier);
        }

        public float DefaultScaleMultiplier = 0.5f;

        public Action JumpCallback;

        public void GetOneLootEffect(LootCardBehaviour currentLootCard, Action callback = null)
        {
            state = BoxState.GetOneLoot;
            this.currentLootCard = currentLootCard;
            getOneLootCallback = callback;
            ChangeBoxState();
        }

        public void ChangeState(BoxState state, Action callback = null)
        {
            if (this.state == state) return;
            this.state = state;
            ChangeBoxState();
            if (callback != null)
            {
                if (state == BoxState.Explosion)
                {
                    explosionCallback = callback;
                }
                else if(state == BoxState.JumpToSlot)
                {
                    JumpCallback = callback;
                }
            }
        }

        public void Remove()
        {
            if (IsInvoking("ChangeBoxState"))
            {
                CancelInvoke("ChangeBoxState");
            }
        }

        private void ChangeBoxState()
        {
            Back.gameObject.SetActive(true);
            Front.gameObject.SetActive(true);
            if (Back.state == null || Front.state == null)
            {
                Back.Initialize(false);
                Front.Initialize(false);
                Invoke("ChangeBoxState", 0f);
                return;
            }
            else
            {
                if (IsInvoking("ChangeBoxState"))
                {
                    CancelInvoke("ChangeBoxState");
                }
            }

            switch (state)
            {
                case BoxState.Opening:
                    Back.AnimationName = "Opening_Back";
                    Back.loop = true;
                    Back.timeScale = 0.9f;
                    Front.AnimationName = "Opening_Front";
                    Front.loop = true;
                    Front.timeScale = 0.9f;
                    OpeningEffect.gameObject.SetActive(true);
                    ExplosionContainer.SetActive(false);
                    JumpToSlotContainer.SetActive(false);
                    OpenedEffect.gameObject.SetActive(false);
                    ClosedEffect.gameObject.SetActive(false);
                    break;
                case BoxState.InQueue:
                    OpeningEffect.gameObject.SetActive(false);
                    JumpToSlotContainer.SetActive(false);
                    ExplosionContainer.SetActive(false);
                    OpenedEffect.gameObject.SetActive(false);
                    ClosedEffect.gameObject.SetActive(true);
                    Back.loop = true;
                    Front.loop = true;
                    Back.AnimationName = "Closed_Back";
                    Front.AnimationName = "Closed_Front";
                    JumpCallback?.Invoke();
                    JumpCallback = null;
                    break;
                case BoxState.Closed:
                    OpeningEffect.gameObject.SetActive(false);
                    ExplosionContainer.SetActive(false);
                    JumpToSlotContainer.SetActive(false);
                    OpenedEffect.gameObject.SetActive(false);
                    ClosedEffect.gameObject.SetActive(true);
                    Back.loop = true;
                    Front.loop = true;
                    Back.AnimationName = "Closed_Back";
                    Front.AnimationName = "Closed_Front";
                    JumpCallback?.Invoke();
                    JumpCallback = null;
                    break;
                case BoxState.Opened:
                    OpeningEffect.gameObject.SetActive(false);
                    ExplosionContainer.SetActive(false);
                    JumpToSlotContainer.SetActive(false);
                    OpenedEffect.gameObject.SetActive(true);
                    ClosedEffect.gameObject.SetActive(false);
                    Back.loop = true;
                    Front.loop = true;
                    Back.AnimationName = "Opened_Back";
                    Front.AnimationName = "Opened_Front";
                    break;
                case BoxState.Explosion:
                    OpeningEffect.gameObject.SetActive(false);
                    ExplosionContainer.SetActive(true);
                    JumpToSlotContainer.SetActive(false);
                    OpenedEffect.gameObject.SetActive(false);
                    ClosedEffect.gameObject.SetActive(false);
                    ExplosionDirector.stopped += Explosion_played;
                    ExplosionDirector.Play();
                    Back.loop = false;
                    Front.loop = false;
                    Back.AnimationName = "Explosion_Back";
                    Front.AnimationName = "Explosion_Front";
                    break;
                case BoxState.JumpToSlot:
                    
                    OpeningEffect.gameObject.SetActive(false);
                    ExplosionContainer.SetActive(false);
                    OpenedEffect.gameObject.SetActive(false);
                    ClosedEffect.gameObject.SetActive(false);
                    JumpToSlotContainer.SetActive(true);
                    JumpToSlotDirector.stopped += Jump_played;
                    JumpToSlotDirector.Play();
                    Back.loop = false;
                    Front.loop = false;
                    Back.AnimationName = "JumpToSlot_Back";
                    Front.AnimationName = "JumpToSlot_Front";
                    break;
                case BoxState.SlotsFull:
                    Back.loop = false;
                    Front.loop = false;
                    OpeningEffect.gameObject.SetActive(false);
                    ExplosionContainer.SetActive(false);
                    JumpToSlotContainer.SetActive(false);
                    OpenedEffect.gameObject.SetActive(false);
                    ClosedEffect.gameObject.SetActive(false);
                    Back.AnimationName = "Closed_Back";
                    Front.AnimationName = "Closed_Front";
                    Back.timeScale = 0;
                    Front.timeScale = 0;
                    break;
                case BoxState.GetOneLoot:
                    if (currentLootCard != null)
                    {                        
                        StartAppear();
                    }
                    break;
                default:
                    break;
            }
        }

        void StartAppear()
        {
            IsApperEffectStart = false;
            //Vector2 CardAppearPosition = new Vector2(currentLootCard.GetComponent<RectTransform>().position.x, OneLootAppearEffectRect.position.y);
            //OneLootAppearEffectRect.position = CardAppearPosition;
            GetOneLootDirector.Play();
            StartCoroutine(OneLootAppearEffect(currentLootCard.GetRarity()));
        }
        public void StopAppear()
        {
            StopAllCoroutines();
            AppearFinished();
        }
        public void SkipExplosion()
        {
            ExplosionDirector.time = 2.5;
        }

        public bool IsApperEffectStart;
        IEnumerator OneLootAppearEffect(CardRarity rarity)
        {
            yield return new WaitForSeconds(0.4f);
            OneLootAppearParticleSystem.Play();
            IsApperEffectStart = true;
            yield return new WaitForSeconds(0.2f);
            AppearFinished();
        }

        private void AppearFinished()
        {
            getOneLootCallback?.Invoke();
            ChangeState(BoxState.Opening);
        }
        internal IEnumerator AppearNextState()
        {
            yield return new WaitForSeconds(0.6f);
            ChangeState(BoxState.Closed);
        }

        internal void Init(BoxState state, BinaryLoot binary)
        {
            this.BinaryData = binary;
            this.state = state;
            if (BoxScaler == null)
            {
                BoxScaler = GetComponent<RectScaleToBehaviour>();
                if (BoxScaler == null)
                {
                    BoxScaler = gameObject.AddComponent<RectScaleToBehaviour>();
                }
            }
            if (SortBox == null)
            {
                SortBox = gameObject.AddComponent<SortingGroup>();
                SortBox.enabled = false;
                SortBox.sortingLayerName = "PopUpWindows";
                SortBox.sortingOrder = 40;
            }
            if (BoxPositionBehaviour == null) {
                BoxPositionBehaviour = gameObject.AddComponent<RectPositionToBehaviour>();
                BoxPositionBehaviour.SetLerpSpeed(0.35f);
            }
            ResetScale();
            ChangeBoxState();
        }

        private void Jump_played(PlayableDirector obj)
        {
           if (LootBehaviour)
            {
                LootBehaviour.Arrived();
            }
            JumpCallback?.Invoke();
            JumpCallback = null;
         //   ChangeBoxState();

        }        

        void Update()
        {
            if(ExplosionDirector != null && ExplosionDirector.state == PlayState.Playing)
            {
                if(ExplosionDirector.time > 1.53f)
                {
                    if (explosionCallback != null)
                    {
                        explosionCallback();
                        explosionCallback = null;
                    }
                }
            }
        }

        private void Explosion_played(PlayableDirector obj)
        {
            obj.transform.parent.gameObject.SetActive(false);
            Front.gameObject.SetActive(false);
            Back.gameObject.SetActive(false);            
        }

        public void SetScaleMultiplier(float multiplier)
        {
            if(BoxScaler)
               BoxScaler.SetScaleMultiplier(multiplier);
        }
    }
}
