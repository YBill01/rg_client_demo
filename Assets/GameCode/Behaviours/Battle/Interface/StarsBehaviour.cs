using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsBehaviour : MonoBehaviour
{
    public BattleStarBehaviour[] Stars;
    internal void Explosion(byte v)
    {
        Stars[v - 1].Explosion();
    }
}
