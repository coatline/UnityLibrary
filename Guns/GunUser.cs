using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GunHolder))]
public class GunUser : MonoBehaviour
{
    public event System.Action<float, bool> Used;

    [SerializeField] MuzzleFlashAnimation muzzleFlashAnimation;
    [SerializeField] ReloadBehavior reloadBehavior;
    [SerializeField] ItemUserDelay itemUserDelay;
    [SerializeField] AudioSource itemAudioSource;
    [SerializeField] SpriteRenderer muzzleFlash;
    [SerializeField] SpriteRenderer itemSprite;
    [SerializeField] Transform handSprite;
    [SerializeField] RecoilAnimation recoil;
    [SerializeField] GunHolder itemHolder;
    [SerializeField] Collider2D[] hitBoxes;
    [SerializeField] HealthHaver damageable;
    [SerializeField] Player player;
    [SerializeField] Mover mover;

    public Collider2D[] HitBoxes => hitBoxes;
    public bool Bursting { get; private set; }
    public bool UseItemLock { get; set; }

    private void Start()
    {
        damageable.Respawned += Respawned;
    }

    void Respawned()
    {
        Bursting = false;
        itemUserDelay.Respawn();
    }

    public void TryUseItem()
    {
        if (UseItemLock || itemUserDelay.CantUseItem || itemHolder.Gun == null) return;

        GunStack gunStack = itemHolder.GunStack;
        Gun gun = itemHolder.Gun;

        if (gunStack.ShotsRemaining == 0 || reloadBehavior.AutoReloading)
            return;

        if (reloadBehavior.Reloading)
            reloadBehavior.StopReloading();

        if (gun.Burst)
            StartCoroutine(BurstFire(gunStack));
        else
            Fire(gunStack);

        Used?.Invoke(gun.UseDelay, gun.ManualFire);
    }

    void Fire(GunStack gunStack)
    {
        InGameStats itemStats = player.ItemMatchStats[gunStack.GunType];
        itemStats.uses++;
        player.ItemMatchStats[gunStack.GunType] = itemStats;

        SoundManager.I.PlaySound(gunStack.GunType.SoundOnUse, handSprite.position);

        for (int i = 0; i < gunStack.GunType.BulletCount; i++)
            if (gunStack.TryShoot())
                ShootProjectile(i, gunStack.GunType);
            else
                break;

        // Do recoil
        recoil.Recoil(gunStack.GunType.RecoilSettings);

        if (gunStack.GunType.RecoilSettings.SetVelocityToRecoil)
        {
            mover.SetYVelocity(0);
            mover.AddExternalForce((-itemSprite.transform.right * gunStack.GunType.RecoilSettings.ActualRecoilForce));


            //mover.SetYVelocity((-itemSprite.transform.right * gunStack.GunType.RecoilSettings.ActualRecoilForce).y);
            //mover.AddExternalForce((-itemSprite.transform.right * gunStack.GunType.RecoilSettings.ActualRecoilForce) * new Vector2(1, 0));
        }
        else
            mover.AddExternalForce((-itemSprite.transform.right * gunStack.GunType.RecoilSettings.ActualRecoilForce));
    }

    void ShootProjectile(int bulletIndex, Gun gun)
    {
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

    IEnumerator BurstFire(GunStack gunStack)
    {
        Gun gun = gunStack.GunType;

        Bursting = true;

        float burstTime = gun.TimeBetweenAttacks;
        int bursts = gun.AttacksPerBurst;

        for (int i = 0; i < bursts; i++)
        {
            // If we change items partway through the burst then stop bursting
            if (itemHolder.Gun == null || gun != itemHolder.Gun || gunStack.ShotsRemaining == 0 || reloadBehavior.Reloading) { break; }

            Fire(gunStack);

            // Do not wait again if this is the last bullet
            if (i < bursts - 1 && gunStack.ShotsRemaining > 0)
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

        Projectile newProjectile = Instantiate(gun.ProjectilePrefab, itemSprite.transform.position, Quaternion.Euler(handSprite.eulerAngles - new Vector3(0, 0, 90 + randRot)));

        newProjectile.transform.localPosition += new Vector3(bulletHole.x, bulletHole.y);
        // Z value is strange.
        newProjectile.transform.Translate(newProjectile.transform.right * xOffset, Space.World);

        newProjectile.Setup(gun.ProjectileProperties, newProjectile.transform.up, player, player.Team.Hitboxes);

        return newProjectile;
    }

    private void OnDestroy()
    {
        damageable.Respawned -= Respawned;
        //playerInputs.UseItem -= TryUseItem;
    }
}
