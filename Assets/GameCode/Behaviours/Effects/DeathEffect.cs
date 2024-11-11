using Legacy.Client;
using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Legacy.Client
{
    public class DeathEffect : MonoBehaviour
    {
        [SerializeField] MinionSoundManager soundManager;

        public GameObject Effect;
        public Transform DeathEffectSpawnPoint;
        [SerializeField] private bool _playDeathAnimation = false;
        public bool PlayDeathAnimation { get => _playDeathAnimation; }

        public string addDeathEffect;

        public void Die()
        {
            GetComponent<MinionPanel>().Delete();
            if (PlayDeathAnimation)
                StartCoroutine(DelayedDie());
            else
                MakeDeath();
        }

        private IEnumerator DelayedDie()
        {
            var animator = GetComponent<Animator>();
            animator.ResetBools();
            var currentClipLength = animator.GetCurrentAnimatorStateInfo(0).length;

            yield return new WaitForSeconds(currentClipLength);
            MakeDeath();
        }

        private void MakeDeath()
        {
            Effect.transform.position = DeathEffectSpawnPoint ? DeathEffectSpawnPoint.position : Vector3.zero;
            Effect.SetActive(true);

            GetComponent<MinionInitBehaviour>().DoMinionInvisible();
            if (GetComponent<InitBehaviour>()) 
                GetComponent<InitBehaviour>().DoMinionInvisible();
        }

        public void DoAddDeathEffect()
        {
            if (addDeathEffect == "") return;
            var effect = ObjectPooler.instance.GetEffect(addDeathEffect);
            effect.transform.position = transform.position;
            effect.SetActive(true);

            LegacyHelpers.TurnParticlesOn(effect);
        }
    }
}
