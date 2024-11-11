using System;
using UnityEngine;

namespace Legacy.Visual.NonTweenAnimations
{
	public class TouchShake : INonTweenAnimation
	{
        public TouchShake() { Fade = false; }

        public bool Fade { get; set; }

        private bool _isZero;
		public bool IsZero => _isZero;

        public GameObject target { get; set; }

        public float shakeMultiplierFixed;

        public float power = 0;
        public float angleLimit = 2;
        public float scaleTimeValue = 35;
        private float angleValue = 0;

		private float updateTime = 0.03f;
        private float fadeLerp = 0.25f;
        private float unfadeLerp = 0.15f;
        public void Update()
		{
            float finalScaleTimeValue = scaleTimeValue;
            float dTime = Time.deltaTime;
			float updates = dTime / updateTime;
            if (Fade)
            {
                float fadeValue = 1-(float)Math.Pow(1-fadeLerp, updates);
                power = Mathf.Lerp(power, 0, fadeValue);
                target.transform.localScale = Vector3.Lerp(target.transform.localScale, Vector3.one, fadeValue);
                if (power < 0.001f)
                {
                    target.transform.localScale = Vector3.one;
                    finalScaleTimeValue = 0;
                    _isZero = true;
                }
            }
            else
            {
                _isZero = false;
                power = Mathf.Lerp(power, 1, unfadeLerp);
                target.transform.localScale = Vector3.Lerp(target.transform.localScale, Vector3.one * 1.05f, 1-(float)Math.Pow(1-unfadeLerp, updates));
            }
            angleValue += dTime * finalScaleTimeValue;
            Vector3 result = Vector3.zero;
            result.z = Mathf.Sin(angleValue) * angleLimit * power;
            result.x = 0;
            result.y = 0;
            var lp = target.transform.localRotation;
            lp.eulerAngles = result;
            target.transform.localRotation = lp;
        }
	}
}