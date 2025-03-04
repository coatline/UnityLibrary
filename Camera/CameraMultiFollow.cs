using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMultiFollow : MonoBehaviour
{
    [SerializeField] List<Transform> followObjects;

    [Header("Movement")]
    [SerializeField] Vector2 offset;
    [SerializeField] float moveSmoothTime;
    [SerializeField] Transform topRightBorder;
    [SerializeField] Transform bottomLeftBorder;
    [SerializeField] bool dontOffsetBordersByZoom;
    [SerializeField] bool dontAutoSetMinMaxZoomAccordingToBorders;

    [Header("Zoom")]
    [SerializeField] float zoomInSpeed;
    [SerializeField] float zoomOutSpeed;
    [SerializeField] float maxZoom;
    [SerializeField] float minZoom;
    [SerializeField] float extraZoom;
    [SerializeField] Camera cam;
    [SerializeField] bool activeOnAwake;

    [SerializeField] bool doIncrementByPixels;
    [SerializeField] int ppu;

    public bool Active { get; set; }
    float unitPerPixel;

    Vector2 moveVelocity;
    float targetZoom;

    private void Awake()
    {
        if (activeOnAwake)
            Active = true;

        SetBarriers(bottomLeftBorder, topRightBorder);

        unitPerPixel = 1f / ppu;
    }

    public void AddFollowObject(Transform t)
    {
        followObjects.Add(t);
    }

    public void RemoveFollowObject(Transform t)
    {
        followObjects.Remove(t);
    }

    void Zoom(Bounds bounds)
    {
        float xZoom = (bounds.extents.x) / cam.aspect;
        float yZoom = (bounds.extents.y);

        targetZoom = Mathf.Clamp(Mathf.Max(xZoom, yZoom) + extraZoom, minZoom, maxZoom);

        float speed = cam.orthographicSize < targetZoom ? zoomOutSpeed : zoomInSpeed;

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * speed);
    }

    void Move(Bounds bounds)
    {
        Vector2 center = bounds.center;
        Vector3 newPosition = center + offset;

        float mSmoothTime = moveSmoothTime;

        if (cam.orthographicSize < targetZoom)
            mSmoothTime *= 0.5f;

        transform.position = Vector2.SmoothDamp(transform.position, newPosition, ref moveVelocity, mSmoothTime);

        Vector3 clampedPosition = ClampPositionToCurrZoom(transform.position);
        transform.position = new Vector3(clampedPosition.x, clampedPosition.y, -10);

        if (doIncrementByPixels)
            transform.position = ClampToGrid(transform.position);
    }

    Vector3 ClampPositionToCurrZoom(Vector3 pos)
    {
        Vector3 camZoomRadius = C.GetCameraZoomRadius(cam, cam.orthographicSize);

        if (topRightBorder != null)
        {
            Vector3 trb = topRightBorder.position;

            if (dontOffsetBordersByZoom == false)
                trb -= camZoomRadius;

            if (pos.x > trb.x)
                pos.x = trb.x;
            if (pos.y > trb.y)
                pos.y = trb.y;
        }

        if (bottomLeftBorder != null)
        {
            Vector3 blb = bottomLeftBorder.position;

            if (dontOffsetBordersByZoom == false)
                blb += camZoomRadius;

            if (pos.x < blb.x)
                pos.x = blb.x;
            if (pos.y < blb.y)
                pos.y = blb.y;
        }

        return pos;
    }

    Vector3 ClampToBordersAtMaxZoom(Vector3 pos)
    {
        if (topRightBorder != null)
        {
            if (pos.x > topRightBorder.position.x)
                pos.x = topRightBorder.position.x;
            if (pos.y > topRightBorder.position.y)
                pos.y = topRightBorder.position.y;
        }
        if (bottomLeftBorder != null)
        {
            if (pos.x < bottomLeftBorder.position.x)
                pos.x = bottomLeftBorder.position.x;
            if (pos.y < bottomLeftBorder.position.y)
                pos.y = bottomLeftBorder.position.y;
        }

        return pos;
    }

    Vector3 ClampToGrid(Vector3 position)
    {
        float x = Mathf.Round(position.x / unitPerPixel) * unitPerPixel;
        float y = Mathf.Round(position.y / unitPerPixel) * unitPerPixel;

        return new Vector3(x, y, -10);
    }

    Bounds GetBounds()
    {
        for (int i = followObjects.Count - 1; i >= 0; i--)
            if (followObjects[i] == null)
                followObjects.RemoveAt(i);

        if (followObjects.Count == 0)
            return new Bounds(Vector3.zero, Vector3.zero);

        Bounds bounds = new Bounds(ClampToBordersAtMaxZoom(followObjects[0].position), Vector2.zero);

        for (int i = followObjects.Count - 1; i >= 0; i--)
        {
            Transform toFollow = followObjects[i];

            if (toFollow != null)
            {
                if (toFollow.gameObject.activeSelf)
                    bounds.Encapsulate(ClampToBordersAtMaxZoom(toFollow.position));
            }
            else
                followObjects.RemoveAt(i);
        }

        return bounds;
    }

    void FixedUpdate()
    {
        if (followObjects.Count == 0 || Active == false)
            return;

        Bounds bounds = GetBounds();

        Move(bounds);
        Zoom(bounds);
    }

    public void SetBarriers(Transform bottomLeft, Transform topRight)
    {
        bottomLeftBorder = bottomLeft;
        topRightBorder = topRight;

        if (bottomLeft == null || topRight == null || dontAutoSetMinMaxZoomAccordingToBorders)
            return;

        topRight.position = new Vector3(topRight.position.x, Mathf.Max(topRight.position.y, bottomLeft.position.y + (topRight.position.x - bottomLeft.position.x) / Camera.main.aspect));

        float deltaX = Mathf.Abs(topRightBorder.position.x - bottomLeftBorder.position.x);
        float deltaY = Mathf.Abs(topRightBorder.position.y - bottomLeftBorder.position.y);
        maxZoom = Mathf.Min(deltaX / 2f / cam.aspect, deltaY / 2f);

        if (minZoom > maxZoom)
            minZoom = maxZoom;
    }

    public void SetExtraZoom(float extraZoom)
    {
        this.extraZoom = extraZoom;
    }

    public void SetMoveSmoothTime(float moveSmoothTime)
    {
        this.moveSmoothTime = moveSmoothTime;
    }

    public Transform TopRightBorder => topRightBorder;
    public Transform BottomLeftBorder => bottomLeftBorder;
}