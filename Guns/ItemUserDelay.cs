using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GunUser))]
public class ItemUserDelay : MonoBehaviour
{
    public bool CantUseItem { get; private set; }
    [SerializeField] PlayerInputs playerInputs;
    [SerializeField] GunUser user;
    bool needInputReleased;
    bool timeUp;

    private void Start()
    {
        playerInputs.UseItemInputReleased += UseInputReleased;
        user.Used += Wait;
    }

    IEnumerator ItemUseTimer()
    {
        yield return new WaitForSeconds(useTime);

        timeUp = true;

        if (needInputReleased == false)
            CantUseItem = false;
    }

    public void UseInputReleased()
    {
        needInputReleased = false;

        if (timeUp == true)
            CantUseItem = false;
    }

    float useTime;

    void Wait(float delay, bool isManual)
    {
        useTime = delay;
        timeUp = false;
        CantUseItem = true;

        needInputReleased = isManual;

        StartCoroutine(ItemUseTimer());
    }

    public void Respawn()
    {
        StopAllCoroutines();
        CantUseItem = false;
    }
}
