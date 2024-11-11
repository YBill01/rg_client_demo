using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToPool : MonoBehaviour
{
    public Transform parent;
    internal void Back()
    {        
        gameObject.SetActive(false);
    }
}
