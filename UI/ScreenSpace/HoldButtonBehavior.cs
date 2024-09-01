using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HoldButtonBehavior : MonoBehaviour
{
    public event System.Action HoldingComplete;

    [SerializeField] InputAction holdAction;
    [SerializeField] TMP_Text skippingText;
    [SerializeField] Image fillImage;

    float fillSpeed;
    bool complete;
    bool holding;
    bool destroy;

    private void Update()
    {
        if (complete)
        {
            fillImage.color -= new Color(0, 0, 0, Time.deltaTime);

            if (fillImage.color.a <= 0 && destroy)
                Destroy(gameObject);
            return;
        }

        if (holding)
            fillImage.fillAmount += Time.deltaTime * fillSpeed;
        else
            fillImage.fillAmount -= Time.deltaTime * fillSpeed * 2;

        if (fillImage.fillAmount >= 1)
        {
            complete = true;
            HoldingComplete?.Invoke();
        }
    }

    public void BeginListening(float fillSpeed)
    {
        holdAction.performed += StartedHolding;
        holdAction.canceled += StoppedHolding;

        fillImage.color = Color.white;
        fillImage.fillAmount = 0;
        complete = false;

        holdAction.Enable();

        this.fillSpeed = fillSpeed;
    }

    public void StopListening(bool destroy)
    {
        if (destroy)
        {
            complete = true;
            this.destroy = true;
        }

        holdAction.Disable();
    }

    void StartedHolding(InputAction.CallbackContext s)
    {
        holding = true;

        if (skippingText)
            skippingText.enabled = true;
    }

    void StoppedHolding(InputAction.CallbackContext s)
    {
        holding = false;

        if (skippingText)
            skippingText.enabled = false;
    }

    private void OnDestroy()
    {
        holdAction.Dispose();
    }
}
