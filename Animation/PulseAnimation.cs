using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseAnimation : MonoBehaviour
{
    [SerializeField] float pulseFrequency;
    [SerializeField] float pulseAmplitude;
    [SerializeField] bool random;

    Vector2 initialScale;
    float randOffset;

    private void Awake()
    {
        if (random)
            randOffset = Random.Range(0, 9999f);

        initialScale = transform.localScale;
    }

    void Update()
    {
        transform.localScale = initialScale + (Vector2.one * Mathf.Sin((Time.time + randOffset) * pulseFrequency) * pulseAmplitude);
    }
}
