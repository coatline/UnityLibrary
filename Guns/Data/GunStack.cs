using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunStack
{
    public event System.Action ShotsGone;
    public event System.Action Shot;
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
    public readonly int MaxShots;

    public readonly Gun GunType;
    public GunStack(Gun gun)
    {
        GunType = gun;
        MaxShots = gun.ShotsPerClip;
        ShotsRemaining = MaxShots;
    }

    public bool TryShoot()
    {
        if (ShotsRemaining <= 0) return false;

        ShotsRemaining--;
        Shot?.Invoke();
        return true;
    }

    public bool FullyReloaded => shotsRemaining == MaxShots;
    public void Reload() => ShotsRemaining++;
    public void FullReload() => ShotsRemaining = MaxShots;
}
