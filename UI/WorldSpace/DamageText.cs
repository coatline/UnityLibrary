using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] AnimationCurve damageToRiseSpeedCurve;
    [SerializeField] AnimationCurve damageToSizeCurve;
    [SerializeField] AnimationCurve damageToLifeTimeCurve;
    [SerializeField] Gradient damageToColor;
    [SerializeField] TMP_Text text;

    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;

    [SerializeField] float minSize;
    [SerializeField] float maxSize;

    [SerializeField] float minLifetime;
    [SerializeField] float maxLifetime;

    Vector3 targetPosition;
    float riseSpeed;

    public void Setup(float damage)
    {
        text.text = damage.ToString();

        float percentage = damage / 50f;

        targetPosition = transform.position + new Vector3(0, 1, 0);

        riseSpeed = damageToRiseSpeedCurve.Evaluate(percentage) * (maxSpeed - minSpeed) + minSpeed;

        text.color = damageToColor.Evaluate(percentage);
        text.fontSize = damageToSizeCurve.Evaluate(percentage) * (maxSize - minSize) + minSize;

        Destroy(gameObject, damageToLifeTimeCurve.Evaluate(percentage) * (maxLifetime - minLifetime) + minLifetime);
    }

    void Update()
    {
        //riseSpeed -= Time.unscaledDeltaTime;

        transform.position = Vector3.Slerp(transform.position, targetPosition, riseSpeed * Time.deltaTime);
        //transform.Translate(new Vector3(0, riseSpeed * Time.deltaTime));
    }
}
