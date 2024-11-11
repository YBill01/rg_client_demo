using DG.Tweening;
using UnityEngine;

namespace Legacy.Client
{
    public class DamageEffect : MonoBehaviour
    {
        [SerializeField] private bool shouldBump = true;
        public GameObject DefaultDamagePoint;
        public GameObject CustomDamagePoint;
        public GameObject DefaultEffect;
        private ParticleSystem DamageParticles;

        private bool isEnemy;
        private bool isHero;
        private MinionInitBehaviour         minionInitBehaviour;
        private MinionMaterialsBehaviour    minionMaterialsBehaviour;

        [SerializeField] Vector3 PunchPower;
        [SerializeField] Ease PunchEase;
        [SerializeField] float PunchDuration;

        internal void SetHeroEnemy(bool isHero, bool isEnemy)
        {
            this.isEnemy = isEnemy;
            this.isHero = isHero;
        }

        private void Start()
        {
            minionMaterialsBehaviour = GetComponent<MinionMaterialsBehaviour>();
            minionInitBehaviour      = GetComponent<MinionInitBehaviour>();
        }

        private void OnEnable()
        {
            if (DefaultEffect != null)
            {
                DefaultEffect.SetActive(false);
                DamageParticles = DefaultEffect.GetComponent<ParticleSystem>();
            }
            isEnemy = GetComponent<MinionPanel>().IsEnemy;
        }

        private void PlayDamageEffect()
        {
            if (DefaultEffect)
            {
                DefaultEffect.transform.position = GetDamagePoint().position;
                DefaultEffect.SetActive(true);
                if (DefaultEffect.GetComponent<ParticleSystem>()) DefaultEffect.GetComponent<ParticleSystem>().Play();
            }
        }

        internal Transform GetDamagePoint()
        {
            if (CustomDamagePoint != null)
            {
                return CustomDamagePoint.transform;
            }
            else
            {
                return DefaultDamagePoint.transform;
            }
        }

        

        public void Punch()
        {
            if (shouldBump)
            {
                if (DefaultEffect) DefaultEffect.SetActive(false);
                PlayDamageEffect();
                minionMaterialsBehaviour.SetDamageMaterials(minionInitBehaviour.IsEnemy);
                transform.DOKill(true);
                transform.DOPunchScale(PunchPower, PunchDuration, 1, 0)
                    .SetEase(PunchEase)
                    .OnComplete(() =>
                    {
                        minionMaterialsBehaviour.SetDefaultMaterials();
                    });
            }
        }
    }
}
