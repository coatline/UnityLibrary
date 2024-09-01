using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMultiFollow : MonoBehaviour
{
    [SerializeField] List<Transform> followObjects;

    [Header("Movement")]
    [SerializeField] Vector2 offset;
    [SerializeField] float moveSmoothTime;

    [Header("Zoom")]
    [SerializeField] float maxZoom;
    [SerializeField] float minZoom;
    [SerializeField] float extraZoom;
    [SerializeField] Camera cam;

    public bool Active { get; set; }
    Vector2 moveVelocity;

    private void Awake()
    {
        //RespawnAnimation.Respawned += (p) => { AddFollowObject(p.transform); };
        //Player.PlayerKilledPlayer += (player, killed) => { RemoveFollowObject(killed.transform.GetChild(0)); };
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
        cam.orthographicSize = Mathf.Lerp(minZoom, maxZoom, (Mathf.Max(bounds.extents.x / 2, bounds.extents.y)) / maxZoom) + extraZoom;
    }

    void Move(Bounds bounds)
    {
        Vector2 center = bounds.center;
        Vector3 newPosition = center + offset;
        transform.position = Vector2.SmoothDamp(transform.position, newPosition, ref moveVelocity, moveSmoothTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    Bounds GetBounds()
    {
        if (followObjects.Count == 0)
            return new Bounds(Vector3.zero, Vector3.zero);

        Bounds bounds = new Bounds(followObjects[0].position, Vector2.zero);

        for (int i = followObjects.Count - 1; i >= 0; i--)
        {
            Transform toFollow = followObjects[i];

            if (toFollow != null)
            {
                if (toFollow.gameObject.activeSelf)
                    bounds.Encapsulate(toFollow.position);
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
}
