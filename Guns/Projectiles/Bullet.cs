using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    [SerializeField] bool bounceOffWall;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //else if (bounceOffWall && collision.gameObject.CompareTag("Wall"))
        //{
        //    Reflect(collision.contacts[0]);
        //    return;
        //}

        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
            HitDamageable(damageable);
        else
            HitSurface();
    }

    void Reflect(ContactPoint2D point)
    {
        Vector2 dir = Vector2.Reflect(transform.up, point.normal);
        transform.rotation = Quaternion.Euler(0, 0, Extensions.AngleFromPosition(transform.position, transform.position + new Vector3(dir.x, dir.y, 0)) - 90);

        if (particles)
            particles.Emit(1);
    }
}
