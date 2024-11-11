using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Legacy.Client
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private GameObject[] _immediatelyHideOnHit;
        [SerializeField] private ParticleSystem[] _vfxToSmoothStopOnHit;
        protected Vector3 position { get => transform.position; set { transform.position = value; } }
        protected Quaternion rotation { get => transform.rotation; set { transform.rotation = value; } }

        protected float speed;
        protected Vector3 startPosition;
        protected Transform targetTrans;
        protected GameObject onHitVFX;

        private AudioSource _audioSource;

        void Awake()
        {
            _audioSource = GetComponentInChildren<AudioSource>();
        }

        private void OnEnable()
        {
            InternalBulletInit();
        }
        public void Init(Vector3 startPos, Transform targetTransform, float bulletSpeed, GameObject onHitEffect)
        {
            startPosition = startPos;
            speed = bulletSpeed;
            targetTrans = targetTransform;
            onHitVFX = onHitEffect;

            OnBeforeStart();
            OnFire();
        }
        protected virtual void OnFire()
        {
            StartCoroutine(BulletFlight());
            if(_audioSource != null)
            {
                _audioSource.Play();
            }
        }
        protected virtual IEnumerator BulletFlight()
        {
            var dist = Vector3.Distance(startPosition, targetTrans.position);
            var time = 0f;
            var flightTime = dist / speed;

            while (time < flightTime)
            {
                var progress = time / flightTime;

                position = Vector3.Lerp(startPosition, targetTrans.position, progress);
                transform.LookAt(targetTrans);

                yield return null;
                time += Time.deltaTime;
            }
            OnBeforeDie();
        }
        protected virtual void OnBeforeStart()
        {
            transform.position = startPosition;
            ActivateDeactivateBullet(true);
        }
        protected virtual void OnBeforeDie()
        {
            PlayOnHitVFX();
            ActivateDeactivateBullet(false);
        }
        protected virtual void ActivateDeactivateBullet(bool turnOn)
        {
            if (_immediatelyHideOnHit != null)
            {
                foreach (var item in _immediatelyHideOnHit)
                {
                    item.gameObject.SetActive(turnOn);
                }
            }

            if (_vfxToSmoothStopOnHit != null)
            {
                foreach (var item in _vfxToSmoothStopOnHit)
                {
                    if (turnOn)
                    {
                        item.ResetAndPlay();
                    }
                    else
                    {
                        item.Stop();
                    }
                }
            }
        }
        private void InternalBulletInit()
        {
            if (_immediatelyHideOnHit != null)
            {
                foreach (var gameObj in _immediatelyHideOnHit)
                {
                    var partSysts = gameObj.GetComponentsInChildren<ParticleSystem>();
                    foreach (var pSyst in partSysts)
                    {
                        var main = pSyst.main;
                        main.playOnAwake = true;
                    }
                    gameObj.SetActive(false);
                }
            }

            if (_vfxToSmoothStopOnHit != null)
            {
                foreach (var vfx in _vfxToSmoothStopOnHit)
                {
                    vfx.gameObject.SetActive(true);
                    vfx.Stop();
                }
            }
        }
        protected virtual void PlayOnHitVFX()
        {
            if (onHitVFX != null)
            {
                onHitVFX.transform.position = position;
                onHitVFX.transform.rotation = rotation;
                onHitVFX.SetActive(true);
                if (onHitVFX.GetComponent<ParticleSystem>())
                    onHitVFX.GetComponent<ParticleSystem>().ResetAndPlay();
            }
        }
        private void OnDisable()
        {
            ActivateDeactivateBullet(false);
        }
    }
}
