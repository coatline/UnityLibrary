using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHolder : MonoBehaviour
{
    public GunStack GunStack { get; private set; }

    [SerializeField] SpriteRenderer muzzleFlash;
    [SerializeField] SpriteRenderer itemSprite;

    [SerializeField] Transform originTransform;
    [SerializeField] Transform handTransform;
    [SerializeField] Transform handPivot;
    [SerializeField] Transform handSprite;

    [SerializeField] ReloadBehavior reloadBehavior;
    [SerializeField] CharacterInputs playerInputs;
    [SerializeField] RecoilAnimation recoil;
    [SerializeField] GunUser itemUser;

    [SerializeField] float reach;

    public bool Locked { get; private set; }

    public void SetGun(Gun gun)
    {
        GunStack = new GunStack(gun, GameData.I.GameSettings.Preset.Rules.AmmoMultiplier);
        itemSprite.sprite = GunStack.GunType.Sprite;

        itemUser.GunChanged();
        reloadBehavior.SetGun(GunStack);
    }

    /// <summary>
    /// Aims the hand and item towards designated position.
    /// </summary>
    /// <param name="toPosition">The position you want to aim at.</param>
    /// <param name="aimVariability">For variability in AI attacks.</param>
    public void Aim(Vector2 toPosition, Vector2 aimVariability)
    {
        float angle = C.AngleFromPosition(originTransform.position, toPosition) - 90;

        angle += Random.Range((float)aimVariability.x, (float)aimVariability.y);

        float flip = 0;

        if (angle > 0 || angle < -180)
            flip = 180;

        Vector2 pos = (toPosition - new Vector2(originTransform.position.x, originTransform.position.y)).normalized * (Mathf.Clamp(Vector2.Distance(originTransform.position, toPosition), 0f, reach));

        handTransform.localPosition = pos;

        handTransform.transform.localRotation = Quaternion.Euler(0, 0, (angle + 90));
        handSprite.transform.localRotation = Quaternion.Euler(flip, 0, 0);
    }

    public void SetActive(bool active, bool doLock = false)
    {
        if (doLock)
            Locked = !Locked;
        else if (Locked)
            return;

        if (active)
        {
            itemUser.UseItemLock = false;
            handSprite.gameObject.SetActive(true);
        }
        else
        {
            itemUser.UseItemLock = true;
            handSprite.gameObject.SetActive(false);
        }
    }

    public void SetItemSprite(Sprite sprite)
    {
        itemSprite.sprite = sprite;
    }

    public void SetItemSpriteToCurrentGun()
    {
        itemSprite.sprite = GunStack.GunType.Sprite;
    }
}