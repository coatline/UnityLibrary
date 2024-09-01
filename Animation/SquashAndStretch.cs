using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashAndStretch : MonoBehaviour
{
    [SerializeField] float maxExtraScale;
    [SerializeField] float velocityToScaleFactor;

    [SerializeField] ParticleSystem movementParticles;
    [SerializeField] GameObject spriteGameobject;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Jumper jumper;
    [SerializeField] Mover mover;

    void Awake()
    {
        jumper.JumpedOffGround += Jumped;
    }

    private void Update()
    {
        float xScalePercent = rb.velocity.x / velocityToScaleFactor;
        float yScalePercent = rb.velocity.y / velocityToScaleFactor;

        float extraX = Mathf.Min(Mathf.Abs(xScalePercent), maxExtraScale);
        float extraY = Mathf.Min(Mathf.Abs(yScalePercent), maxExtraScale);

        float extraXScale = (extraX - extraY);
        float extraYScale = (extraY - extraX);

        spriteGameobject.transform.localScale = new Vector3(1 + extraXScale, 1 + extraYScale);
        spriteGameobject.transform.localPosition = new Vector3(xScalePercent, -extraX) / 2f;

        if (jumper.IsGrounded && mover.Velocity.magnitude > 1)
            if (Random.Range(0, 100) < Mathf.Abs(mover.Velocity.x))
                movementParticles.Emit(1);
    }

    void Jumped() => movementParticles.Emit(6);
}
