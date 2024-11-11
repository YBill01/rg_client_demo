using Legacy.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EZCameraShake.CameraShaker;

namespace Legacy.Client
{

    public class IrvaBehaviour : MonoBehaviour
    {
        public ParticleSystem Wave;

        [Header("ShakeCamera Settings")]
        [SerializeField] ShakeSettings shakeSettings;

        public void ShockWave()
        {
            var irvaParent = transform;
            irvaParent.LookAt(Vector3.zero);
            Wave.transform.SetParent(null);
            Wave.transform.LookAt(Vector3.zero);
            Wave.gameObject.SetActive(true);
            IEnumerator SetFalse()
            {
                yield return new WaitForSeconds(3f);
                Wave.gameObject.SetActive(false);
                Wave.transform.SetParent(irvaParent);
            }
            StartCoroutine(SetFalse());
        }
        public void ShakeEvent()
        {
            shakeSettings.Shake();
        }
    }

}
