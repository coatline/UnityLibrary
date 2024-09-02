using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FPCamera : MonoBehaviour
{
    [SerializeField] float cameraPitchLimit = 80;
    [SerializeField] float lookSensitivity = 1;
    [SerializeField] float smoothing = 1;
    [SerializeField] float speed = 4;
    [SerializeField] Camera cam;

    Vector2 currentLookingPosition;
    Vector2 smoothedVelocity;


    /// <param name="inputValues">Input.GetAxisRaw("Mouse X"), ...</param>
    public void RotateCamera(Vector2 inputValues)
    {
        inputValues = Vector2.Scale(inputValues, new Vector2(smoothing * lookSensitivity, smoothing * lookSensitivity));

        smoothedVelocity.x = Mathf.Lerp(smoothedVelocity.x, inputValues.x, 1 / smoothing);
        smoothedVelocity.y = Mathf.Lerp(smoothedVelocity.y, inputValues.y, 1 / smoothing);

        currentLookingPosition += smoothedVelocity;

        // Clamp the vertical rotation (pitch)
        currentLookingPosition.y = Mathf.Clamp(currentLookingPosition.y, -cameraPitchLimit, cameraPitchLimit);

        cam.transform.localRotation = Quaternion.AngleAxis(-currentLookingPosition.y, Vector3.right);
        transform.localRotation = Quaternion.AngleAxis(currentLookingPosition.x, transform.up);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(cam.transform.position, cam.transform.position + (cam.transform.forward * 10));
    //}
}
