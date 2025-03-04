using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [SerializeField] ShaderEffectController shaderController;
    [SerializeField] float flashSpeed = 5;

    Coroutine flashRoutine;
    float flashAmount;
    float actualFlash;
    float percentage;

    public void Flash(float duration)
    {
        if (gameObject.activeInHierarchy == false) return;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashAmount += duration;

        flashRoutine = StartCoroutine(FlashRoutine(duration));
    }

    private IEnumerator FlashRoutine(float duration)
    {
        percentage = 0;

        while (percentage < 1)
        {
            percentage += Time.deltaTime / duration;
            flashAmount = Mathf.Abs(Mathf.Sin(percentage * Mathf.PI) * 10);
            shaderController.SetHitEffect(flashAmount);
            yield return null;
        }

        shaderController.SetHitEffect(0);
        flashRoutine = null;
    }

    public void StopFlashing()
    {
        actualFlash = 0;
        flashAmount = 0;
        shaderController.SetHitEffect(0);

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = null;
    }
}