using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MinionHealthBar : MonoBehaviour
{
    public MinionPanel minionPanel;
    protected float colliderSize;
    public float Health { get; set; }

    public abstract void SetLevel(byte level);
    public abstract void DeActivateHealthBar();
    public abstract void ActivateHealthBar(bool flag = true);
    public abstract void SetValue(float value, bool shouldView = true, GameObject heroGameObject = null,MinionLayerType _layer = MinionLayerType.Ground);

    internal void SetCollider(float collider)
    {
        colliderSize = collider;
    }
}
