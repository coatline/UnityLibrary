using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GunHolder))]
public class GunUser : MonoBehaviour
{
    public event System.Action GunUsed;

    [SerializeField] MuzzleFlashAnimation muzzleFlashAnimation;
    [SerializeField] ParticleSystem bulletCasingParticles;
    [SerializeField] ReloadBehavior reloadBehavior;
    [SerializeField] ItemUserDelay itemUserDelay;
    [SerializeField] AudioSource itemAudioSource;
    [SerializeField] SpriteRenderer muzzleFlash;
    [SerializeField] SpriteRenderer itemSprite;
    [SerializeField] SpecialUser specialUser;
    [SerializeField] Transform handSprite;
    [SerializeField] RecoilAnimation recoil;
    [SerializeField] GunHolder itemHolder;
    [SerializeField] Collider2D[] hitBoxes;
    [SerializeField] Character character;
    [SerializeField] Mover mover;

    public Collider2D[] MyColliders => hitBoxes;
    public bool Bursting { get; private set; }
    public bool UseItemLock { get; set; }

    GunStack GunStack => itemHolder.GunStack;

    public void GunChanged()
    {
        Bursting = false;
        itemUserDelay.ResetDelay();
    }

    public void Respawning()
    {
        Bursting = false;
        itemUserDelay.ResetDelay();
    }

    public void TryUseItem()
    {
        if (UseItemLock || itemUserDelay.CantUseItem || GunStack == null || gameObject.activeSelf == false || (specialUser.SpecialActive && specialUser.Special.CanShoot == false)) return;

        if (GunStack.ShotsRemaining == 0 || reloadBehavior.AutoReloading)
            return;

        if (reloadBehavior.Reloading)
            reloadBehavior.StopReloading();

        if (GunStack.GunType.Burst)
            StartCoroutine(BurstFire());
        else
            Fire();

        itemUserDelay.Wait(GunStack.GunType.UseDelay, GunStack.GunType.ManualFire);
        GunUsed?.Invoke();
    }

    void Fire()
    {
        RumbleController.I.TryStartRumbleFor(character.PlayerData, new Rumble(GunStack.GunType.RumbleLowFrequency, GunStack.GunType.RumbleHighFrequency, GunStack.GunType.RumbleDuration));
        SoundManager.I.PlaySound(GunStack.GunType.SoundOnUse, handSprite.position);

        for (int i = 0; i < GunStack.GunType.BulletCount; i++)
            if (GunStack.TryShoot())
                ShootProjectile(i, GunStack.GunType);
            else
                break;

        // Do recoil
        recoil.Recoil(GunStack.GunType.RecoilSettings);

        Vector2 recoilForce = -itemSprite.transform.right * GunStack.GunType.RecoilSettings.ActualRecoilForce;

        if (GunStack.GunType.RecoilSettings.SetVelocityToRecoil)
        {
            mover.SetXForce(recoilForce.x);
            mover.SetYVelocity(recoilForce.y);
        }
        else
        {
            mover.AddXForce(recoilForce.x);
            mover.AddYVelocity(recoilForce.y);
        }
    }

    void ShootProjectile(int bulletIndex, Gun gun)
    {
        bulletCasingParticles.Emit(1);

        float randRot = 0;
        float xOffset = 0;

        if (!gun.ParellelBullets)
        {
            float spread = (((float)gun.BulletCount * (float)gun.AttackSpacing) / 2f);
            float weaponSpreadVal = gun.Spread;

            randRot = -(spread) + Random.Range(-weaponSpreadVal, weaponSpreadVal) + ((float)bulletIndex * gun.AttackSpacing);
        }
        else
        {
            xOffset = -((gun.BulletCount * gun.AttackSpacing) / 2) + (bulletIndex * gun.AttackSpacing);
        }

        CreateProjectile(randRot, xOffset, gun);
    }

    IEnumerator BurstFire()
    {
        GunStack startingGun = GunStack;

        Bursting = true;

        float burstTime = startingGun.GunType.TimeBetweenAttacks;
        int bursts = startingGun.GunType.AttacksPerBurst;

        for (int i = 0; i < bursts; i++)
        {
            // If we change items partway through the burst then stop bursting
            if (itemHolder.GunStack == null || startingGun != itemHolder.GunStack || GunStack.ShotsRemaining == 0 || reloadBehavior.Reloading)
                break;

            Fire();

            // Do not wait again if this is the last bullet
            if (i < bursts - 1 && GunStack.ShotsRemaining > 0)
                yield return new WaitForSeconds(burstTime);
        }

        Bursting = false;
    }

    Projectile CreateProjectile(float randRot, float xOffset, Gun gun)
    {
        var bulletHole = recoil.GetOffsetFromHand(new Vector2(gun.AttackOffset.x, gun.AttackOffset.y));

        if (gun.MuzzleFlash.DoFlash)
        {
            muzzleFlashAnimation.Flash(gun.MuzzleFlash.MuzzleFlashSpeed, gun.MuzzleFlash.MuzzleFlashSize, gun.MuzzleFlash.MuzzleFlashColor);
            muzzleFlash.transform.position = bulletHole + itemSprite.transform.position;
        }

        Quaternion rot = Quaternion.Euler(handSprite.eulerAngles - new Vector3(0, 0, 90 + randRot));
        Projectile projectile = Instantiate(gun.ProjectilePrefab, itemSprite.transform.position, rot);

        projectile.transform.localPosition += new Vector3(bulletHole.x, bulletHole.y);
        // Z value is strange.
        projectile.transform.Translate(projectile.transform.right * xOffset, Space.World);

        projectile.Move(gun.ShotForce, projectile.transform.up);
        projectile.Initialize(gun.ProjectileProperties, gun, character);
        projectile.SetColor(character.TeamColor, character.TeamOutlineColor);

        return projectile;
    }
}