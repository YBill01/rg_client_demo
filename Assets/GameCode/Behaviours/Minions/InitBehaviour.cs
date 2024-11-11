using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitBehaviour : MonoBehaviour
{
    public float SpaWnHeight;
    virtual public void InitMaterial(bool isEnemy) { }
    virtual public void Init() { }
    virtual public void Spawn() { }
    virtual public void DoMinionInvisible() { }
    virtual public void DoMinionVisible() { }
}
