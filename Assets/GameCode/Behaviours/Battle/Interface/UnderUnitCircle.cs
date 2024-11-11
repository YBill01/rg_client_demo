using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderUnitCircle : MonoBehaviour
{
    public Transform Circle;
    public Transform DebugCollider;
    public Transform DebugAggro;

    private void OnEnable()
    {
        //Circle.localScale = Vector3.one;
        //DebugCollider.localScale = Vector3.one;
        DebugAggro.gameObject.SetActive(AppInitSettings.Instance.EnableColliders);
        DebugCollider.gameObject.SetActive(AppInitSettings.Instance.EnableColliders);
    }

    public void ScaleDebugCollider(float radius)
    {
        DebugCollider.localScale = new Vector3(radius, radius, radius);
    }

    public void ScaleDebugAggro(float radius)
    {
        DebugAggro.gameObject.SetActive(true);
        DebugAggro.localScale = new Vector3(radius, radius, radius);
    }
    public void DeScaleDebugAggro(float radius)
    {
        DebugAggro.gameObject.SetActive(false);
    }
}
