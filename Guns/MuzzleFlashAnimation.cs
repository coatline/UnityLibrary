using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlashAnimation : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;

    float speed;

    public void Flash(float speed, float size, Color color)
    {
        this.speed = speed;

        sr.transform.localScale = Vector3.one * size;
        sr.color = color;
    }

    void Update()
    {
        if (sr.color.a > 0)
        {
            //alpha -= Time.deltaTime * speed;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - Time.deltaTime * speed);
        }
    }

    private void OnEnable()
    {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
    }
}
