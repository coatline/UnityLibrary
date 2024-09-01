using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimator : MonoBehaviour
{
    [SerializeField] CameraFollowWithBarriers cfwb;
    [SerializeField] float initialUnitZoom;
    [SerializeField] Camera cam;
    Character c;

    CameraMoveCommand currentCommand;
    float unitOrthographicSize;

    void Start()
    {
        cam = GetComponent<Camera>();
        c = FindObjectOfType<Character>();

        SetZoom(initialUnitZoom);
    }

    public void Command(CameraMoveCommand cmc, bool setToInitialZoom = false, bool setPositionToPlayerPos = false)
    {
        if (setPositionToPlayerPos)
            cmc.Position = c.transform.position;
        if (setToInitialZoom)
            cmc.zoom = initialUnitZoom;

        currentCommand = cmc;
    }

    public void SetZoom(float orthographicSize)
    {
        //ppc.assetsPPU = Mathf.FloorToInt(ppc.refResolutionX / (float)orthographicSize);
        //ppc.refResolutionY = Mathf.CeilToInt((orthographicSize / 2) * ppc.assetsPPU);
        unitOrthographicSize = orthographicSize;
        cam.orthographicSize = orthographicSize;
    }

    void Update()
    {
        if (currentCommand != null)
        {
            if (cfwb.enabled)
                cfwb.enabled = false;

            if (currentCommand.lerpZoom)
                SetZoom(Mathf.Lerp(unitOrthographicSize, currentCommand.zoom, Time.deltaTime * currentCommand.zoomSpeed));
            else
                SetZoom(currentCommand.zoom);

            if (currentCommand.lerpPosition)
                transform.position = Vector3.Lerp(transform.position, currentCommand.Position, Time.deltaTime * currentCommand.moveSpeed);
            else
                transform.position = currentCommand.Position;

            if (Mathf.Abs(unitOrthographicSize - currentCommand.zoom) < .03f && Vector2.Distance(transform.position, currentCommand.Position) < .03f)
            {
                if (currentCommand.followPlayerWhenFinished)
                    cfwb.enabled = true;

                transform.position = currentCommand.Position;
                SetZoom(currentCommand.zoom);

                currentCommand = null;
            }
        }
    }
}

public class CameraMoveCommand
{
    public Vector3 Position { get { return pos; } set { value = new Vector3(value.x, value.y, -10); pos = value; } }
    Vector3 pos;

    public float zoom;

    public float zoomSpeed;
    public float moveSpeed;

    public bool followPlayerWhenFinished;
    public bool lerpPosition;
    public bool lerpZoom;

    public CameraMoveCommand(Vector3 position, float zoom, float zoomSpeed, float moveSpeed, bool lerpPosition, bool lerpZoom, bool followPlayerWhenFinished = false)
    {
        this.Position = position;
        this.zoom = zoom;
        this.zoomSpeed = zoomSpeed;
        this.moveSpeed = moveSpeed;
        this.lerpPosition = lerpPosition;
        this.lerpZoom = lerpZoom;
        this.followPlayerWhenFinished = followPlayerWhenFinished;
    }
}