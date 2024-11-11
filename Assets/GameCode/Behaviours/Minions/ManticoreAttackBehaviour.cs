using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class ManticoreAttackBehaviour : MonoBehaviour
    {
        [SerializeField] private float secondBulletDelay = 0.1f;
        void Start()
        {
            GetComponent<RangeHitEffect>().SecondBulletDelay = secondBulletDelay;
        }
    }
}
