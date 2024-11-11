using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZacZarAxe : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] private float angle = 500f;
    [SerializeField] private Vector3 axis = Vector3.right;
    void Update()
    {
        transform.Rotate(axis, angle);
    }
}
