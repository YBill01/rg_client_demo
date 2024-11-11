using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class WaveProgressScript : MonoBehaviour
    {
        public float waveSize;
        [SerializeField]
        private float progress;

        private Transform unitContainer;

        public float Progress { get { return 1; } set { progress = value; UpdateView(); } }

        private Material material;
        private MeshRenderer meshRenderer;
        void Start()
        {
            unitContainer = transform.parent;
        }

        private bool _started;
        public void StartWave()
        {
            _started = true;
            //plaguePrefab.SetActive(true);
            wavePrefab = ObjectPooler.instance.GetEffect(smallEffectName);
            wavePrefab.SetActive(true);

            meshRenderer = wavePrefab.GetComponent<MeshRenderer>();
            material = meshRenderer.material;
        }

        public void StopWave()
        {
            _started = false;
            walkScripts.Clear();
            targeted.Clear();
            if (wavePrefab == null) return;
            wavePrefab.SetActive(false);
        }

        private void UpdateView()
        {
            material.SetFloat("_Progress", progress);
        }

        private List<MinionInitBehaviour> walkScripts = new List<MinionInitBehaviour>();
        private List<MinionInitBehaviour> targeted = new List<MinionInitBehaviour>();
        void Update()
        {
            if (!_started) return;

            var list = unitContainer.GetComponentsInChildren<MinionInitBehaviour>();
            float currentx = (progress * 24) - 12;
            foreach (var w in list)
            {
                if (walkScripts.Contains(w)) continue;
                if (w.transform.localPosition.x > currentx)
                    walkScripts.Add(w);
            }
            foreach (var w in walkScripts)
            {
                if (w.transform.localPosition.x < currentx)
                {
                    if (!targeted.Contains(w))
                    {
                        targeted.Add(w);
                        AddPlagueOBject(w.transform);
                    }
                }
            }
        }

        [SerializeField]
        private string smallEffectName;

        [SerializeField]
        private string waveName;

        private GameObject wavePrefab;

        private GameObject plaguePrefab;
        private void AddPlagueOBject(Transform t)
        {
            Vector3 vfxPosition = t.localPosition;
            vfxPosition.y = 1f;
            var vfxInstance = ObjectPooler.instance.GetEffect(smallEffectName);
            vfxInstance.SetActive(true);
            vfxInstance.transform.localPosition = vfxPosition;

            plaguePrefab = vfxInstance;
        }
    }
}
