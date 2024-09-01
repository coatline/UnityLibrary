using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ProjectileProperties
{
    public Item SourceItem;
    public float Force;
    public float Gravity;
    public Vector2 LinearDrag;
    public float MinVelocityMagnitude;
    public float MaxLifeTime;
    public float Knockback;
    public float Damage;
    public Vector2 Scale;

    public ProjectileProperties(Item sourceItem, float force, float gravity, Vector2 linearDrag, float minVelocityMagnitude, float maxLifeTime, float knockback, float damage, Vector2 scale)
    {
        SourceItem = sourceItem;
        Force = force;
        Gravity = gravity;
        LinearDrag = linearDrag;
        MinVelocityMagnitude = minVelocityMagnitude;
        MaxLifeTime = maxLifeTime;
        Knockback = knockback;
        Damage = damage;
        Scale = scale;
    }
}
