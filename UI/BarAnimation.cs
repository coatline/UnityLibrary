using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarAnimation : MonoBehaviour
{
    [SerializeField] AnimationCurve catuchUpPercentCurve;
    [SerializeField] Color backgroundIncreaseColor;
    [SerializeField] float catchUpTime;
    [SerializeField] Image background;
    [SerializeField] Image bar;

    Color defaultColor;

    float targetPercentage;
    float lerpTimer;

    private void Awake()
    {
        defaultColor = bar.color;
    }

    private void Update()
    {
        if (background.fillAmount > targetPercentage)
        {
            bar.fillAmount = targetPercentage;
            background.color = Color.white;

            lerpTimer += Time.deltaTime;

            float percentComplete = catuchUpPercentCurve.Evaluate(lerpTimer / catchUpTime);
            background.fillAmount = Mathf.Lerp(background.fillAmount, targetPercentage, percentComplete);
        }
        else if (bar.fillAmount < targetPercentage)
        {
            background.fillAmount = targetPercentage;
            background.color = backgroundIncreaseColor;

            lerpTimer += Time.deltaTime;

            float percentComplete = catuchUpPercentCurve.Evaluate(lerpTimer / catchUpTime);
            bar.fillAmount = Mathf.Lerp(bar.fillAmount, targetPercentage, percentComplete);
        }
    }

    public void UpdateFill(float val, float max)
    {
        targetPercentage = (float)val / max;
        //bar.fillAmount = targetPercentage;
        //background.fillAmount = targetPercentage;
    }

    public void UpdateFillAndFlash(float val, float max)
    {
        targetPercentage = (float)val / max;
        lerpTimer = 0;
    }

    public void SetDefaultColor() => bar.color = defaultColor;
    public void SetColor(Color color) => bar.color = color;
}
