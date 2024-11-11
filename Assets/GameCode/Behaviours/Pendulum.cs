using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float Limit = 100f;
    public float Speed = 1f;
    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        var r = transform.rotation;
        var e = r.eulerAngles;
        e.z = Mathf.Sin(Time.time * Speed) * Limit;
        r.eulerAngles = e;
        transform.rotation = r;
    }
}
