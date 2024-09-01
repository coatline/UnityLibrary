using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class UserInputData
{
    public event System.Action ControlsChanged;

    public Controls Controls { get; private set; }

    public readonly ControlScheme ControlScheme;
    public readonly InputDevice[] Devices;
    public readonly InputUser InputUser;
    public readonly bool HasKeyboard;

    public UserInputData(ControlScheme controlScheme, InputDevice[] devices, Controls controls, InputUser inputUser)
    {
        HasKeyboard = controlScheme.Name == "Keyboard&Mouse";
        ControlScheme = controlScheme;
        Devices = devices;
        Controls = controls;
        InputUser = inputUser;
    }

    public void SetControls(Controls newControls)
    {
        Controls = newControls;
        InputsManager.I.SetUserNewControls(InputUser, newControls);
        ControlsChanged?.Invoke();
    }
}
