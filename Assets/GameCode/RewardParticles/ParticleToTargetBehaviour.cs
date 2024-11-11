using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class ParticleToTargetBehaviour : MonoBehaviour
    {

        [SerializeField] GameObject particlePrefab;
        [SerializeField] ParticleSystem salute;
        [SerializeField] RectTransform saluteRect;

        [Space]
        [Header("Paticles pool")]
        [SerializeField] ushort maxCount;
        Queue<GameObject> particlesQueue = new Queue<GameObject>();

        [Space]
        [Header("Target UI RectTransform")]
        [SerializeField] TargetParticlesBehaviour particlesTarget;

        [Space]
        [SerializeField, Tooltip("Scale of particles.")] RandomBetweenMinMax AdditionalScale;

        [Header("Drop animation settins")]
        [SerializeField, Tooltip("Time to drop particles in seconds.")] RandomBetweenMinMax DropDuration;
        [SerializeField, Tooltip("Drop Ease type.")] Ease dropEaseType;
        [SerializeField, Range(0.0f, 1.0f), Tooltip("Spread position by X.")] float spreadX;
        [SerializeField, Range(0.0f, 1.0f), Tooltip("Spread position by Y.")] float spreadY;

        [Header("ToTarget Fly animation")]
        [SerializeField, Range(0.0f, 3.0f), Tooltip("Time to wait after drop before start fly to target in seconds.")] float intervalAfterDrop;
        [SerializeField, Range(0, 100), Tooltip("Deviation intensity from line between particle and target.")] byte curveIntensity;
        [SerializeField, Range(2, 20), Tooltip("Number of waypoints in path to target.")] int wayPointsCount = 3;

        [SerializeField, Tooltip("Time to fly to target after drop in seconds.")] RandomBetweenMinMax FlyAnimDuration;
        byte fadeTimePart = 5;
        [SerializeField, Tooltip("Flying Ease type.")] Ease flyingEaseType;

        [Header("Trail Settings")]
        [SerializeField, Tooltip("Width of trail.")] RandomBetweenMinMax trailWidth;
        [SerializeField, Range(0.0f, 1.0f), Tooltip("Ratio of trails that will be enabled.")] float trailRatio;

        [Header("Sounds")]
        [SerializeField] private AudioClip particlesAppear;
        [SerializeField] private AudioClip particlesFly;
        

        Vector3 targetPosition;

        byte completedParticles = 0;
        byte startedParticles = 0;

        AudioSource audioSource;

        public bool Finished => completedParticles == startedParticles;

        [Serializable]
        public struct RandomBetweenMinMax
        {
            public float min;
            public float max;

            public float GetNextRandom()
            {
                return UnityEngine.Random.Range(min, max);
            }
        }

        public ProgressBarChangeValueBehaviour GetTargetProgressBar()
        {
            return particlesTarget.GetComponentInChildren<ProgressBarChangeValueBehaviour>();
        }

        void Start()
        {            
            CreateQueue();
            audioSource = GetComponent<AudioSource>();
        }

        void CreateQueue()
        {
            for (byte i = 0; i < maxCount; i++)
            {
                GameObject particle = Instantiate(particlePrefab, transform);                
                particle.SetActive(false);
                particlesQueue.Enqueue(particle);
            }
        }

        internal void DropParticles(Vector3 collectedPosition, byte count, UnityEvent OnParticleCame)
        {
            if (salute != null)
            {
                saluteRect.position = collectedPosition;
                salute.Play();
            }
            targetPosition = particlesTarget.GetTargetPosition();
            byte _count = (byte)Mathf.Min(maxCount, count, particlesQueue.Count);
            startedParticles = _count;
            completedParticles = 0;

            Vector3 mainDirection = targetPosition - collectedPosition;
            if(_count > 0)
            {
                //говорим валюте, в которую полетят партиклы, чтобы ждала партиклов и пока не менялась
                particlesTarget.WaitForParticles(_count);
            }

            for (byte i = 0; i < _count; i++)
            {
                Vector3 spreadPosition = collectedPosition + new Vector3(
                    UnityEngine.Random.Range(-spreadX, spreadX),
                    UnityEngine.Random.Range(-spreadY, spreadY), 
                    0
                );

                GameObject particle = particlesQueue.Dequeue();
                var rect = particle.GetComponent<RectTransform>();

                TrailRenderer trail = particle.GetComponent<TrailRenderer>();
                DisableTrail(trail);

                rect.localScale = Vector3.zero;
                rect.position = collectedPosition;
                particle.SetActive(true);

                Vector3 droppedScale = RandomizeScale();

                float angle = Vector3.SignedAngle(mainDirection, targetPosition - spreadPosition, Vector3.forward);
                Vector3[] path = GeneratePath(spreadPosition, angle);

                float dropDuration = DropDuration.GetNextRandom();
                float flyDuration = FlyAnimDuration.GetNextRandom();
                float fullTime = dropDuration + intervalAfterDrop + flyDuration;
                float endTime = flyDuration / fadeTimePart;
                float timeBeforeFadeOut = fullTime - endTime;

                DOTween.Sequence()
                    .Append(rect.DOMove(spreadPosition, dropDuration).SetEase(dropEaseType)
                        .OnStart(()=> { PlayClip(particlesAppear);  }))
                    .Join(rect.DOScale(droppedScale, dropDuration))                    
                    .AppendInterval(intervalAfterDrop)
                    .Append(rect.DOPath(path, flyDuration, PathType.CatmullRom, PathMode.Ignore).SetEase(flyingEaseType)
                        .OnStart(() => {
                            StartCoroutine(EnableTrail(trail));                            
                            PlayClip(particlesFly); 
                            MinionsSoundsManager.AddSourceToList(audioSource);  
                        }))
                    .Join(rect.DOScale(Vector3.one, flyDuration))
                    .Insert(timeBeforeFadeOut, rect.DOScale(0.5f, endTime).OnStart(() => DisableTrail(trail)))
                    .OnComplete(() => {                        
                        particlesTarget.ParticleCame(OnParticleCame);
                        particle.SetActive(false);
                        particlesQueue.Enqueue(particle);
                        completedParticles++;
                        MinionsSoundsManager.RemoveSourceFromList(audioSource);
                    });                
            }
        }

        private void DisableTrail(TrailRenderer trail)
        {
            if (trail != null)
            {
                trail.enabled = false;
                trail.emitting = false;
            }
        }

        private IEnumerator EnableTrail(TrailRenderer trail)
        {
            yield return new WaitForSeconds(0.1f);
            if (trail != null)
            {
                float random = UnityEngine.Random.Range(0.0f, 1.0f);
                if (random < trailRatio)
                {
                    trail.widthMultiplier = trailWidth.GetNextRandom();
                    trail.enabled = true;
                    trail.emitting = true;
                }
            }
        }

        private Vector3[] GeneratePath(Vector3 position, float angle)
        {
            Vector3 localDirection = targetPosition - position;

            Vector3 perp;
            if (angle < 0)
            {
                //Полетят слева от оси (ось - линия от начала пути до таргета)
                perp = new Vector3(-localDirection.y, localDirection.x, 0);
            }
            else
            {
                //Полетят справа от оси (ось - линия от начала пути до таргета)
                perp = new Vector3(localDirection.y, -localDirection.x, 0);
            }
            float offsetMultiplier = (float)curveIntensity / 10.0f;
            Vector3 OffsetVector = (position + perp).normalized * offsetMultiplier;

            Vector3[] path = new Vector3[wayPointsCount + 1];
            float step = 1.0f / wayPointsCount;
            float currentLerpStep = step;
            byte i;
            for(i = 0; i < wayPointsCount; i++)
            {
                path[i] = GenerateWayPoint(position, OffsetVector, currentLerpStep);
                currentLerpStep += step;
            }
            path[path.Length - 1] = GenerateWayPoint(targetPosition, Vector3.zero, 1.0f);
            return path;
        }

        private Vector3 GenerateWayPoint(Vector3 position, Vector3 offsetVector, float lerpStep)
        {
            float multiplier = 1.0f - lerpStep;
            return Vector3.Lerp(position, targetPosition, lerpStep) + offsetVector * multiplier;
        }

        private Vector3 RandomizeScale()
        {
            float scaleValue = 1.0f + AdditionalScale.GetNextRandom();
            return new Vector3(scaleValue, scaleValue, scaleValue);
        }

        private void PlayClip(AudioClip clip)
        {
            GetComponent<AudioSource>().clip = clip;
            GetComponent<AudioSource>().Play();
        }
    }
}
