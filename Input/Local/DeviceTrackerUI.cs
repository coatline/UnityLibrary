using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DeviceDisplayer : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Image deviceIconPrefab;
    [SerializeField] Sprite keyboardSprite;
    [SerializeField] float spacing;
    [SerializeField] float padding;

    Dictionary<InputDevice, Image> deviceToIcon;
    List<Image> icons;

    void Start()
    {
        deviceToIcon = new();
        icons = new();

        InputUserManager.I.DeviceAdded += DeviceAdded;
        InputUserManager.I.DeviceReconnected += DeviceReconnected;
        InputUserManager.I.DeviceDisconnected += DeviceDisconnected;

        for (int i = 0; i < InputUserManager.I.AllSeenInputDevices.Count; i++)
        {
            InputDevice dev = InputUserManager.I.AllSeenInputDevices[i];
            DeviceAdded(dev);

            if (InputUserManager.I.IsDeviceConnected(dev) == false)
                DeviceDisconnected(dev);
        }
    }

    void DeviceAdded(InputDevice device)
    {
        if (device is Mouse)
            return;

        Image newIcon = Instantiate(deviceIconPrefab, transform);
        deviceToIcon.Add(device, newIcon);
        icons.Add(newIcon);

        if (device is Keyboard)
            newIcon.sprite = keyboardSprite;

        float totalSize = 0;

        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].rectTransform.anchoredPosition = new Vector2(i * spacing + totalSize + padding, 0);
            totalSize += icons[i].rectTransform.sizeDelta.x;
        }

        rectTransform.sizeDelta = new Vector2(totalSize + (spacing * (icons.Count - 1)) + padding * 2, rectTransform.sizeDelta.y);
    }

    void DeviceReconnected(InputDevice device)
    {
        deviceToIcon[device].color += new Color(1, 1f, 1f, 0f);
    }

    void DeviceDisconnected(InputDevice device)
    {
        deviceToIcon[device].color -= new Color(1, 1f, 1f, 0f);
    }

    private void OnDestroy()
    {
        if (InputUserManager.I == null)
            return;

        InputUserManager.I.DeviceAdded -= DeviceAdded;
        InputUserManager.I.DeviceReconnected -= DeviceReconnected;
        InputUserManager.I.DeviceDisconnected -= DeviceDisconnected;
    }
}