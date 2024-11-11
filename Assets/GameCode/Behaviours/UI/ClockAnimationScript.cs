using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockAnimationScript : MonoBehaviour
{
    public GameObject smallArrow;
    public GameObject bigArrow;
    public float smallArrowSpeed;
    public float bigArrowSpeed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;
        var erot1 = smallArrow.transform.rotation.eulerAngles;
        var rot1 = smallArrow.transform.rotation;
        erot1.z += delta * smallArrowSpeed;
        rot1.eulerAngles = erot1;
        smallArrow.transform.rotation = rot1;

        var erot2 = bigArrow.transform.rotation.eulerAngles;
        var rot2 = bigArrow.transform.rotation;
        erot2.z += delta * bigArrowSpeed;
        rot2.eulerAngles = erot2;
        bigArrow.transform.rotation = rot2;
    }
}
