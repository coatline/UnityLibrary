using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveInputs : MonoBehaviour
{
    const float DOWN_PLATFORM_DEADZONE = .5f;
    const float MOVE_DEADZONE = .2f;

    public bool CanGoDownPlatform { get; private set; }
    public float XMovement { get; private set; }
    public float YMovement { get; private set; }

    [SerializeField] Player player;

    private void Start()
    {
        player.Controls.Gameplay.Move.performed += OnMove;
        player.Controls.Gameplay.Move.canceled += OnMove;
    }


    void OnMove(InputAction.CallbackContext value)
    {
        Vector2 val = value.ReadValue<Vector2>();

        XMovement = Mathf.MoveTowards(0, 1, Mathf.Abs(val.x) / .5f);
        YMovement = Mathf.MoveTowards(0, 1, Mathf.Abs(val.y) / .5f);

        if (val.x < 0)
            XMovement *= -1;

        if(val.y < 0)
            YMovement *= -1;

        CanGoDownPlatform = val.y < -DOWN_PLATFORM_DEADZONE;
    }

    private void OnDestroy()
    {
        player.Controls.Gameplay.Move.performed -= OnMove;
        player.Controls.Gameplay.Move.canceled -= OnMove;
    }
}
