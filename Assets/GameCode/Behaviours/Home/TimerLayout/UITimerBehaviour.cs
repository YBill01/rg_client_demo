using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Legacy.Client.UITimerValueBehaviour;

namespace Legacy.Client
{
    public class UITimerBehaviour : MonoBehaviour
    {
        [Header("Fonts")]
        [SerializeField] float ValueFontSize;
        [SerializeField] float ValueTypeFontSize;
        [Space(10)]

        [SerializeField, Range(1, 4)] byte ShowingItemsCount;

        [SerializeField] RectTransform TimerLayoutRect;
        [SerializeField] GameObject TimerValuePrefab;

        [SerializeField] GameObject ClockIcon;

        public UnityEvent TimerFinished = new UnityEvent();
        public UnityEvent<uint> OnTimerUpdate = new UnityEvent<uint>();

        private Dictionary<TimerValueType, UITimerValueBehaviour> values = new Dictionary<TimerValueType, UITimerValueBehaviour>();

        DateTime finishTime;
        private bool isStatic = false;
        private bool isNarrowed = false;
        private TimerValueType narrowedType;

        public DateTime FinishTime => finishTime;


        void ClearLayout()
        {
            foreach (UITimerValueBehaviour child in values.Values)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        void Clear()
        {
            ClearLayout();
            values.Clear();
        }

		public void SetFinishedTime(uint secondsToOpen)
        {
            var finishTime = DateTime.Now.AddSeconds(secondsToOpen);
            SetFinishedTime(finishTime);
        }

        public void SetFinishedTime(DateTime finishTime)
        {
            Clear();
            isStatic = false;
            this.finishTime = finishTime.ToLocalTime();
            foreach (TimerValueType type in Enum.GetValues(typeof(TimerValueType)))
            {
                CreateLayoutElement(type);
            }
            UpdateTime();
            gameObject.SetActive(true);
        }

        public void SetStaticTime(uint value, bool narrowed = false)
        {
            //Debug.Log($"Static timer seconds: {value}");
            staticTime = TimeSpan.FromSeconds(value);
            //Debug.Log($"Static timer TimeSpan: {staticTime}");

            Clear();
            isStatic = true;

            if (narrowed)
			{
                isNarrowed = narrowed;

                if (staticTime.TotalSeconds < 60)
                {
                    CreateLayoutElement(TimerValueType.s);
                    narrowedType = TimerValueType.s;
                }
                else if (staticTime.TotalMinutes < 100)
				{
                    CreateLayoutElement(TimerValueType.m);
                    narrowedType = TimerValueType.m;
                }
				else if (staticTime.TotalHours < 100)
				{
                    CreateLayoutElement(TimerValueType.h);
                    narrowedType = TimerValueType.h;
                }
				else
				{
                    CreateLayoutElement(TimerValueType.d);
                    narrowedType = TimerValueType.d;
                }
            }
			else
			{
                if (staticTime.Days > 0)
                {
                    CreateLayoutElement(TimerValueType.d);
                }
                if (staticTime.Hours > 0)
                {
                    CreateLayoutElement(TimerValueType.h);
                }
                if (staticTime.Minutes > 0)
                {
                    CreateLayoutElement(TimerValueType.m);
                }
                if (staticTime.Seconds > 0)
                {
                    CreateLayoutElement(TimerValueType.s);
                }
            }

            if (IsInvoking("UpdateTime"))
            {
                CancelInvoke("UpdateTime");
            }
            UpdateTime();
            gameObject.SetActive(true);
        }

        void CreateLayoutElement(TimerValueType type)
        {
            UITimerValueBehaviour _value = Instantiate(TimerValuePrefab, TimerLayoutRect).GetComponent<UITimerValueBehaviour>();
            _value.Init(ValueFontSize, ValueTypeFontSize, type);
            _value.gameObject.SetActive(false);
            values.Add(type, _value);
        }

        public bool IsFinished { get { return DateTime.Now > finishTime; } }

        public TimeSpan staticTime;

        void Update()
        {
            if (isStatic) return;

            if (ClockIcon != null)
                ClockIcon.SetActive(!IsFinished);

            if (!IsFinished)
            {
                if (!IsInvoking("UpdateTime"))
                {
                    InvokeRepeating("UpdateTime", 0.0f, 0.5f);
                }
            }
            else
            {
                TimerFinished.Invoke();
                if (IsInvoking("UpdateTime"))
                {                    
                    CancelInvoke("UpdateTime");
                }
                gameObject.SetActive(false);
            }
        }

        void SetValue(TimerValueType type, byte value)
        {           
            if (value == 0) 
            {
                DisableValue(type);
                return;
            }
            if (values.TryGetValue(type, out UITimerValueBehaviour valueBehaviour))
            {
                valueBehaviour.gameObject.SetActive(true);
                
                valueBehaviour.SetValue(value);
            }
        }

        void UpdateTime()
        {
            byte showed = 0;
            var timeleft = isStatic ? staticTime : finishTime - DateTime.Now;

            OnTimerUpdate.Invoke((uint)Mathf.CeilToInt((float)timeleft.TotalSeconds));

            if (isStatic && isNarrowed)
			{
                switch (narrowedType)
                {
                    case TimerValueType.d:
                        SetValue(narrowedType, (byte)timeleft.TotalDays);
                        break;
                    case TimerValueType.h:
                        SetValue(narrowedType, (byte)timeleft.TotalHours);
                        break;
                    case TimerValueType.m:
                        SetValue(narrowedType, (byte)timeleft.TotalMinutes);
                        break;
                    case TimerValueType.s:
                        SetValue(narrowedType, (byte)timeleft.TotalSeconds);
                        break;
                }

                return;
            }

            if (timeleft.Days > 0)
            {
                SetValue(TimerValueType.d, (byte)timeleft.Days);
                showed++;
            }
            else
            {
                DisableValue(TimerValueType.d);
            }


            if (timeleft.Hours > 0 && showed < ShowingItemsCount)
            {
                SetValue(TimerValueType.h, (byte)timeleft.Hours);
                showed++;
            }
            else
            {
                if (showed > 0 && showed < ShowingItemsCount)
                {
                    SetValue(TimerValueType.h, (byte)timeleft.Hours);
                    showed++;
                }
                else
                {
                    DisableValue(TimerValueType.h);
                }
            }


            if (timeleft.Minutes > 0 && showed < ShowingItemsCount)
            {
                SetValue(TimerValueType.m, (byte)timeleft.Minutes);
                showed++;
            }
            else
            {
                if (showed > 0 && showed < ShowingItemsCount)
                {
                    SetValue(TimerValueType.m, (byte)timeleft.Minutes);
                    showed++;
                }
                else
                {
                    DisableValue(TimerValueType.m);
                }
            }


            if (timeleft.Seconds > 0 && showed < ShowingItemsCount)
            {
                SetValue(TimerValueType.s, (byte)timeleft.Seconds);
            }
            else
            {
                if (showed > 0 && showed < ShowingItemsCount)
                {
                    SetValue(TimerValueType.s, (byte)timeleft.Seconds);
                }
                else
                {
                    DisableValue(TimerValueType.s);
                }
            }
        }

        private void DisableValue(TimerValueType type)
        {
            if (values.TryGetValue(type, out UITimerValueBehaviour valueBehaviour))
            {
                valueBehaviour.gameObject.SetActive(false);
            }
        }

        public ushort GetSecondsToFinish()
        {
            return (ushort)Mathf.CeilToInt((float)(finishTime - DateTime.Now).TotalSeconds);
        }
    }
}
