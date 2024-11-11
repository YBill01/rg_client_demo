using DG.Tweening;
using Legacy.Database;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class TimerBehaviour : MonoBehaviour
    {

        private int timeleft;

        private int time;
        private Sequence sequence;
        private GameObject textTrailParticle;

        private TextMeshProUGUI timer_text;

        [SerializeField]private AudioSource audioSource_countDown;
        [SerializeField] private AudioSource audioSource_battleEndIn;

        void Start()
        {
            timer_text = GetComponent<TextMeshProUGUI>();
            var settings = Legacy.Database.Settings.Instance.Get<BaseBattleSettings>();
            timeleft = settings.mana.lastmin * 1000;
            SetStartTimerAnim();
        }

        public void SetTime(int Time)
        {
            time = Time;
            if (Time < timeleft)
            {
                GetComponent<Animator>().Play("FlashTimer");
            }
            if (!IsInvoking("SetTimerText"))
            {
                InvokeRepeating("SetTimerText", 0f, 1f);
            }
        }

        public void SetStartBattleAnimationTime()
        {
            if (!IsInvoking("SetTimerTextStartBattle"))
                InvokeRepeating("SetTimerTextStartBattle", 0f, 1.4f);

        }
        public void StopInvokeTextAnimation()
        {
            CancelInvoke("SetTimerTextStartBattle");
        }

        private void SetTimerText()
        {
            uint minutes = (uint)((time / 60) / 1000);
            uint seconds = (uint)(time / 1000) - (minutes * 60);
            timer_text.text = GetText(minutes, seconds);

            if (time < 10000 && time != _savedTime)
            {
                audioSource_countDown.Play();
                _savedTime = time;
                _battleEndInPlayed = false;
            }
            else if (time < 13000 && !_battleEndInPlayed)
            {
                audioSource_battleEndIn.Play();
                _battleEndInPlayed = true;
            }
            if (minutes == 0 && seconds == 0 && !_battleStageIsActive)
            {
                SoundManager.Instance.PlayBattleMusicActive();
                _battleStageIsActive = true;
            }
        }
        private int _savedTime;
        private bool _battleEndInPlayed = false;
        private bool _battleStageIsActive = false;

        private void SetTimerTextStartBattle()
        {
            GetComponent<TextMeshProUGUI>().text = GetText();
            PlayStartTimerAnimations();
        }

        private string GetText(uint minutes, uint seconds)
        {
            var _seconds = seconds.ToString();
            if (_seconds.Length < 2)
                _seconds = "0" + _seconds;
            return "0" + minutes.ToString() + ":" + _seconds;
        }


        private string[] startTexts = { "locale:1072", "locale:1075" };
        int counter = -1;
        private string GetText()
        {
            counter++;
            if (counter >= startTexts.Length)
            {
                StopInvokeTextAnimation();
                return "";
            }
            var text = Locales.Get(startTexts[counter]);
            return text;
        }

        private void SetStartTimerAnim()
        {
            if (!this.gameObject.GetComponent<Animator>())
            {
                Vector3 OriginalScale = Vector3.one;
                Vector3 MaxScale = new Vector3(OriginalScale.x + 0.1f, OriginalScale.y + 0.1f, OriginalScale.z + 0.1f);
                float timeSpeed = 1f;
                sequence = DOTween.Sequence()
                             .AppendCallback(() =>
                             {
                                 if (counter == startTexts.Length - 1)
                                 {
                                     this.GetComponent<TextMeshProUGUI>().fontSize = 115;
                                     MainCameraMoveBehaviour.instance.StartZoomCamera(0.35f, 100f);
                                 }
                             })
                             .Append(transform.DOScale(0f, 0f))
                             .Append(transform
                                .DOScale(MaxScale, 0.15f * timeSpeed)
                                .SetEase(Ease.OutQuad))
                             .Append(transform
                                .DOScale(OriginalScale, 0.06f * timeSpeed)
                                .SetEase(Ease.Linear))
                             .Append(transform
                                .DOScale(OriginalScale, 1f * timeSpeed)
                                .SetEase(Ease.Linear))
                             .Append(transform
                                .DOScale(MaxScale, 0.08f * timeSpeed)
                                .SetEase(Ease.Linear))
                             .Append(transform
                                .DOScale(Vector3.zero, 0.15f * timeSpeed)
                                .SetEase(Ease.Linear))
                             ;
                sequence.SetAutoKill(false);
                sequence.WaitForStart();
            }
        }

        private void PlayStartTimerAnimations()
        {
            sequence.Restart();
            sequence.Play();
        }
        //
        public void SetUnactive()
        {
        }
    }
}
