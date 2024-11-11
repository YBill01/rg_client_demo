using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class WaveEffectBehaviour : MonoBehaviour
    {
        private Material material;
        void Start()
        {
            if (material == null)
            {
                var meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
                foreach (var m in meshRenderers)
                    material = m.material;
            }
        }

        void Update()
        {
            if (!inited) return;
            if (!transformUpdated)
            {
                if (diffx != 0)
                {
                    facktx += diffx;
                }
            }
            transformUpdated = false;
            UpdateInfront();
            UpdateTargeted();
        }

        private void UpdateInfront()
        {
            foreach (var w in allenemies)
            {
                bool isInfront = false;
                if (direction == 1)
                {
                    if (facktx < w.localPosition.x)
                        isInfront = true;
                }
                else
                {
                    if (facktx > w.localPosition.x)
                        isInfront = true;
                }
                if (!isInfront) continue;
                infront.Add(w);
            }
        }

        private void UpdateTargeted()
        {
            foreach (var w in infront)
            {
                bool isInfront = false;
                if (direction == 1)
                {
                    if (facktx < w.localPosition.x)
                        isInfront = true;
                }
                else
                {
                    if (facktx > w.localPosition.x)
                        isInfront = true;
                }
                if (isInfront) continue;
                if (targeted.Contains(w)) continue;
                targeted.Add(w);
                AddPlagueOBject(w);
            }
        }

        private void AddPlagueOBject(Transform t)
        {
            Vector3 vfxPosition = t.localPosition;
            vfxPosition.y = 1.5f;
            var vfxInstance = ObjectPooler.instance.GetEffect(EffectName);
            var pscb = vfxInstance.gameObject.GetComponent<ParticleSystemControlBehaviour>();
            vfxInstance.SetActive(true);
            vfxInstance.transform.localPosition = vfxPosition;
            if (pscb != null)
                pscb.followTransform = t;
        }

        private int direction;
        public void SetDirection(int direction)
        {
            inited = false;
            this.direction = direction;
            var s = transform.localScale;
            s.x = (Mathf.Abs(s.x)) * direction;
            transform.localScale = s;
            if (material == null)
            {
                var meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
                foreach (var m in meshRenderers)
                    material = m.material;
            }
            if (material == null) return;
            material.SetFloat("_Progress", 0);
        }

        public const float heroPosition = 12.5f;
        private bool inited;
        private float prevx;
        private float facktx;
        private float diffx;
        private bool transformUpdated;
        public void SetPosition(float x)
        {
            if (!inited)
            {
                if (x != 0)
                    inited = true;
            }
            if (!inited) return;
            transformUpdated = true;
            if (prevx == x)
            {
                facktx += diffx;
            }
            else
            {
                facktx = x;
                prevx = x;
            }
            if (direction == 1)
            {
                float path = (facktx + heroPosition);
                material.SetFloat("_Progress", path / (2 * heroPosition));
            }
            else
            {
                float path = (heroPosition - facktx);
                material.SetFloat("_Progress", path / (2 * heroPosition));
            }
        }

        public string EffectName;
        private List<Transform> allenemies = new List<Transform>();
        private List<Transform> targeted = new List<Transform>();
        private List<Transform> infront = new List<Transform>();
        public void AddEnemy(Transform transform)
        {
            if (allenemies.Contains(transform)) return;
            allenemies.Add(transform);
        }

        public void OnDisable()
        {
            allenemies.Clear();
            targeted.Clear();
            infront.Clear();
            inited = false;

            if (material == null)
            {
                var meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
                foreach (var m in meshRenderers)
                    material = m.material;
            }
            if (material == null) return;
            material.SetFloat("_Progress", 0);
        }

        private void OnEnable()
        {
            inited = false;
            if (material == null)
            {
                var meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
                foreach (var m in meshRenderers)
                    material = m.material;
            }
            if (material == null) return;
            material.SetFloat("_Progress", 0);
        }

        private void OnDestroy()
        {
            allenemies.Clear();
            targeted.Clear();
            infront.Clear();
        }

    }
}
