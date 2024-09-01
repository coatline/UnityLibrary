using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDecender : MonoBehaviour
{
    [SerializeField] Collider2D[] worldColliders;
    [SerializeField] MoveInputs moveInputs;
    Collider2D ignorable;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            ignorable = collision.collider;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            ignorable = null;
        }
    }

    public void TryDecend()
    {
        if (ignorable != null)
        {
            StartCoroutine(DoIgnore());
        }
    }

    private void Update()
    {
        if (moveInputs.CanGoDownPlatform)
            TryDecend();
    }

    IEnumerator DoIgnore()
    {
        Collider2D ignoreCol = ignorable;
        ignorable = null;

        foreach (Collider2D collider2D in worldColliders)
        {
            Physics2D.IgnoreCollision(collider2D, ignoreCol, true);
        }

        yield return new WaitForSeconds(.25f);

        foreach (Collider2D collider2D in worldColliders)
        {
            Physics2D.IgnoreCollision(collider2D, ignoreCol, false);
        }
    }
}
