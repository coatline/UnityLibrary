using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] float lagBehindAmount;
    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        transform.position = Extensions.MultiplyVector3s(cam.transform.position, new Vector3(1 / lagBehindAmount, 1 / lagBehindAmount, 0));
    }
}
