using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReloadBehavior : MonoBehaviour
{
    public event System.Action<float> OnAutoReloading;
    public event System.Action AutoReloadComplete;
    public event System.Action StartedAutoReload;
    public event System.Action Reloaded;
    public event System.Action Shot;

    [SerializeField] SpriteRenderer itemSR;

    public bool AutoReloading { get; private set; }
    public bool Reloading { get; private set; }

    float reloadOneBulletTime;
    float autoReloadTimer;

    GunStack gunStack;

    public void SetGun(GunStack gunStack)
    {
        if (this.gunStack != null)
        {
            this.gunStack.Shot -= Shot;
            this.gunStack.ShotsGone -= StartAutoReload;
        }

        this.gunStack = gunStack;

        InstantlyFullyReload();

        this.gunStack.Shot += Shot;
        this.gunStack.ShotsGone += StartAutoReload;
        reloadOneBulletTime = this.gunStack.GunType.FullReloadTime / this.gunStack.GunType.ShotsPerClip;
    }

    public void InstantlyFullyReload()
    {
        autoReloadTimer = 0;
        itemSR.color = Color.white;
        gunStack.FullReload();
        Reloading = false;
        AutoReloading = false;
        AutoReloadComplete?.Invoke();
    }

    void StartAutoReload()
    {
        if (AutoReloading)
            return;

        SoundManager.I.PlaySound(DataLibrary.I.Sounds["Auto Reload"], transform.position);

        itemSR.color = Color.white * 0.5f;
        AutoReloading = true;
        StartedAutoReload?.Invoke();
    }

    public void TryStartReloading()
    {
        if (gunStack != null && gunStack.FullyReloaded == false)
            StartAutoReload();
    }

    public void StopReloading()
    {
        Reloading = false;
    }

    void Update()
    {
        if (AutoReloading)
        {
            autoReloadTimer += Time.deltaTime;
            OnAutoReloading?.Invoke(autoReloadTimer);

            if (autoReloadTimer >= gunStack.GunType.FullReloadTime)
            {
                InstantlyFullyReload();
                SoundManager.I.PlaySound(DataLibrary.I.Sounds["Finished Auto Reloading"], transform.position);
            }
        }
    }

    private void OnDestroy()
    {
        if (gunStack != null)
        {
            gunStack.ShotsGone -= StartAutoReload;
            gunStack.Shot -= Shot;
        }
    }
}