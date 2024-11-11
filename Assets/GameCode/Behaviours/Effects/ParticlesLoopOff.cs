using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesLoopOff : MonoBehaviour
{
    private float Timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            Timer += Time.deltaTime;
            if(Timer > 1)
            {
                var particles = GetComponentsInChildren<ParticleSystem>();
                for(var i = 0; i < particles.Length; i++)
                {
                    var particle = particles[i];
                    var main = particle.main;
                    main.loop = false;
                    Destroy(gameObject, 1f);
                }
                Timer = 0f;
            }
        }        
    }
}
