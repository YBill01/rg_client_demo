using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMainCamera : MonoBehaviour
{
    private Camera mainCam;
    private Camera me;
    void Start()
    {
        mainCam = Camera.main;
        me = GetComponentsInChildren<Camera>()[0];
        me.fieldOfView = mainCam.fieldOfView;
        me.nearClipPlane = mainCam.nearClipPlane;
        me.farClipPlane = mainCam.farClipPlane;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localPosition != mainCam.transform.position)
            transform.localPosition = mainCam.transform.position;
        if (transform.localRotation != mainCam.transform.rotation)
            transform.localRotation = mainCam.transform.rotation;
    }
}
