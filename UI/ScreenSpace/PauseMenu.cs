using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static event Action Paused;
    public static event Action Resumed;

    [SerializeField] UnityEngine.UI.Button initialButton;
    [SerializeField] GameObject display;

    [SerializeField] InputAction cancelAction;
    [SerializeField] InputAction pauseAction;

    private void Start()
    {
        EnableInput();
    }

    void TogglePause(InputAction.CallbackContext s)
    {
        if (display.activeSelf)
            Resume();
        else
            Pause();
    }

    public void Resume()
    {
        display.gameObject.SetActive(false);
        cancelAction.performed -= TogglePause;
        Time.timeScale = 1;
        Resumed?.Invoke();
    }

    void Pause()
    {
        StartCoroutine(DelayEnableCancel());
        Time.timeScale = 0;
        Paused?.Invoke();
        display.gameObject.SetActive(true);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(initialButton.gameObject);
    }

    IEnumerator DelayEnableCancel()
    {
        yield return null;
        cancelAction.performed += TogglePause;
    }

    public void ChangeScene(string name)
    {
        Resume();
        SceneFader.I.LoadNewScene(name, 0.25f);
    }

    void EnableInput()
    {
        pauseAction.performed += TogglePause;
        pauseAction.Enable();
    }

    void DisableInput()
    {
        pauseAction.Disable();
    }

    void OnDestroy()
    {
        pauseAction.Dispose();
        cancelAction.Dispose();
    }
}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.InputSystem;
//using UnityEngine;
//using System;
//using UnityEngine.SceneManagement;

//public class OptionMenuDisplay : Singleton<OptionMenuDisplay>
//{
//    public static event Action Paused;
//    public static event Action Resumed;

//    [SerializeField] UnityEngine.UI.Button initialButton;
//    [SerializeField] GameObject display;

//    //Controls inputs;

//    private void Start()
//    {
//        //inputs = new Controls();

//        GameEnder.I.GameEnded += DisableInput;
//        //EnableInput();
//    }

//    void TogglePause(InputAction.CallbackContext s)
//    {
//        if (display.activeSelf)
//            Resume();
//        else
//            Pause();
//    }

//    public void Resume()
//    {
//        display.gameObject.SetActive(false);
//        inputs.UI.Cancel.performed -= TogglePause;
//        Time.timeScale = 1;
//        Resumed?.Invoke();
//    }

//    void Pause()
//    {
//        StartCoroutine(DelayEnableCancel());
//        Time.timeScale = 0;
//        Paused?.Invoke();
//        display.gameObject.SetActive(true);
//        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(initialButton.gameObject);
//    }

//    IEnumerator DelayEnableCancel()
//    {
//        yield return null;
//        inputs.UI.Cancel.performed += TogglePause;
//    }

//    public void ChangeScene(string name)
//    {
//        Resume();
//        SceneFader.I.LoadNewScene(name, 0.25f);
//    }

//    void EnableInput()
//    {
//        inputs.Gameplay.Pause.performed += TogglePause;
//        inputs.Enable();
//    }

//    void DisableInput()
//    {
//        inputs.Disable();
//    }

//    void OnDestroy()
//    {
//        inputs.Gameplay.Pause.performed -= TogglePause;
//        inputs.Dispose();
//        inputs = null;
//    }
//}
