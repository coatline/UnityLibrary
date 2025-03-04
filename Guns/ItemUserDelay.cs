using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GunUser))]
public class ItemUserDelay : MonoBehaviour
{
    [SerializeField] CharacterInputs playerInputs;
    [SerializeField] GunUser user;

    public bool CantUseItem { get; private set; }


    bool needInputReleased;
    IntervalTimer timer;

    public void UseInputReleased()
    {
        needInputReleased = false;

        if (timer.IsRunning == false)
            CantUseItem = false;
    }

    public void Wait(float delay, bool isManual)
    {
        timer.StartWithInterval(delay);
        CantUseItem = true;

        needInputReleased = isManual;
    }

    public void ResetDelay()
    {
        timer.Stop();
        CantUseItem = false;
    }

    private void FixedUpdate()
    {
        if (timer.DecrementIfRunning(Time.fixedDeltaTime))
        {
            timer.Stop();

            if (needInputReleased == false)
                CantUseItem = false;
        }
    }

    private void OnEnable()
    {
        ResetDelay();
    }
}
