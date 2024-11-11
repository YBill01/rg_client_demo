using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Legacy.Database;

namespace Legacy.Client
{
    public class ZacZarAxeOnSkillBeh : MonoBehaviour
    {
        [SerializeField] private MeshRenderer zacZarAxeMesh;
        [SerializeField] private ParticleSystem[] particlesSystems;

        private EntityProxyBehaviour _proxy;
        private bool _skillStarted;
        private bool _skillFinished;

        private void OnEnable()
        {
            _skillStarted = false;
            _skillFinished = false;

            _proxy = GetComponent<EntityProxyBehaviour>();
            if (_proxy == null)
                return;

            ActivatePrefabView(false);
        }

        private void Update()
        {
            if (_skillFinished || _proxy == null || _proxy.Entity == null)
                return;

            var _effectData = ClientWorld.Instance.EntityManager.GetComponentData<EffectData>(_proxy.Entity);
            if (_effectData.Equals(default(EffectData)))
            {
                return;
            }

            if(_effectData.state == EffectState.Active)
            {
                if (!_skillStarted)
                {
                    _skillStarted = true;
                    ActivatePrefabView(true);
                }

                //var divDistToHero = Vector2.Distance(_effectData.position, _heroPos) / 2;
                //var vfxScale = Mathf.Clamp(divDistToHero, 0, particlesSystemStartScale);
                //particlesSystems[0].transform.localScale = new Vector3(vfxScale, vfxScale, vfxScale);
            }

            if(_skillStarted && _effectData.state != EffectState.Active)
            {
                _skillFinished = true;
                ActivatePrefabView(false);
            }
        }

        private void ActivatePrefabView(bool on)
        {
            zacZarAxeMesh.enabled = on;
            foreach (var vfx in particlesSystems)
            {
                if (on)
                {
                    vfx.Play();
                }
                else
                {
                    vfx.Stop();
                    vfx.Clear();
                }
            }
        }
    }
}