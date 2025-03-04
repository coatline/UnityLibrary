using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDecender : MonoBehaviour
{
    [SerializeField] Collider2D[] worldColliders;
    [SerializeField] float platformIgnoreTime;

    Collider2D ignorable;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
            ignorable = collision.collider;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") && collision.gameObject == ignorable)
            ignorable = null;
    }

    public void TryDecend()
    {
        if (ignorable != null)
            StartCoroutine(DoIgnore());
    }

    IEnumerator DoIgnore()
    {
        Collider2D ignoreCol = ignorable;
        ignorable = null;

        foreach (Collider2D collider2D in worldColliders)
            Physics2D.IgnoreCollision(collider2D, ignoreCol, true);

        yield return new WaitForSeconds(platformIgnoreTime);

        foreach (Collider2D collider2D in worldColliders)
            Physics2D.IgnoreCollision(collider2D, ignoreCol, false);
    }
}