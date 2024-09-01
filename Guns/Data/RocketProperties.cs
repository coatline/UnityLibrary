using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RocketProperties
{
    [SerializeField] int destructionRadius;
    [SerializeField] float startupDuration;
    [SerializeField] float acceleration;
    [SerializeField] float startupSpeed;
    [SerializeField] float maxSpeed;

    public float StartupSpeed { get { return startupSpeed; } }
    public float Acceleration { get { return acceleration; } }
    public float StartupDuration { get { return startupDuration; } }
    public int DestructionRadius { get { return destructionRadius; } }
    public float MaxSpeed { get { return maxSpeed; } }
}
