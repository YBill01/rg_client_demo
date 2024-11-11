using DG.Tweening;
using Legacy.Database;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Legacy.Client
{
    [RequireComponent(typeof(Animator))]
    public class LegacyButton : Button
    {
        [SerializeField] private GameObject blickControl;
        [SerializeField] private bool muteSound = false;

        private UnityEvent successClickEvent = new UnityEvent();
        private UnityEvent anyClickEvent = new UnityEvent();
        public bool isLocked = false;
        public string localeAlert;
        public Action LoockedOnClick = null;    // isLocked функция
        public UnityEvent SuccessClickEvent { get => successClickEvent; }

        public bool IsDown { get => isDown; }
        public UnityEvent AnyClickEvent { get => anyClickEvent; set => anyClickEvent = value; }

        private bool isDown;
        protected override void Awake()
        {
            AnyClickEvent.AddListener(Clicked);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            isDown = true;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            isDown = false;
        }

        private float previousClick = 0;
        private float click = 0;
        public override void OnPointerClick(PointerEventData eventData)
        {
            click = Time.time;
            if (previousClick != 0 && click - previousClick <= 0.5f) return;
            previousClick = click;


            base.OnPointerClick(eventData);
            anyClickEvent.Invoke();

            if (interactable)
            {
                successClickEvent.Invoke();
            }
            if (!isLocked && !muteSound)
            {
                ButtonSoundPlayManager.Instance.PlayDefaultClip();
            }
            if (isLocked && !muteSound)
            {
                if (LoockedOnClick != null)
                {
                    LoockedOnClick();
                }
                PopupAlertBehaviour.ShowHomePopupAlert(Input.mousePosition, localeAlert);
                ButtonSoundPlayManager.Instance.PlayLockedClip();
            }
            animator.Play("PressPunk");
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
        }

        private float PunchPower = 1.1f;
        private PunchType PunchType = PunchType.Size;
        private event Action OnComplete;
        private int animationIndex;

        private void Clicked()
        {
            animationIndex++;
            var i = animationIndex;

            if (PunchType == PunchType.Position)
                transform.DOPunchPosition(new Vector3(transform.position.x, transform.position.y, transform.position.z - PunchPower), 0.2f, 1).OnComplete(() => OnCompleteAnimation(i));
            if (PunchType == PunchType.Size)
                transform.localScale = Vector3.one;
        }

        private void OnCompleteAnimation(int index)
        {
            if (index < animationIndex)
                return;
            OnComplete?.Invoke();
        }

        public void EnableBlick()
        {
            blickControl?.SetActive(true);
        }

        public void DisableBlick()
        {
            blickControl?.SetActive(false);
        }
    }
}
