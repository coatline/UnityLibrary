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

    [SerializeField] GunHolder itemHolder;
    [SerializeField] SpriteRenderer itemSR;

    public bool AutoReloading { get; private set; }
    public bool Reloading { get; private set; }

    float fullyReloadTime;
    float reloadOneBulletTime;
    float autoReloadTimer;

    GunStack gunStack;

    private void Start()
    {
        gunStack = itemHolder.GunStack;

        gunStack.Shot += Shot;
        gunStack.ShotsGone += StartAutoReload;
        fullyReloadTime = gunStack.GunType.FullReloadTime;
        reloadOneBulletTime = gunStack.GunType.FullReloadTime / gunStack.GunType.ShotsPerClip;
    }

    public void InstantlyFullyReload()
    {
        gunStack.FullReload();
        Reloading = false;
        AutoReloading = false;
        AutoReloadComplete?.Invoke();
    }

    void StartAutoReload()
    {
        if (AutoReloading) return;

        SoundManager.I.PlaySound(DataLibrary.I.Sounds["Auto Reload"], transform.position);

        itemSR.color = Color.white * 0.5f;
        AutoReloading = true;
        StartedAutoReload?.Invoke();
    }

    public void StartReloading()
    {
        StartAutoReload();

        //if (AutoReloading) return;

        //// If we are not fully reloaded, reload
        //if (gunStack.FullyReloaded == false && Reloading == false)
        //    StartCoroutine(Reload());
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

            if (autoReloadTimer >= fullyReloadTime)
            {
                autoReloadTimer = 0;
                AutoReloading = false;
                gunStack.FullReload();
                itemSR.color = Color.white;
                SoundManager.I.PlaySound(DataLibrary.I.Sounds["Finished Auto Reloading"], transform.position);
                AutoReloadComplete?.Invoke();
            }
        }
    }

    IEnumerator Reload()
    {
        Reloading = true;

        while (Reloading && gunStack.FullyReloaded == false)
        {
            yield return new WaitForSeconds(reloadOneBulletTime);

            if (Reloading)
            {
                gunStack.Reload();
                Reloaded?.Invoke();
            }
        }

        Reloading = false;
    }

    private void OnDestroy()
    {
        gunStack.ShotsGone -= StartAutoReload;
        gunStack.Shot -= Shot;
    }
}
