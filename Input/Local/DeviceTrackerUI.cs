using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DeviceTrackerUI : MonoBehaviour
{
    [SerializeField] HorizontalLayoutGroup layout;
    [SerializeField] Image deviceIconPrefab;
    [SerializeField] bool debugLogs;

    [SerializeField] Sprite keyboardSprite;

    Dictionary<InputDevice, Image> deviceToIcon;

    void Awake()
    {
        deviceToIcon = new();

        InputsManager.I.DeviceAdded += DeviceAdded;
        InputsManager.I.DeviceDisconnected += DeviceDisconnected;

        InitializeDevices();
    }

    void InitializeDevices()
    {
        foreach (InputDevice device in InputsManager.I.AllSeenInputDevices)
        {
            DeviceAdded(device);

            if (InputsManager.I.IsDeviceConnected(device) == false)
                DeviceDisconnected(device);
        }
    }

    void DeviceAdded(InputDevice device)
    {
        if (device is Mouse)
            return;

        if (deviceToIcon.ContainsKey(device))
        {
            DeviceReconnected(device);
            return;
        }

        Image newIcon = Instantiate(deviceIconPrefab, layout.transform);
        deviceToIcon.Add(device, newIcon);

        if (device is Keyboard)
            newIcon.sprite = keyboardSprite;
    }

    void DeviceDisconnected(InputDevice device)
    {
        deviceToIcon[device].color -= new Color(0, 0, 0, 0.5f);
    }

    void DeviceReconnected(InputDevice device)
    {
        deviceToIcon[device].color += new Color(0, 0, 0, 0.5f);
    }

    //IEnumerator DisconnectTextAnimation()
    //{
    //    disconnectedText.gameObject.SetActive(true);

    //    Vector3 position = disconnectedText.transform.position;

    //    while (disconnectedText.transform.position.y < position.y + 50)
    //    {
    //        disconnectedText.transform.Translate(0, 0.75f, 0);
    //        disconnectedText.color = new Color(1, 1, 1, disconnectedText.color.a - Time.deltaTime);
    //        yield return null;
    //    }

    //    disconnectedText.gameObject.SetActive(false);
    //    disconnectedText.transform.position = position;
    //    disconnectedText.color = Color.white + new Color(0, 0, 0, 1);
    //}

    private void OnDestroy()
    {
        if (InputsManager.I == null) return;
        InputsManager.I.DeviceAdded -= DeviceAdded;
        InputsManager.I.DeviceDisconnected -= DeviceDisconnected;
    }
}
