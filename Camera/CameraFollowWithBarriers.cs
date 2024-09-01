using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class CameraFollowWithBarriers : MonoBehaviour, IFollower
{
    public Transform bottomLeftBarrier;
    public Transform topRightBarrier;

    [SerializeField] bool offsetBarrierPositionsToFitCameraSize;
    [SerializeField] bool useCoords;
    public Vector2 bottomLeftBarrierCoords;
    public Vector2 topRightBarrierCoords;

    [SerializeField] Transform followObject;
    public Vector2 cameraSizeInUnits;

    [SerializeField] bool doBarriers = true;

    [Range(.01f, 1f)]
    [SerializeField] float speed;

    private void Awake()
    {
        var cam = GetComponent<Camera>();

        cameraSizeInUnits.x = cam.orthographicSize * cam.aspect;
        cameraSizeInUnits.y = cam.orthographicSize;
    }

    private void Start()
    {
        if (offsetBarrierPositionsToFitCameraSize && topRightBarrier && bottomLeftBarrier)
        {
            bottomLeftBarrier.transform.position += new Vector3((cameraSizeInUnits.x), cameraSizeInUnits.y, 0);
            topRightBarrier.transform.position -= new Vector3((cameraSizeInUnits.x), cameraSizeInUnits.y, 0);
        }

        if (!useCoords && doBarriers)
            if (!bottomLeftBarrier || !topRightBarrier) { Debug.LogWarning("You forgot to assign barriers!"); doBarriers = false; }
    }

    void FixedUpdate()
    {
        if (!followObject) { return; }

        Vector3 movement = new Vector3(followObject.position.x - transform.position.x, followObject.position.y - transform.position.y);

        Vector3 blbc = bottomLeftBarrierCoords;
        Vector3 trbc = topRightBarrierCoords;

        if (doBarriers)
        {
            if (!useCoords)
            {
                if (bottomLeftBarrier)
                    blbc = bottomLeftBarrier.position;

                if (topRightBarrier)
                    trbc = topRightBarrier.position;
            }

            if (offsetBarrierPositionsToFitCameraSize)
            {
                blbc += new Vector3((cameraSizeInUnits.x), cameraSizeInUnits.y, 0);
                trbc -= new Vector3((cameraSizeInUnits.x), cameraSizeInUnits.y, 0);
            }

            #region OldClamping
            //if (transform.position.x <= blbc.x)
            //{
            //    if (movement.x < 0)
            //    {
            //        transform.position = new Vector3(blbc.x, transform.position.y, -10);
            //        movement.x = 0;
            //    }
            //}
            //if (transform.position.y <= blbc.y)
            //{
            //    if (movement.y < 0)
            //    {
            //        transform.position = new Vector3(transform.position.x, blbc.y, -10);
            //        movement.y = 0;
            //    }
            //}
            //if (transform.position.x >= trbc.x)
            //{
            //    if (movement.x > 0)
            //    {
            //        transform.position = new Vector3(trbc.x, transform.position.y, -10);
            //        movement.x = 0;
            //    }
            //}
            //if (transform.position.y >= trbc.y)
            //{
            //    if (movement.y > 0)
            //    {
            //        transform.position = new Vector3(transform.position.x, trbc.y, -10);
            //        movement.y = 0;
            //    }
            //} 
            #endregion
        }

        Vector3 newPos = (transform.position + (movement * speed));

        if (doBarriers) { newPos = new Vector3(Mathf.Clamp(newPos.x, blbc.x, trbc.x), Mathf.Clamp(newPos.y, blbc.y, trbc.y), -10); }

        transform.position = newPos;
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    public void SetFollow(Transform follow)
    {
        followObject = follow;
    }
}
