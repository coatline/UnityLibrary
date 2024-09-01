using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputsManager : Singleton<InputsManager>
{
    public event System.Action<InputDevice> DeviceAdded;
    public event System.Action<InputDevice> DeviceDisconnected;

    [SerializeField] bool debugLogs;

    public List<InputDevice> AllSeenInputDevices { get; private set; }
    List<InputDevice> connectedDevices;

    Dictionary<InputUser, UserInputData> userToInputData;
    Dictionary<InputDevice, InputUser> deviceToUser;
    Dictionary<InputUser, Controls> userToControls;

    protected override void Awake()
    {
        AllSeenInputDevices = new();
        connectedDevices = new();
        userToInputData = new();
        userToControls = new();
        deviceToUser = new();

        InputSystem.onDeviceChange += OnDeviceChanged;

        for (int i = 0; i < InputSystem.devices.Count; i++)
            OnDeviceChanged(InputSystem.devices[i], InputDeviceChange.Added);

        base.Awake();
    }

    public Controls GetControlsFromUser(InputUser u) => userToControls[u];
    public bool IsDeviceConnected(InputDevice device) => connectedDevices.Contains(device);
    public bool IsDeviceBeingUsed(InputDevice device) => deviceToUser.ContainsKey(device);
    public UserInputData GetUserInputData(InputUser user)
    {
        if (userToInputData.TryGetValue(user, out UserInputData val))
            return val;
        else
        {
            //Debug.Log($"Creating new UserInputData for {user.pairedDevices[0].displayName}");

            Controls controls = GetControlsFromUser(user);
            ControlScheme controlScheme = DataLibrary.I.ControlSchemes[user.controlScheme.Value.name];

            UserInputData newData = new UserInputData(controlScheme, user.pairedDevices.ToArray(), controls, user);
            userToInputData.Add(user, newData);
            return newData;
        }
    }

    public void RemoveUsersFromDevices(InputDevice[] devices)
    {
        for (int i = 0; i < devices.Length; i++)
        {
            InputDevice device = devices[i];
            deviceToUser.TryGetValue(device, out InputUser user);
            deviceToUser.Remove(device);

            if (user.valid == false) continue;

            user.actions.Disable();

            userToControls.Remove(user);
            user.UnpairDevicesAndRemoveUser();
        }
    }

    public InputUser CreateInputUser(InputDevice device)
    {
        List<InputDevice> devices = new();

        ControlScheme controlScheme = DataLibrary.I.ControlSchemes["Gamepad"];

        if (device is Mouse || device is Keyboard)
        {
            controlScheme = DataLibrary.I.ControlSchemes["Keyboard&Mouse"];

            devices.Add(Keyboard.current);
            devices.Add(Mouse.current);
        }
        else
            devices.Add(device);

        InputUser user = InputUser.PerformPairingWithDevice(devices[0]);

        foreach (InputDevice dev in devices)
        {
            if (dev != devices[0])
                InputUser.PerformPairingWithDevice(dev, user: user);

            deviceToUser.Add(dev, user);
        }

        Controls userControls = new Controls();

        user.AssociateActionsWithUser(userControls);
        user.ActivateControlScheme(controlScheme.Name);

        userToControls.Add(user, userControls);

        userControls.Enable();
        //Debug.Log($"Connecting: {device.displayName}");

        return user;
    }

    public void SetUserNewControls(InputUser user, Controls newControls)
    {
        Controls previousControls = userToControls[user];
        previousControls.Dispose();

        userToControls[user] = newControls;

        user.AssociateActionsWithUser(newControls);
        newControls.Enable();
    }

    void OnDeviceChanged(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:

                if (IsDeviceNew(device))
                {
                    if (debugLogs)
                        Debug.Log($"Device Added: {device.displayName}, Id: {device.deviceId}");

                    AllSeenInputDevices.Add(device);
                }
                else if (debugLogs)
                    Debug.Log($"Device Reconnected: {device.displayName}, ID: {device.deviceId}");

                connectedDevices.Add(device);

                DeviceAdded?.Invoke(device);
                break;

            case InputDeviceChange.Disconnected:
                if (debugLogs)
                    Debug.Log($"Device Disconnected: {device.displayName}");

                connectedDevices.Remove(device);
                DeviceDisconnected?.Invoke(device);
                break;
        }
    }

    bool IsDeviceNew(InputDevice device)
    {
        return AllSeenInputDevices.Contains(device) == false;

        for (int i = 0; i < AllSeenInputDevices.Count; i++)
        {
            if (AllSeenInputDevices[i].deviceId == device.deviceId)
            {
                Debug.Log($"deviceID : {AllSeenInputDevices[i].deviceId}, {AllSeenInputDevices[i].displayName}");
                return false;
            }
        }

        return true;
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChanged;
    }
}
