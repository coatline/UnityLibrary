using UnityEngine;

[System.Serializable]
public class MuzzleFlashProperties
{
    [SerializeField] Color muzzleFlashColor;
    [SerializeField] float muzzleFlashSpeed;
    [SerializeField] float muzzleFlashSize;
    [SerializeField] bool doFlash;

    public float MuzzleFlashSpeed { get { return muzzleFlashSpeed; } }
    public Color MuzzleFlashColor { get { return muzzleFlashColor; } }
    public float MuzzleFlashSize { get { return muzzleFlashSize; } }
    public bool DoFlash { get { return doFlash; } }

    //public float bulletSpeed;
    //public int bulletCount = 1;
    //public float spread;
    //public float bulletHoleX;
    //public float bulletHoleY;

    //[Header("Burst")]
    //public bool burst;
    //public int shotsPerBurst;
    //public float timeBetweenBullets;

    //[Header("Multi Shot")]
    //public bool parallelBullets;
    //public float bulletSpacing = .3f;
}