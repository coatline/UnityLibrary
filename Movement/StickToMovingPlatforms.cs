using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach to the gameobject with your player's collider component
public class StickToMovingPlatforms : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    MovingPlatform movingPlatform;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If we got on a platform
        if (collision.gameObject.CompareTag("Platform"))
        {
            MovingPlatform movingP = collision.gameObject.GetComponent<MovingPlatform>();

            if (movingP != null)
                movingPlatform = movingP;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // If we got off a platform
        if (collision.gameObject.CompareTag("Platform"))
        {
            MovingPlatform movingP = collision.gameObject.GetComponent<MovingPlatform>();

            if (movingPlatform == movingP)
                movingPlatform = null;
        }
    }

    private void FixedUpdate()
    {
        if (movingPlatform == null)
            return;

        if (rb.velocity.y - movingPlatform.MoveDelta.y > 0.1f) return;

        // If we are on a platform, move our position to match the movement of the platform's
        transform.Translate(movingPlatform.MoveDelta.x * Time.fixedDeltaTime, 0, 0);
    }
}
