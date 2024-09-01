using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] Collider2D circleCollider;
    [SerializeField] Sound explosionSound;

    Item sourceItem;
    float damage;
    Player player;

    public void Setup(float damage, Player player, Item sourceItem)
    {
        SoundManager.I.PlaySound(explosionSound, transform.position);

        this.damage = damage;
        this.player = player;
        this.sourceItem = sourceItem;

        foreach (Collider2D collider2D in player.Team.Hitboxes)
        {
            // Hurt original user, just not their teamates
            if (player.HitBoxes[0] == collider2D || player.HitBoxes[1] == collider2D) continue;

            Physics2D.IgnoreCollision(circleCollider, collider2D);
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
            damageable.Damage(damage, player, sourceItem);
    }
}
