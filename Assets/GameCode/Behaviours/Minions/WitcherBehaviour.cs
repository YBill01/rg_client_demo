using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class WitcherBehaviour : InitBehaviour
    {
        public GameObject[] HandsOrbs;
        public GameObject[] HandsOrbsRed;

        public GameObject RedBullet;
        public GameObject RedCharge;
        public GameObject RedExplosion;
        private bool refreshed;

        public override void InitMaterial(bool isEnemy)
        {
            for (var i = 0; i < HandsOrbs.Length; i++)
            {
                HandsOrbs[i].SetActive(isEnemy);
                HandsOrbsRed[i].SetActive(isEnemy);
            }
            if (isEnemy)
            {
                var hitRange = GetComponent<RangeHitEffect>();
                if (hitRange != null)
                {
                    hitRange.Bullet = RedBullet;
                    hitRange.ChargeObject = RedCharge;
                    hitRange.Explosion = RedExplosion;
                }
            }
        }
    }
}