using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Play shooting sound on projectiles so that fast firing weapons do not cut off the sounds

    [SerializeField] protected ParticleSystem particles;
    [SerializeField] protected SpriteRenderer sr;
    [SerializeField] protected Collider2D col;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] bool keepMass;

    [SerializeField] float actualDestroyDelay;
    [SerializeField] Sound soundOnHit;

    [SerializeField] int durability;

    protected ProjectileProperties properties;

    protected Player player;
    protected bool dead;

    float highestVelocity;

    public virtual void Setup(ProjectileProperties properties, Vector3 direction, Player player, Collider2D[] ignoreColliders = null)
    {
        this.properties = properties;

        rb.velocity = direction * properties.Force;
        transform.localScale *= properties.Scale;
        rb.gravityScale = properties.Gravity;

        this.player = player;

        if (keepMass == false)
            // So that the physics system does not determine knockback.
            rb.mass = 0.000001f;

        StartCoroutine(DoLifeTime(properties.MaxLifeTime));
        IgnoreColliders(ignoreColliders);
    }

    public void Setup(ProjectileProperties properties, Player player, Collider2D[] ignoreColliders = null)
    {
        this.player = player;

        this.properties = properties;

        IgnoreColliders(ignoreColliders);
        StartCoroutine(DoLifeTime(properties.MaxLifeTime));
    }

    float tooSlowTimer;

    protected virtual void FixedUpdate()
    {
        // Do minimum velocity
        if (rb.velocity.magnitude < properties.MinVelocityMagnitude)
        {
            tooSlowTimer += Time.fixedDeltaTime;

            if (tooSlowTimer > .05f)
                TryDestroyProjectile(true);
        }
        else
            tooSlowTimer = 0;

        // Do linear drag
        rb.velocity = rb.velocity / (properties.LinearDrag + Vector2.one);

        if (highestVelocity < rb.velocity.magnitude)
            highestVelocity = rb.velocity.magnitude;
    }

    void TryDestroyProjectile(bool fadeOut)
    {
        if (dead) return;
        dead = true;

        OnDestroyed();

        if (fadeOut == false)
            if (particles != null)
                particles.Emit((int)highestVelocity / 3);

        col.enabled = false;
        sr.enabled = false;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        Destroy(gameObject, actualDestroyDelay);
    }

    // Contains logic
    protected virtual void OnDestroyed() { }

    protected virtual void HitSurface()
    {
        durability--;

        if (soundOnHit != null)
            SoundManager.I.PlaySound(soundOnHit, transform.position);

        if (durability <= 0)
        {
            // Play explode animation
            TryDestroyProjectile(false);
        }
    }

    protected virtual void HitDamageable(IDamageable damageable)
    {
        if (damageable.Dead)
            return;

        DamageDamageable(damageable, properties.Damage);

        HitSurface();
    }

    protected virtual void DamageDamageable(IDamageable damageable, float amount)
    {
        damageable.Damage(amount, player, properties.SourceItem);
    }

    void IgnoreColliders(Collider2D[] cols)
    {
        if (col != null)
            foreach (Collider2D collider in cols)
                Physics2D.IgnoreCollision(col, collider);
    }

    IEnumerator DoLifeTime(float time)
    {
        yield return new WaitForSeconds(time);
        if (gameObject != null)
            TryDestroyProjectile(true);
    }
}