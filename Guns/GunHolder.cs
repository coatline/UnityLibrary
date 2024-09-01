using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHolder : MonoBehaviour
{
    public GunStack GunStack { get; private set; }
    public Gun Gun => GunStack.GunType;

    [SerializeField] SpriteRenderer muzzleFlash;
    [SerializeField] SpriteRenderer itemSprite;

    [SerializeField] Transform originTransform;
    [SerializeField] Transform handTransform;
    [SerializeField] Transform handPivot;
    [SerializeField] Transform handSprite;

    [SerializeField] RecoilAnimation recoil;
    [SerializeField] AimInputs aimInputs;
    [SerializeField] GunUser itemUser;

    [SerializeField] float reach;
    bool disabled;

    public void SetGun(Gun gun) => GunStack = new GunStack(gun);

    private void Start()
    {
        itemSprite.sprite = Gun.Sprite;
    }

    /// <summary>
    /// Aims the hand and item towards designated position.
    /// </summary>
    /// <param name="toPosition">The position you want to aim at.</param>
    /// <param name="aimVariability">For variability in AI attacks.</param>
    public void Aim(Vector3 toPosition, Vector2 aimVariability)
    {
        float angle = Extensions.AngleFromPosition(originTransform.position, toPosition) - 90;

        angle += Random.Range((float)aimVariability.x, (float)aimVariability.y);

        float flip = 0;

        if (angle > 0 || angle < -180)
            flip = 180;

        toPosition.z = 0;
        Vector2 pos = (toPosition - originTransform.position).normalized * (Mathf.Clamp(Vector2.Distance(originTransform.position, toPosition), 0f, reach));

        handTransform.localPosition = pos;

        handTransform.transform.localRotation = Quaternion.Euler(0, 0, (angle + 90));
        handSprite.transform.localRotation = Quaternion.Euler(flip, 0, 0);


        //handTransform.transform.localRotation = Quaternion.Euler(0, 0, (angle + 90));
        //handSprite.transform.localRotation = Quaternion.Euler(flip, 0, 0);
    }

    private void Update()
    {
        Aim(aimInputs.Position, Vector2.zero);
    }

    public void SetActive(bool active)
    {
        //Debug.Log($"Weapon setting to {active}, disabled was {disabled}.");

        if (active)
        {
            //if (disabled)
            {
                disabled = false;
                itemUser.UseItemLock = false;
                handSprite.gameObject.SetActive(true);
            }
        }
        //else if (disabled == false)
        else
        {
            disabled = true;
            itemUser.UseItemLock = true;
            handSprite.gameObject.SetActive(false);
        }
    }
}