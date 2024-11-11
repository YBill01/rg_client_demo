using Legacy.Database;
using System.Collections;
using UnityEngine;
using static EZCameraShake.CameraShaker;

namespace Legacy.Client
{
    public class SimpleHitEffect : MonoBehaviour
    {
        [Header("ShakeCamera Settings")]
        [SerializeField] ShakeSettings ShakeSettings;

        [Space]
        public GameObject Effect;
        public GameObject AdditionalEffect;
        public GameObject HitEffectObject;
        public float effectDuration = 1.5f;
        public bool shouldSetEffectOnTarget = false;
        public void HitStart()
        {
            if (Effect == null) return;
            if (!IsAttack()) return;

            Effect.SetActive(true);

            if (shouldSetEffectOnTarget) SetEffectPosition();

            StartCoroutine(StopEffect());

            if (AdditionalEffect)
            {
                AdditionalEffect.SetActive(true);
                StartCoroutine(StopAdditionalEffect());
            }

            ShakeSettings.Shake();

            IEnumerator StopEffect()
            {
                yield return new WaitForSeconds(effectDuration);
                Effect.SetActive(false);
            }

            IEnumerator StopAdditionalEffect()
            {
                yield return new WaitForSeconds(0.45f);
                AdditionalEffect.SetActive(false);
            }
        }

        private void SetEffectPosition()
        {
            var _buckets = ClientWorld.Instance.GetOrCreateSystem<BattleBucketsSystem>();
            var epb = GetComponent<EntityProxyBehaviour>();

            if (!epb || epb.Entity == null) return;
            var selfEntity = epb.Entity;
            var EM = ClientWorld.Instance.EntityManager;
            if (!EM.HasComponent<MinionData>(selfEntity)) return;
            var md = EM.GetComponentData<MinionData>(selfEntity);

            if (!EM.HasComponent<MinionData>(selfEntity)) return;

            if (_buckets.Minions.TryGetValue(md.atarget, out MinionClientBucket bucket))
            {
                if (EM.HasComponent<Transform>(bucket.entity))
                {
                    var tr = EM.GetComponentObject<Transform>(bucket.entity);
                    Vector3 effectPosition = tr.position;
                    effectPosition.y = 0.5f;
                    Effect.transform.position = effectPosition;

                }
            }
        }

        private bool IsAttack()
        {
            var epb = GetComponent<EntityProxyBehaviour>();

            if (!epb || epb.Entity == null) return false;
            var selfEntity = epb.Entity;
            var EM = ClientWorld.Instance.EntityManager;
            if (!EM.HasComponent<MinionData>(selfEntity)) return false;


            var md = EM.GetComponentData<MinionData>(selfEntity);

            if (md.state == MinionState.Paralize) return false;
            if (md.state == MinionState.Idle) return false;
            if (md.state == MinionState.Move) return false;

            return true;
        }

    }
}
