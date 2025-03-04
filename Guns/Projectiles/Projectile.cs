using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] ShaderEffectController shaderEffectController;
    [SerializeField] FadeDestroyAnimation fadeDestroyAnimation;
    [SerializeField] Collider2D hitCollider;
    [SerializeField] SoundType soundOnHit;

    [SerializeField] protected ParticleSystem destroyParticles;
    [SerializeField] protected SpriteRenderer sr;
    [SerializeField] bool setTrailColorToOutline;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] float actualDestroyDelay;
    [SerializeField] TrailRenderer trail;

    protected ProjectileProperties properties;
    protected Character sourceCharacter;
    protected Item sourceItem;

    protected Vector2 lastVelocity;
    protected float hitPoints;
    protected bool dead;

    public virtual void Initialize(ProjectileProperties properties, Item sourceItem, Character sourceCharacter)
    {
        IgnoreColliders(hitCollider, sourceCharacter.FriendlyColliders);
        shaderEffectController.Initialize();

        this.sourceCharacter = sourceCharacter;
        this.properties = properties;
        this.sourceItem = sourceItem;

        rb.gravityScale = properties.Gravity;
        hitPoints = properties.HitPoints;
        rb.mass = properties.Mass;

        transform.localScale *= properties.Scale;

        if (fadeDestroyAnimation != null)
        {
            float startFadePercentage = 0.8f;
            float timeTilFade = properties.MaxLifeTime * startFadePercentage;
            fadeDestroyAnimation.StartFade(timeTilFade, properties.MaxLifeTime - timeTilFade, true);
            fadeDestroyAnimation.Finished += DestroyNoParticles;
        }
    }

    public void Move(float force, Vector2 direction)
    {
        rb.linearVelocity = force * direction;
    }

    public void SetColor(Color normalColor, Color outlineColor)
    {
        shaderEffectController.SetColor(normalColor);
        shaderEffectController.SetOutlineColor(outlineColor);

        if (destroyParticles != null)
        {
            var main = destroyParticles.main;
            main.startColor = normalColor;
        }

        if (trail != null)
        {
            if (setTrailColorToOutline)
                trail.startColor = outlineColor;
            else
                trail.startColor = normalColor;

            trail.endColor = outlineColor;
        }
    }

    protected virtual void FixedUpdate()
    {
        CheckForMinimumVelocity();

        // Do linear drag
        rb.linearVelocity = rb.linearVelocity / (properties.LinearDrag + Vector2.one);

        lastVelocity = rb.linearVelocity;
    }

    void CheckForMinimumVelocity()
    {
        // Do minimum velocity
        if (rb.linearVelocity.magnitude < properties.MinVelocityMagnitude)
        {
            //tooSlowTimer += Time.fixedDeltaTime;

            //if (tooSlowTimer > .05f)
            fadeDestroyAnimation.StartFade();
        }
        //else
        //    tooSlowTimer = 0;
    }

    protected void DestroyNoParticles()
    {
        destroyParticles = null;
        TryDestroy();
    }

    protected void TryDestroy()
    {
        if (dead)
            return;

        dead = true;

        OnDestroyed();

        if (destroyParticles != null)
            destroyParticles.Emit(Mathf.CeilToInt(lastVelocity.magnitude / 3));

        sr.enabled = false;
        rb.simulated = false;
        hitCollider.enabled = false;
        Destroy(gameObject, actualDestroyDelay);
    }

    protected virtual void OnDestroyed() { }

    protected void IgnoreColliders(Collider2D col1, Collider2D[] cols)
    {
        if (hitCollider != null)
            foreach (Collider2D collider in cols)
                Physics2D.IgnoreCollision(col1, collider, true);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
            HitDamageable(damageable);

        HitSomething();
    }

    protected virtual void HitDamageable(IDamageable damageable)
    {
        if (damageable.Dead || dead)
            return;

        damageable.ApplyKnockback(lastVelocity.normalized * properties.Knockback);
        damageable.Damage(GetDamage(), sourceCharacter, sourceItem);
    }

    void HitSomething()
    {
        hitPoints--;

        if (soundOnHit != null)
            SoundManager.I.PlaySound(soundOnHit, transform.position);

        if (hitPoints <= 0)
            TryDestroy();
    }

    protected void Reflect(ContactPoint2D point)
    {
        Vector2 dir = Vector2.Reflect(transform.up, point.normal);
        transform.rotation = Quaternion.Euler(0, 0, C.AngleFromPosition(transform.position, transform.position + new Vector3(dir.x, dir.y, 0)) - 90);

        if (destroyParticles != null)
            destroyParticles.Emit(1);
    }

    public float HitPoints => hitPoints;
    protected virtual float GetDamage() => properties.Damage;
}