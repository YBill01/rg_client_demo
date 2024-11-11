using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace Legacy.Client
{
    public class ButtonPunch : MonoBehaviour
    {
        public float PunchPower;
        public PunchType PunchType;
        private LegacyButton button;
        private Toggle toggle;
        public event Action OnComplete;
        [NonSerialized]
        public bool IsPlayed;
        private int animationIndex;

        private void Awake()
        {
            button = GetComponent<LegacyButton>();
            toggle = GetComponent<Toggle>();
            if (button != null)
            {
                button.AnyClickEvent.AddListener(Clicked);
            }

            if (toggle != null)
            {
                toggle.onValueChanged.AddListener((e) =>
                {
                    if (e)
                        Clicked();
                });
            }
        }

        private void Clicked()
        {
            animationIndex++;
            var i = animationIndex;

            IsPlayed = true;
            if (PunchType == PunchType.Position)
            {

                transform.DOPunchPosition(new Vector3(transform.position.x, transform.position.y, transform.position.z - PunchPower), 0.2f, 1).OnComplete(() => OnCompleteAnimation(i));
                //transform.localScale = Vector3.one * 1.05f;

            }
            if (PunchType == PunchType.Size)
            {
                //startTime = Time.time;
                //transform.localScale = Vector3.one * (1.05f * PunchPower);
                transform.DOPunchScale(transform.localScale * PunchPower, 0.2f, 1).OnComplete(() => OnCompleteAnimation(i));

            }
        }

        //private float startTime;
        //private float effectTime = 0.2f;
        //public void Update()
        //{
        //	if (!IsPlayed) return;
        //	var currentTime = Time.time;
        //	var delta = currentTime - startTime;
        //	var half = effectTime / 2;
        //	var theOne = delta / effectTime;
        //	float addVal = (1 - ((1 - Mathf.Pow((theOne * 2 - 1), 2)))*0.05f);
        //	transform.localScale = (Vector3.one) * addVal;
        //	if(currentTime>startTime + effectTime)
        //	{
        //		//OnCompleteAnimation(0);
        //		IsPlayed = false;
        //	}
        //		//* (1 + (effectTime - Math.Pow(delta - effectTime, 2)));
        //}

        private void OnCompleteAnimation(int index)
        {
            if (index < animationIndex)
                return;
            IsPlayed = false;
            OnComplete?.Invoke();
        }
    }

    public enum PunchType
    {
        Size,
        Position,

    }
}
