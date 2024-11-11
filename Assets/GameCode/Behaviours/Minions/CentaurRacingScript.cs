using Legacy.Client;
using Legacy.Database;
using Legacy.Effects;
using System.Collections;
using Unity.Entities;
using UnityEngine;

namespace Legacy.Client
{
    public class CentaurRacingScript : MonoBehaviour
    {
        [SerializeField] private GameObject spearEffect;
        [SerializeField] private GameObject mainCentaurTrailEffect;
        [SerializeField] private GameObject HITCentaurBoomEffect;

        private void Update()
        {

            var epb = GetComponent<EntityProxyBehaviour>();
            if (!epb || epb.Entity == null) return;
            var selfEntity = epb.Entity;

            var EM = ClientWorld.Instance.EntityManager;
            if (!EM.HasComponent<MinionData>(selfEntity)) return;

            var md = EM.GetComponentData<MinionData>(selfEntity);
            if (md.state == MinionState.Skill1)
            {
                mainCentaurTrailEffect.SetActive(true);
                spearEffect.SetActive(true);
            }
            else
            {
                mainCentaurTrailEffect.SetActive(false);
                spearEffect.SetActive(false);
            }

            if (md.state == MinionState.Skill2)
            {
                HITCentaurBoomEffect.SetActive(true);
            }
        }
    }
}