using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadVisuals : MonoBehaviour
{
    [SerializeField] BarAnimation reloadBarPrefab;
    [SerializeField] ReloadBehavior behavior;
    [SerializeField] Transform position;
    [SerializeField] Color autoReloadingColor;
    GunStack gunStack;
    BarAnimation bar;

    void Start()
    {
        CreateBar();

        behavior.Shot += Shot;
        behavior.Reloaded += OnOneNormalReload;
        behavior.OnAutoReloading += OnAutoReloading;
        behavior.StartedAutoReload += OnStartedAutoReload;
        behavior.AutoReloadComplete += OnAutoReloadComplete;
    }

    void OnOneNormalReload()
    {
        bar.UpdateFillAndFlash(gunStack.ShotsRemaining, gunStack.MaxShots);
    }

    void OnStartedAutoReload()
    {
        bar.SetColor(autoReloadingColor);
    }

    void OnAutoReloadComplete()
    {
        bar.SetDefaultColor();
    }

    void OnAutoReloading(float timer)
    {
        bar.UpdateFill((timer + gunStack.ShotsRemaining), gunStack.MaxShots);
    }

    void Shot()
    {
        bar.UpdateFillAndFlash(gunStack.ShotsRemaining, gunStack.MaxShots);
    }

    private void Update()
    {
        bar.transform.position = position.position;
    }

    void CreateBar()
    {
        Canvas[] canvasas = FindObjectsOfType<Canvas>();
        Canvas c = null;

        foreach (Canvas canvas in canvasas)
            if (canvas.name == "WorldSpaceCanvas")
            {
                c = canvas;
                break;
            }

        bar = Instantiate(reloadBarPrefab, c.transform);
    }
}
