using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFlash : MonoBehaviour
{
    [Tooltip("GUI/Text")]
    [SerializeField] Material flashMaterial;

    [Tooltip("Duration of the flash.")]
    [SerializeField] float duration = .075f;

    [SerializeField] SpriteRenderer sr;
    [SerializeField] HealthHaver healthHaver;

    Material originalMaterial;

    Coroutine flashRoutine;

    void Start()
    {
        originalMaterial = sr.material;

        healthHaver.Damaged += () => { Flash(duration); };
        healthHaver.Respawned += Respawned;
    }

    void Respawned() => sr.material = originalMaterial;

    public void Flash(float duration)
    {
        if (gameObject.activeInHierarchy == false) return;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine(duration));
    }

    private IEnumerator FlashRoutine(float duration)
    {
        sr.material = flashMaterial;

        yield return new WaitForSeconds(duration);

        sr.material = originalMaterial;

        flashRoutine = null;
    }

    //public void StopFlashing()
    //{
    //    sr.material = originalMaterial;

    //    flashRoutine = null;

    //    if (flashRoutine != null)
    //        StopCoroutine(flashRoutine);
    //}
}
