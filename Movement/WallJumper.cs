using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumper : Jumper
{
    [SerializeField] float wallJumpSideVelocity;
    [SerializeField] float wallJumpUpVelocity;

    [SerializeField] CharacterInputs characterInputs;
    [SerializeField] AnimationCurve wallJumpForceCurve;

    [Header("Wall Slide")]
    [SerializeField] ParticleSystem slideParticles;
    [SerializeField] AudioSource slideAudioSource;
    [SerializeField] AudioClip slideAudioClip;

    int wallJumpingDir;

    protected override void FixedUpdate()
    {
        // We are wall sliding
        if (collision.WallDirection != 0)
            rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY / collision.WallFriction);

        base.FixedUpdate();
    }

    protected override void Jump()
    {
        rb.gravityScale = jumpingGravity;

        float jumpVel = jumpForceCurve.Evaluate(spaceBarTimer.PercentageComplete) * jumpVelocity;

        if (wallJumpingDir != 0)
        {
            float wallJumpVel = jumpForceCurve.Evaluate(spaceBarTimer.PercentageComplete) * wallJumpUpVelocity;
            float wallJumpSideVel = wallJumpForceCurve.Evaluate(spaceBarTimer.PercentageComplete) * wallJumpSideVelocity * -wallJumpingDir;

            if (mover.Velocity.y < wallJumpVel)
                mover.SetYVelocity(wallJumpVel);

            if (Mathf.Abs(mover.Velocity.x) < Mathf.Abs(wallJumpSideVel))
                mover.SetXForce(wallJumpSideVel);
        }

        else if (mover.Velocity.y < jumpVel)
            mover.SetYVelocity(jumpVel);
    }

    protected override void TryStartNewJump()
    {
        // If we are only touching the wall
        if (collision.IsOnGround == false && collision.WallDirection != 0)
            wallJumpingDir = collision.WallDirection;
        else
            wallJumpingDir = 0;

        base.TryStartNewJump();
    }
}