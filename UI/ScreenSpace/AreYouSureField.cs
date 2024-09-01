using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreYouSureField : MonoBehaviour
{
    [SerializeField] Button yesButton;
    [SerializeField] GameObject fieldHolder;
    [SerializeField] TMP_Text areYouSureText;
    public bool Open { get; set; }

    public void EnableAreYouSureField(Action a, string message = "Are you sure?")
    {
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => { a(); });
        yesButton.onClick.AddListener(() => { OnChose(); });

        areYouSureText.text = message;
        fieldHolder.gameObject.SetActive(true);
        Open = true;
    }

    public void OnChose()
    {
        fieldHolder.SetActive(false);
        Open = false;
    }

    //IEnumerator Delay()
    //{
    //    // This is so that the button click doesn't register 
    //    yield return new WaitForEndOfFrame();
    //    fieldHolder.SetActive(false);
    //}
}
