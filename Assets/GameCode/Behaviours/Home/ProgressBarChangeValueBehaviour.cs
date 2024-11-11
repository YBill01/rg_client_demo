
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


namespace Legacy.Client
{
    public class ProgressBarChangeValueBehaviour : MonoBehaviour, IParticleReciever
    {
        public UnityEvent FullBarEvent;//Эмитится, каждый раз когда бар полностью заполняется или переполняется
        public UnityEvent OneLevelBack;//Эмитится, каждый раз когда бар полностью заполняется или переполняется
        public float PercentComplete
        {
            get
            {
                return Mathf.Clamp01(1.0f * currentValueInt / maxValueInt);
            }
        }
        private int currentValueInt => Mathf.RoundToInt(currentValue);
        private int maxValueInt => Mathf.RoundToInt(maxValue);

        [SerializeField, Range(0.0f, 1.0f)] float LerpSpeed = 0.4f;
        [SerializeField] RectTransform Filler;
        [SerializeField] TextMeshProUGUI ValueText;
        [SerializeField] GameObject effect;
        [SerializeField] DelayedRewardTargetBehaviour AfterBattleTarget;
        [SerializeField] Transform lineLayer;
        [SerializeField] GameObject line;

        bool HasMaxValue;//Для случаев, когда не требуется вывод максимального значения, а только одного.
        bool BarOnly; //Для случаев, когда нужен только прогресс-бар текст на нем написанный управляется отдельно кастомно не здесь.

        private float maxValue;
        private float oldValue;
        private float currentValue;
        private int tempValueInt;
        private float newValue;
        private bool FullEventInvoked = false;
        private bool inited = false;
        private Vector2 CurrentFillerAnchorMax;
        private Vector2 anchorCurrent;
        private float timer = 0f;
        private float allTime = 0f;

        private void OnEnable()
        {
            WaitParticles = false;
        }

        bool WaitParticles = false;
        float ParticlesPercentage = 0.0f;
        private bool canUpdate = true;

        void Start()
        {
            CurrentFillerAnchorMax.y = Filler.anchorMax.y;
        }
        public int GetHoldValue()
        {
            return AfterBattleTarget?.CheckHold() ?? 0;
        }
        public void SetHoldValue(uint count)
        {
            AfterBattleTarget?.SetDelayed(count);
        }

        void Update()
        {
            if (canUpdate)
            {
                if (!WaitParticles)
                { 
                    currentValue = Mathf.Lerp(currentValue, newValue, LerpSpeed);
                }
                

                SetProgressAnchor();

                if (tempValueInt != currentValueInt)
                {
                    tempValueInt = currentValueInt;
                    UpdateView();
                    if (currentValueInt < maxValueInt)
                    {
                        if (FullEventInvoked)
                        {
                            FullEventInvoked = false;
                        }
                    }
                    else
                    {
                        if (!FullEventInvoked)
                        {
                            FullBarEvent.Invoke();
                            FullEventInvoked = true;
                        }
                    }
                }
            }
        }

        public void StopUpdating(bool value)
        {
            canUpdate = !value;
        }

        private float GetAnchorPersentage(float remainTime)
        {
            var t = remainTime / allTime;
            return t;
        }

        public void AnimationToZero(float time)
        {
            timer = 1;
            allTime = timer;
            anchorCurrent = Filler.anchorMax;
            CurrentFillerAnchorMax.x = 0;
        }

        public void AnimationToCurrentValue(float time)
        {
            Filler.anchorMax = new Vector2(0, 1);
            anchorCurrent = Filler.anchorMax;
            UpdateView();
            timer = 0;
        }

        public void SetMaxSlider()
        {
            CurrentFillerAnchorMax.x = 1;
            Filler.anchorMax = new Vector2(CurrentFillerAnchorMax.x, 1);
            anchorCurrent = Filler.anchorMax;
            timer = 0.5f;
        }

        private void SetProgressAnchor()
        {
            if (timer > 0)
            {
                Filler.anchorMax = Vector2.Lerp(anchorCurrent, CurrentFillerAnchorMax, 1 - GetAnchorPersentage(timer));
                if (CurrentFillerAnchorMax.x != 1f)
                    if (effect)
                        effect.SetActive(true);
            }
            else
            {
                Filler.anchorMax = CurrentFillerAnchorMax;
                if (effect)
                {
                    effect.SetActive(false);
                    CardProgressBarBehaviour cardProgressBarBehaviour = GetComponent<CardProgressBarBehaviour>();
                    if (cardProgressBarBehaviour)
                    {
                        cardProgressBarBehaviour.HideStaticLighter();
                        cardProgressBarBehaviour.SetSlider((uint)newValue, (uint)maxValue);
                    }
                }
                
            }
            timer -= Time.deltaTime;
        }

       private bool isInit = false;
        public void SetDelitel( uint maxValue = 0) {
            if (isInit) return;
            isInit = true;
            float width = lineLayer.GetComponent<RectTransform>().rect.width;
            float k = 100 / maxValue;
            for (byte i = 1; i < maxValue; i++)
            {
                GameObject _line = Instantiate(line, lineLayer);
                _line.transform.position = lineLayer.position;
                float _width = width * (k*i/100) -14f;
                _line.transform.localPosition = Vector3.zero;
              _line.transform.localPosition = new Vector3(_line.transform.localPosition.x + _width, _line.transform.localPosition.y, _line.transform.localPosition.z);
            }
        }
        public void SetAnim(uint value, uint maxValue = 0,float speed=.1f,UnityAction callback=null)
        {
            canUpdate = false;
           float _x = Mathf.Clamp01(1.0f * value / maxValue);

            //UpdateViewAnim();
            //currentValue = Mathf.Lerp(currentValue, newValue, LerpSpeed);
            // view.DOScale(_size, speed).SetEase(aninType)
            // Filler.anchorMax = Vector2.Lerp(anchorCurrent, CurrentFillerAnchorMax, 1 - GetAnchorPersentage(timer));
            if (value > 0)
            {
                DOTween.To(() => Filler.anchorMax, x => Filler.anchorMax = x, new Vector2(_x, 1f), speed)
                    .OnComplete(() =>
                    {
                        ValueText.text = $"{LegacyHelpers.FormatByDigits(value.ToString())} / {LegacyHelpers.FormatByDigits(maxValue.ToString())}";
                        if (callback != null)
                            callback();
                    });
            }
            else
            {
               // ValueText.text = $"{LegacyHelpers.FormatByDigits(value.ToString())} / {LegacyHelpers.FormatByDigits(maxValue.ToString())}";
                Filler.anchorMax = new Vector2(0f, 1f);
            }

           
          
        }
        public void Set(uint value, bool HasMaxValue = false, uint maxValue = 0)
        {
            this.HasMaxValue = HasMaxValue;
            this.maxValue = maxValue;
            ChangeValue(value);
            if (!inited)
            {
                inited = true;
                if (AfterBattleTarget != null)
                {
                    var holdValue = AfterBattleTarget.CheckHold();
                    if (holdValue > 0)
                    {
                        currentValue = value - holdValue;
                        if (currentValue < 0)
                        {
                            currentValue += maxValue;
                            OneLevelBack.Invoke();
                        }
                        ChangeWithParticles();
                    }
                }                
                tempValueInt = currentValueInt;
            }
            UpdateView();
        }


        public void SetOnlyBar(uint value, uint maxValue = 0)
        {
            BarOnly = true;
            HasMaxValue = true;
            this.maxValue = maxValue;
            ChangeValue(value);
            if (!inited)
            {
                inited = true;
                if (AfterBattleTarget != null)
                {
                    var holdValue = AfterBattleTarget.CheckHold();
                    if (holdValue > 0)
                    {
                        currentValue = value - holdValue;
                        if (currentValue < 0)
                        {
                            currentValue += maxValue;
                            OneLevelBack.Invoke();
                        }
                        ChangeWithParticles();
                    }
                }
                
                tempValueInt = currentValueInt;
                UpdateView();
            }
        }

    /*    private void UpdateViewAnim()
        {
            if (ValueText != null && !BarOnly)
            {
                if (HasMaxValue)
                {
                    ValueText.text = $"{LegacyHelpers.FormatByDigits(currentValueInt.ToString())} / {LegacyHelpers.FormatByDigits(maxValueInt.ToString())}";
                }
                else
                {
                    ValueText.text = LegacyHelpers.FormatByDigits(currentValueInt.ToString());
                }
            }
           /* if (HasMaxValue)
            {
               /* if (timer <= 0)
                {
                    CurrentFillerAnchorMax.x = PercentComplete;
                    if (currentValueInt < maxValueInt)
                    {
                        if (TryGetComponent<CardProgressBarBehaviour>(out var bar))
                        {
                            bar.UpgateProgressBarSlider();
                        }
                    }
                }
            }
            else
            {
                SetMaxSlider();
            }
        }*/
        private void UpdateView()
        {
            if (ValueText != null && !BarOnly)
            {
                if (HasMaxValue)
                {
                    ValueText.text = $"{LegacyHelpers.FormatByDigits(currentValueInt.ToString())} / {LegacyHelpers.FormatByDigits(maxValueInt.ToString())}";
                }
                else
                {
                    ValueText.text = LegacyHelpers.FormatByDigits(currentValueInt.ToString());
                }
            }

            if (HasMaxValue)
            {
                if (timer <= 0)
                {
                    CurrentFillerAnchorMax.x = PercentComplete;
                    if (currentValueInt < maxValueInt)
                    {
                        if (TryGetComponent<CardProgressBarBehaviour>(out var bar))
                        {
                            bar.UpgateProgressBarSlider();
                        }
                    }
                }
            }
            else
            {
                SetMaxSlider();
            }
        }

        public void ChangeValue(uint value)
        {
            newValue = value;
        }

        public void ParticleCame(float percentageComplete)
        {
            if (WaitParticles)
            {
                ParticlesPercentage = percentageComplete;
                if (newValue < oldValue)
                {
                    currentValue = Mathf.Lerp(oldValue, maxValue, ParticlesPercentage);
                }
                else
                {
                    currentValue = Mathf.Lerp(oldValue, newValue, ParticlesPercentage);
                }
                if (ParticlesPercentage == 1.0f)
                {
                    AfterBattleTarget?.DropHoldValue();
                    if (gameObject.activeInHierarchy)
                    {
                        StartCoroutine(FinishParticles());
                    }
                    else
                    {
                        WaitParticles = false;
                    }
                }
            }
        }

        public void ChangeWithParticles()
        {
            ParticlesPercentage = 0.0f;
            WaitParticles = true;
            oldValue = currentValue;
        }
        IEnumerator FinishParticles()
        {
            yield return new WaitForSeconds(1.0f);
            WaitParticles = false;
        }
    }
}