using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldUIFollow : MonoBehaviour
{
    [SerializeField] protected GameObject uiPrefab;
    protected GameObject uiElement;

    protected Canvas WorldSpaceCanvas => Extensions.FindObjectOfNameFromArray("WorldSpaceCanvas", FindObjectsOfType<Canvas>()) as Canvas;

    protected virtual void Update()
    {
        uiElement.transform.position = transform.position;
        uiElement.transform.rotation = transform.rotation;
    }

    private void OnEnable()
    {
        if (uiElement != null)
            uiElement.SetActive(true);
    }

    private void OnDisable()
    {
        if (uiElement != null)
            uiElement.SetActive(false);
    }

    protected GameObject InstantiateUi()
    {
        Canvas c = Extensions.FindObjectOfNameFromArray("WorldSpaceCanvas", FindObjectsOfType<Canvas>()) as Canvas;
        uiElement = Instantiate(uiPrefab, c.transform);
        return uiElement;
    }
}
