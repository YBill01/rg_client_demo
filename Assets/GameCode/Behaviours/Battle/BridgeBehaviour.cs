using Legacy.Database;
using UnityEngine;

namespace Legacy.Client
{
    //You need to set "Bridge" animator controller into Animator Component.
    //It controls bridge material color whan capture and loose bridges.
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Animator))]
    public class BridgeBehaviour : MonoBehaviour
    {
        public GameObject myEffect;
        public GameObject enemyEffect;
        public AudioClip mySoundEffect;
        public AudioClip enemySoundEffect;
        private OnBridgeCapturingEffectsManager effectsManager;

        private void Start()
        {
            effectsManager = FindObjectOfType<OnBridgeCapturingEffectsManager>();
        }

        internal void SetSide(BattlePlayerSide side, BridgeSide bridgeSide, bool firstTime)
        {
            GameObject effect = null;
            AudioClip sound = null;
            string triggerName = "Null";
            switch (side)
            {
                case BattlePlayerSide.None:
                    effect = null;
                    sound = null;
                    triggerName = "Null";
                    break;
                case BattlePlayerSide.Left:
                    triggerName = "Ally";
                    sound = mySoundEffect;
                    //BattleInstanceInterface.instance.StartCoroutine(BattleInstanceInterface.instance.BridgeSkill());
                    effectsManager.StartSequenceOfEffectsPlayingOnBridgeCapturing(bridgeSide == BridgeSide.Top, false);
                    effect = myEffect;
                    break;
                case BattlePlayerSide.Right:
                    triggerName = "Enemy";
                    sound = enemySoundEffect;
                    effect = enemyEffect;
                    effectsManager.StartSequenceOfEffectsPlayingOnBridgeCapturing(bridgeSide == BridgeSide.Top, true);
                    break;
                default:
                    break;
            }
            if (effect)
            {
                effect.SetActive(true);
                effect.GetComponent<ParticleSystem>().Play();
            }
            PlayClip(sound);
            GetComponent<Animator>().ResetTrigger("Ally");
            GetComponent<Animator>().ResetTrigger("Enemy");
            GetComponent<Animator>().ResetTrigger("Null");
            GetComponent<Animator>().SetTrigger(triggerName);
        }

        private void PlayClip(AudioClip clip)
        {
            GetComponent<AudioSource>().clip = clip;
            GetComponent<AudioSource>().Play();
        }
    }
}
