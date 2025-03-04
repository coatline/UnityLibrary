using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] float followCameraAmountX;

    public Vector2 Position { get; set; }
    public float FollowCameraAmount => followCameraAmountX;

    Transform bottomLeft;
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
        Position = transform.localPosition;

        CameraBarriers b = FindFirstObjectByType<CameraBarriers>(FindObjectsInactive.Exclude);

        if (b != null)
            bottomLeft = b.BottomLeft;
    }

    void Update()
    {
        float deltaX = cam.transform.position.x * followCameraAmountX;
        float deltaY = cam.transform.position.y * followCameraAmountX;

        //if (bottomLeft != null)
        //    deltaY = ((cam.transform.position.y - C.GetCameraZoomRadius(cam, cam.orthographicSize).y) - bottomLeft.position.y) * followCameraAmountX;

        transform.localPosition = Position + new Vector2(deltaX, deltaY);
    }

    public void SetCameraFollowAmount(float amount)
    {
        followCameraAmountX = amount;
    }
}
