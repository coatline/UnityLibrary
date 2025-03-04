using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Missile : Projectile
{
    public event System.Action Exploded;

    [SerializeField] Explosion explosionPrefab;
    [SerializeField] Sprite acceleratingSprite;
    [SerializeField] float explosionDamage;

    public bool BlewUp { get; private set; }

    IntervalTimer lifetimeTimer;
    float acceleration;
    float speed;

    public override void Initialize(ProjectileProperties properties, Item sourceItem, Character sourceCharacter)
    {
        base.Initialize(properties, sourceItem, sourceCharacter);
    }

    public void SetLifeTime(float time)
    {
        lifetimeTimer.StartWithInterval(time);
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    protected override void OnDestroyed()
    {
        Explosion e = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        e.Initialize(explosionDamage, sourceCharacter, sourceItem, sourceCharacter.PlayerController.FriendlyColliders);

        Exploded?.Invoke();
        BlewUp = true;
        Destroy(gameObject);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector2 direction = transform.up;
        speed += (acceleration) * Time.fixedDeltaTime;
        rb.linearVelocity = direction * speed;

        if (lifetimeTimer.DecrementIfRunning(Time.fixedDeltaTime))
        {
            TryDestroy();
            lifetimeTimer.Stop();
        }
    }

    public void SetAcceleration(float acceleration)
    {
        this.acceleration = acceleration;
        speed *= 1.75f;
    }
    public void SetSprite() => sr.sprite = acceleratingSprite;
}