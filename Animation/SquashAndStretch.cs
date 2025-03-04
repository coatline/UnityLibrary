using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashAndStretch : MonoBehaviour
{
    [SerializeField] float maxExtraScale;
    [SerializeField] float velocityToScaleFactor;

    [SerializeField] GameObject spriteGameobject;
    [SerializeField] Rigidbody2D rb;

    private void Update()
    {
        float xScalePercent = rb.linearVelocity.x / velocityToScaleFactor;
        float yScalePercent = rb.linearVelocity.y / velocityToScaleFactor;

        float extraX = Mathf.Min(Mathf.Abs(xScalePercent), maxExtraScale);
        float extraY = Mathf.Min(Mathf.Abs(yScalePercent), maxExtraScale);

        float extraXScale = (extraX - extraY);
        float extraYScale = (extraY - extraX);

        spriteGameobject.transform.localScale = new Vector3(1 + extraXScale, 1 + extraYScale);
        spriteGameobject.transform.localPosition = new Vector3(xScalePercent, -extraX) / 2f;
    }

    public void ResetSprite()
    {
        spriteGameobject.transform.localPosition = Vector3.zero;
        spriteGameobject.transform.localScale = Vector3.one;
    }
}