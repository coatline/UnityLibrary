using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunStack
{
    public event System.Action ShotsGone;
    public event System.Action Shot;

    public readonly int MaxShots;
    public readonly Gun GunType;

    public GunStack(Gun gun, float ammoMultiplier)
    {
        GunType = gun;
        MaxShots = Mathf.CeilToInt(gun.ShotsPerClip * ammoMultiplier);
        ShotsRemaining = MaxShots;
    }

    public bool TryShoot()
    {
        if (ShotsRemaining <= 0) return false;

        ShotsRemaining--;
        Shot?.Invoke();
        return true;
    }

    int shotsRemaining;
    public int ShotsRemaining
    {
        get => shotsRemaining;
        private set
        {
            if (shotsRemaining == value) return;
            shotsRemaining = value;

            if (value == 0)
                ShotsGone?.Invoke();
        }
    }

    public bool FullyReloaded => shotsRemaining == MaxShots;
    public void Reload() => ShotsRemaining++;
    public void FullReload() => ShotsRemaining = MaxShots;
}