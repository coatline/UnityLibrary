using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] FPCamera fpCamera;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (Input.GetMouseButtonDown(0))
                Cursor.lockState = CursorLockMode.Locked;
            else
            {
                playerMovement.SetDirection(Vector2.zero);
                return;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;

        Jump();
        LateralMovement();
        Mouse();
        Flying();
        Debug();
    }

    void Mouse()
    {
        Vector2 inputValues = new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        fpCamera.RotateCamera(inputValues);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            playerMovement.TryJump();
    }

    void LateralMovement()
    {
        Vector2 inputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        playerMovement.SetDirection(inputs);

        if (Input.GetKeyDown(KeyCode.LeftControl))
            playerMovement.ToggleRunning();
    }

    void Flying()
    {
        if (Input.GetKey(KeyCode.Space))
            playerMovement.TryFly(1);
        if (Input.GetKey(KeyCode.LeftShift))
            playerMovement.TryFly(-1);
    }

    void Debug()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene("Game");

        if (Input.GetKeyDown(KeyCode.F))
        {
            playerMovement.ToggleFlying();
        }
    }
}
