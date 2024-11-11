using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{

    private Canvas canvas;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    void Update()
    {
        if(canvas.worldCamera != Camera.main)
            canvas.worldCamera = Camera.main;        
    }
}
