using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] CircleCollider2D circleCollider;
    [SerializeField] AnimationCurve damageFalloff;
    [SerializeField] SoundType explosionSound;
    [SerializeField] bool hurtsSourcePlayer;
    [SerializeField] float maxRumbleDist;
    [SerializeField] float minDistance;
    [SerializeField] float knockback;

    List<IDamageable> damageables;

    Character character;
    Item sourceItem;

    float maxDistance;
    float damage;

    public virtual void Initialize(float damage, Character character, Item sourceItem, Collider2D[] ignoreColliders)
    {
        SoundManager.I.PlaySound(explosionSound, transform.position);

        damageables = new List<IDamageable>();

        this.maxDistance = circleCollider.radius;
        this.damage = damage;
        this.character = character;
        this.sourceItem = sourceItem;

        foreach (Collider2D collider2D in ignoreColliders)
        {
            // Hurt original user, just not their teamates
            if (hurtsSourcePlayer && (character.MyColliders[0] == collider2D || character.MyColliders[1] == collider2D)) continue;

            Physics2D.IgnoreCollision(circleCollider, collider2D);
        }

        //if (GameManager.I != null)
        {
            // Rumble Controllers
            foreach (PlayerController player in PlayerSpawner.I.Players)
            {
                float dist = Vector2.Distance(transform.position, player.Character.transform.position) / maxRumbleDist;
                RumbleController.I.TryStartRumbleFor(player.PlayerData, new Rumble(1 - dist, 1 - dist, (1 - dist) * 0.35f));
            }
        }

        float distanceFromCamera = Vector2.Distance(transform.position, Camera.main.transform.position);
        float percentage = Mathf.Max(1 - Mathf.Clamp01(distanceFromCamera / 20f), 0.5f);

        EZCameraShake.CameraShaker.Instance.ShakeOnce(percentage * 4, percentage * 4, 0.1f, percentage * 1f);

        StartCoroutine(StartExplosion());

        Destroy(gameObject, 1f);
    }

    public void Die()
    {
        Destroy(gameObject, 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponentInParent<IDamageable>();

        if (damageable != null && damageables.Contains(damageable) == false)
        {
            damageables.Add(damageable);

            float distance = Vector2.Distance(transform.position, collision.transform.position);
            float distancePercentage = Mathf.Clamp01((distance - minDistance) / maxDistance);
            float damagePercentage = damageFalloff.Evaluate(distancePercentage);
            damageable.ApplyKnockback((collision.transform.position - transform.position).normalized * knockback * (1 - distancePercentage));
            damageable.Damage(damage * damagePercentage, character, sourceItem);
        }
    }

    IEnumerator StartExplosion()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        circleCollider.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistance);
    }
}