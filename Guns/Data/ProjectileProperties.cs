using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ProjectileProperties
{
    public float Mass;
    public float HitPoints;
    public Vector2 Scale;
    public float Damage;
    public float Gravity;
    public float Knockback;
    public Vector2 LinearDrag;
    public float MaxLifeTime;
    public float MinVelocityMagnitude;

    public ProjectileProperties(float gravity, Vector2 linearDrag, float minVelocityMagnitude, float maxLifeTime, float knockback, float damage, Vector2 scale, float mass, float hitPoints)
    {
        Mass = mass;
        Scale = scale;
        Damage = damage;
        Gravity = gravity;
        Knockback = knockback;
        HitPoints = hitPoints;
        LinearDrag = linearDrag;
        MaxLifeTime = maxLifeTime;
        MinVelocityMagnitude = minVelocityMagnitude;
    }
}
