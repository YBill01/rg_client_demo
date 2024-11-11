using DG.Tweening;
using Legacy.Client;
using Legacy.Database;
using Legacy.Effects;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Legacy.Client
{
    public class SonnelonBallBehaviour : MonoBehaviour
    {
        private BattleBucketsSystem _buckets;
        private float2 previourPosition = new float2(float.MaxValue, float.MaxValue);
        [SerializeField] private GameObject mainBall;
        [SerializeField] private GameObject container;
        private float timer = 0;
        private ParticleSystem[] pss;

        void OnEnable()
        {
            var manager = ClientWorld.Instance.EntityManager;
            _buckets = ClientWorld.Instance.GetOrCreateSystem<BattleBucketsSystem>();
            pss = this.GetComponentsInChildren<ParticleSystem>();
            timer = 0;
            mainBall.SetActive(true);
            foreach (var ps in pss)
            {
                if (ps.isPlaying)
                {
                    ps.Play(true);
                }
            }

            var _proxy = GetComponent<EntityProxyBehaviour>();

            if (_proxy == null) return;
            var _repl = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<EntityDatabase>(_proxy.Entity);
            GetHero();
            if (Legacy.Database.Effects.Instance.Get(_repl.db, out BinaryEffect effect))
            {
                container.transform.position = new Vector3(hero.position.x, 12f, hero.position.z); ;
                container.transform.DOKill(true);
                var realDelay = effect.delay / 1000f;
                var offsetDelay = 0.3f;
                container.transform
                    .DOLocalMove(Vector3.zero, realDelay + offsetDelay)
                    .SetEase(Ease.InQuart)
                    .OnComplete(() =>
                    {
                        container.transform.DOLocalJump(Vector3.zero, 1f, 2, 1f);
                    });
            }

        }
        private Transform hero = null;

        private void GetHero()
        {
            var _proxy = GetComponent<EntityProxyBehaviour>();
            if (_proxy == null) return;
            var _effectData = ClientWorld.Instance.EntityManager.GetComponentData<EffectData>(_proxy.Entity);
            var _buckets = ClientWorld.Instance.GetOrCreateSystem<Legacy.Client.BattleBucketsSystem>();
            if (_buckets.Minions.TryGetValue(_effectData.source, out MinionClientBucket bucket))
            {
                hero = ClientWorld.Instance.EntityManager.GetComponentObject<Transform>(bucket.entity);
            }
        }
        private void Update()
        {
            var epb = GetComponent<EntityProxyBehaviour>();

            if (!epb || epb.Entity == null) return;
            var selfEntity = epb.Entity;
            var EM = ClientWorld.Instance.EntityManager;
            var ed = EM.GetComponentData<EffectData>(selfEntity);
            if (ed.state >= EffectState.Complete)
            {
                mainBall.SetActive(false);
                foreach (var ps in pss)
                {
                    if (ps.isPlaying)
                    {
                        ps.Stop(true);
                    }
                }
            }
        }
    }
}