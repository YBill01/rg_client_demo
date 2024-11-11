using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class ParticleSystemCallback : MonoBehaviour
    {
        public void OnParticleSystemStopped()
        {
            var minionGO = this.gameObject.GetComponentInParent<DeathEffect>().gameObject;
            ObjectPooler.instance.MinionBack(minionGO);
            minionGO.SetActive(false);
            this.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }
}
