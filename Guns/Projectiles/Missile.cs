using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Missile : Bullet
{
    public event System.Action Exploded;

    [SerializeField] protected RocketProperties rocketProperties;
    [SerializeField] Explosion explosionPrefab;

    public bool BlewUp { get; private set; }
    bool accelerating;

    void Start()
    {
        StartCoroutine(DelayMovement());
    }

    public void RocketSetup(RocketProperties properties)
    {
        this.rocketProperties = properties;
    }

    protected override void OnDestroyed()
    {
        Explosion e = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        e.Setup(properties.Damage, player, properties.SourceItem);

        Exploded?.Invoke();
        BlewUp = true;
        Destroy(gameObject);
    }

    Vector2 acceleration;
    float time;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (accelerating)
        {
            acceleration = (rocketProperties.Acceleration) * transform.up;
            rb.velocity = ((new Vector2(rocketProperties.StartupSpeed, rocketProperties.StartupSpeed)) * transform.up);
            rb.velocity += acceleration * time;
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, rocketProperties.MaxSpeed);
            time += Time.fixedDeltaTime;
        }
        else
            rb.velocity = new Vector2(rocketProperties.StartupSpeed, rocketProperties.StartupSpeed) * transform.up;
    }

    IEnumerator DelayMovement()
    {
        yield return new WaitForSeconds(rocketProperties.StartupDuration);
        accelerating = true;
        //col.enabled = true;
    }
}
