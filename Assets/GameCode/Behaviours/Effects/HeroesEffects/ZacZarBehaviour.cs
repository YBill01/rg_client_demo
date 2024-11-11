using DG.Tweening;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Legacy.Client
{
    public class ZacZarBehaviour : MonoBehaviour
    {
        void Start()
        {
            GetComponent<RangeHitEffect>().SecondBulletDelay = 0.300f;
            SetTimers();
        }

        //
        //trap skill
        //

        public List<GameObject> trapsContainer;

        [SerializeField] private AnimationCurve speedCurve;
        [SerializeField] private Transform StartTrapsPosition;
        [SerializeField] private float timetoflyTraps = 1.25f;
        [SerializeField] private float maxYHeight = 3.5f;
        [SerializeField] private ushort db = 116;

        private List<GameObject> spawningTrapMinions = new List<GameObject>();
        private List<Vector3> EndTrapPoints;
        private Vector3 tempVector = Vector3.zero;
        private float allTimeOfFlyTraps = 0;
        private float timer = 1;
        private bool canThrow = false;


        public void SetEndTargetPosition(List<Vector3> points)
        {
            spawningTrapMinions = new List<GameObject>();
            EndTrapPoints = points;
        }

        public void TrapThrow()//anim
        {
            SetTimers();
            GetTraps();

            foreach (var trapContainer in trapsContainer)
            {
                trapContainer.transform.position = StartTrapsPosition.position;
                trapContainer.GetComponentInChildren<TrapsRotation>(true).gameObject.SetActive(true);
            }

            canThrow = true;
        }

        private void Update()
        {
            if (canThrow)
            {
                MoveTraps();

                if (timer < 0)
                {
                    BackMinionToPool();
                    DisableTraps();

                    canThrow = false;
                }
                timer -= Time.deltaTime;
            }
        }

        private void BackMinionToPool()
        {
            for (int i = 0; i < spawningTrapMinions.Count; i++)
            {
                ObjectPooler.instance.MinionBack(spawningTrapMinions[i]);
            }
        }

        private void MoveTraps()
        {
            if (EndTrapPoints != null)
            {
                for (int i = 0; i < trapsContainer.Count; i++)
                {
                    var trapContainer = trapsContainer[i].transform;

                    var trapHeight = Mathf.Lerp(0, maxYHeight, Mathf.Sin((1 - GetTimePersentage(timer)) * Mathf.PI));
                    EndTrapPoints[i] = GetTempVector3(EndTrapPoints[i].x, trapHeight, EndTrapPoints[i].z);

                    trapContainer.position = Vector3.Lerp(StartTrapsPosition.position, EndTrapPoints[i], 1 - GetTimePersentage(timer));
                }
            }
        }

        private void DisableTraps()
        {
            foreach (var trapContainer in trapsContainer)
            {
                trapContainer.GetComponentInChildren<TrapsRotation>(true).gameObject.SetActive(false);
            }

            EndTrapPoints = null;
            spawningTrapMinions.Clear();
        }

        private Vector3 GetTempVector3(float x, float y, float z)
        {
            tempVector.x = x;
            tempVector.y = y;
            tempVector.z = z;

            return tempVector;
        }

        private void GetTraps()
        {
            var _visualization = ClientWorld.Instance.GetOrCreateSystem<MinionGameObjectInitializationSystem>();
            for (int i = 0; i < trapsContainer.Count; i++)
            {
                CreateEntityPrefab(db, trapsContainer[i].transform, (objFromPool) =>
                {
                    spawningTrapMinions.Add(objFromPool);
                    _visualization.Spawned(db, objFromPool);
                });
            }
        }

        private void CreateEntityPrefab(ushort DBEntity, Transform i, Action<GameObject> callback)
        {
            if (Entities.Instance.Get(DBEntity, out BinaryEntity entity))
            {
                ObjectPooler.instance.GetMinion(entity, (objFromPool) =>
                {
                    objFromPool.SetActive(true);
                    objFromPool.GetComponent<MinionInitBehaviour>().DoMinionVisible();
                    var isEnemy = GetComponent<MinionInitBehaviour>().IsEnemy;
                    objFromPool.GetComponent<MinionInitBehaviour>().InitTeamColor(isEnemy);

                    objFromPool.transform.SetParent(i);
                    objFromPool.transform.localPosition = Vector3.zero;

                    AnimatorParams(objFromPool);

                    callback(objFromPool);
                });
            }
        }

        private void AnimatorParams(GameObject objFromPool)
        {
            var animator = objFromPool.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = true;
                animator.ResetBools();
                animator.SetBool("Stand", true);
            }
        }

        private float GetTimePersentage(float remainTime)
        {
            var t = remainTime / allTimeOfFlyTraps;
            return t;
        }

        private void SetTimers()
        {
            timer = timetoflyTraps;
            allTimeOfFlyTraps = timetoflyTraps;
        }

        //
        //axe skill
        //

        [SerializeField] private Transform CustomStartAxePoint;
        [SerializeField] private Transform Axe;
        [SerializeField] private float timetoflyAxe = 1.8f;
        //мосты через статикколлайдерс // разбраться и взять нормальную позицию
        private Vector3 TargetHeroPoint = new Vector3(0, 1, 12f);
        private Vector3 bridgeup = new Vector3(0, 1, -5.5f);
        private Vector3 bridgedown = new Vector3(0, 1, 5.5f);

        public void SetAxeTargetHeroPosition(Vector3 targetHero)
        {
            TargetHeroPoint = -targetHero;
        }

        public void ThrowAxe()//anim
        {
            Axe.gameObject.SetActive(true);

            Axe.position = CustomStartAxePoint.position;

            TargetHeroPoint = -this.transform.position;
            TargetHeroPoint.y = 1;

            var path = new Vector3[] { bridgeup, TargetHeroPoint, bridgedown, CustomStartAxePoint.position };

            var tween = Axe.DOPath(path, timetoflyAxe, PathType.CatmullRom, PathMode.Full3D, 10, Color.blue)
                           .SetEase(speedCurve);

            tween.OnComplete(() => { Axe.gameObject.SetActive(false); });
        }

    }
}
