using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecoilSettings
{
    [SerializeField] float recoilAmount;
    [SerializeField] float recoverySpeed;
    [SerializeField] float recoveryDelay;
    [SerializeField] Vector2 actualRecoilForce;
    [SerializeField] bool setVelocityToRecoil;

    public RecoilSettings(float recoilAmount, float recoverySpeed, float recoveryDelay, Vector2 actualRecoilForce, bool setVelocityToRecoil)
    {
        this.recoilAmount = recoilAmount;
        this.recoverySpeed = recoverySpeed;
        this.recoveryDelay = recoveryDelay;
        this.actualRecoilForce = actualRecoilForce;
        this.setVelocityToRecoil = setVelocityToRecoil;
    }

    public bool SetVelocityToRecoil => setVelocityToRecoil;
    public Vector2 ActualRecoilForce => actualRecoilForce;
    public float RecoilAmount => recoilAmount;
    public float RecoverySpeed => recoverySpeed;
    public float RecoveryDelay => recoveryDelay;
}