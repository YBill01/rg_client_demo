using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DragRadiusScale : MonoBehaviour
{

    [SerializeField] private Transform scaleGO;

    private void Start()
    {
      //  entityManager = ClientWorld.Instance.EntityManager;
        if (!scaleGO)
            scaleGO = this.transform;
    }  

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    internal void Scale(float v)
    {
        scaleGO.localScale = new Vector3(v, v, v);
    }

    //protected void HighlightZone()
    //{
    //    zoneHighlight = ClientWorld.Instance.EntityManager.CreateEntity();
    //    ClientWorld.Instance.EntityManager.AddComponentData(zoneHighlight, new ArenaZoneHighlight { allZone = true });
    //}

    //protected void PutOutHighlightZone()
    //{
    //    ClientWorld.Instance.EntityManager.DestroyEntity(zoneHighlight);
    //}
}
