using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldNamer : MonoBehaviour
{
    [SerializeField] TMP_Text inputFieldText;
    [SerializeField] TMP_Text renameText;

    public void DisableRenameText()
    {
        renameText.gameObject.SetActive(false);
        inputFieldText.gameObject.SetActive(true);
    }

    public void EnableRenameText()
    {
        renameText.text = inputFieldText.text;
        renameText.gameObject.SetActive(true);
        inputFieldText.gameObject.SetActive(false);
    }
}
