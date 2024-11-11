using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SimplyRotator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    [SerializeField]
    private bool Active;
    [SerializeField]
    private float rotationSpeed;
    void Update()
    {

    }

    private void DoRotation()
    {
        if (!Active) return;
        var rt = transform.localRotation;
        var eu = rt.eulerAngles;
        eu.y += rotationSpeed * Time.deltaTime;
        rt.eulerAngles = eu;
        transform.localRotation = rt;
    }

    private void LateUpdate()
    {
        DoRotation();
    }
}
