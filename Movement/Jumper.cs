using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    public event System.Action JumpedOffGround;

    [SerializeField] protected CharacterCollision collision;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Mover mover;
    [SerializeField] SoundType jumpSound;

    [Header("Luxeries")]
    [SerializeField] float jumpPressEarlyTime;
    [SerializeField] float coyoteTime;

    [Header("Feel")]
    [SerializeField] protected float jumpingGravity;
    [SerializeField] float jumpingGravityCutoff;
    [SerializeField] float yVelFallingStart;
    [SerializeField] float fallingGravity;

    [Header("Input")]
    [SerializeField] protected AnimationCurve jumpForceCurve;
    [SerializeField] protected IntervalTimer spaceBarTimer;
    [SerializeField] protected float jumpVelocity;

    float lastTimeOnGround;
    float lastTimeHitJump;

    int possibleExtraJumps;
    int extraJumps;

    bool canReplenishJumps;
    bool spaceBarReleased;
    bool jumpExists;
    bool falling;


    private void Awake()
    {
        lastTimeOnGround = -coyoteTime;
        lastTimeHitJump = -jumpPressEarlyTime;


        spaceBarReleased = true;
        canReplenishJumps = true;

        possibleExtraJumps = GameData.I.GameSettings.Preset.Rules.Jumps - 1;
        MultiplyGravity(GameData.I.GameSettings.Preset.Rules.Gravity);
    }

    protected virtual void FixedUpdate()
    {
        if (collision.OnJumpableSurface)
            OnSurfaceBehavior();
        else
            AirBehavior();

        if (spaceBarTimer.DecrementIfRunning(Time.fixedDeltaTime))
        {
            spaceBarTimer.Stop();
            jumpExists = false;
        }
    }

    protected virtual void OnSurfaceBehavior()
    {
        // For coyote time
        lastTimeOnGround = Time.time;

        if (canReplenishJumps == true)
            ReplenishJumps();

        //// If we hit the jump button a frame before we hit the ground
        //if (Time.time - lastTimeHitJump < jumpPressEarlyTime)
        //    Jump();
    }

    protected void AirBehavior()
    {
        falling = (rb.linearVelocity.y < yVelFallingStart);

        if (falling)
            rb.gravityScale = fallingGravity;
        else
            rb.gravityScale = jumpingGravity;

        canReplenishJumps = true;
    }

    public void PressJump()
    {
        if (spaceBarReleased && jumpExists == false)
        {
            if (collision.OnJumpableSurface)
                TryStartNewJump();
            else if (extraJumps > 0)
            {
                extraJumps--;
                TryStartNewJump();
            }
            else
                lastTimeHitJump = Time.time;
        }

        if (jumpExists)
            Jump();
    }

    protected virtual void TryStartNewJump()
    {
        if (collision.OnJumpableSurface == true)
        {
            if (collision.Spring)
                mover.AddYVelocity(jumpVelocity * collision.Spring.GetComponentInParent<Spring>().Bounce());

            SoundManager.I.PlaySound(jumpSound, transform.position);
            JumpedOffGround?.Invoke();
        }

        // Start new jump
        spaceBarReleased = false;
        jumpExists = true;

        // We can't replenish our jumps until we leave all jumpable surfaces
        canReplenishJumps = false;

        spaceBarTimer.Start();
    }

    protected virtual void Jump()
    {
        rb.gravityScale = jumpingGravity;

        float jumpVel = jumpForceCurve.Evaluate(spaceBarTimer.PercentageComplete) * jumpVelocity;

        if (mover.Velocity.y < jumpVel)
            mover.SetYVelocity(jumpVel);

        //if (mover.Velocity.y < jumpVelocity)
        //    mover.SetYVelocity(vel);
        //rb.linearVelocity += new Vector2(0, jumpForceCurve.Evaluate(spaceBarTimer.PercentageComplete) * jumpVelocity);
    }

    public void ReleaseJumpButton()
    {
        jumpExists = false;
        spaceBarReleased = true;
    }

    public void MultiplyGravity(float multiplier)
    {
        jumpingGravity *= multiplier;
        fallingGravity *= multiplier;
    }

    void ReplenishJumps()
    {
        extraJumps = possibleExtraJumps;
    }

    public void AddJump()
    {
        extraJumps++;
    }
}