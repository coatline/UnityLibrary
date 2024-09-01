using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    public event System.Action JumpedOffGround;

    public bool JumpLocked { get; set; }

    [SerializeField] string[] wallJumpLayersNames;
    [SerializeField] string[] groundLayersNames;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Sound jumpSound;

    LayerMask wallJumpLayerMask;
    LayerMask groundLayerMask;

    //[SerializeField] WallJumper wallJumper;
    [SerializeField] Mover mover;

    [Header("Luxeries")]
    [SerializeField] float jumpPressEarlyTime;
    [SerializeField] float coyoteTime;
    [Header("Feel")]
    [SerializeField] float jumpingGravityCutoff;
    [SerializeField] float yVelFallingStart;
    [SerializeField] float fallingGravity;
    [SerializeField] float jumpingGravity;
    [Header("Input")]
    [SerializeField] float spacebarTime;
    [SerializeField] float jumpVelocity;
    [SerializeField] float wallJumpSideVelocity;
    [SerializeField] float wallJumpUpVelocity;

    [Header("Jump Raycast")]
    [SerializeField] float jumpRaycastDepth;
    [SerializeField] Vector2 jumpRaycastSpacing;

    [Header("Wall Jump Raycast")]
    [SerializeField] float wallJumpRaycastDepth;
    [SerializeField] Vector2 wallJumpRaycastOffset;

    bool falling;

    public bool IsGrounded { get; private set; }
    int freeJumps;
    int isOnWall;

    float lastTimeOnGround;
    float lastTimeHitJump;

    bool currentActiveJumpExists;
    bool canStartNewJump;
    GameObject spring;

    private void Awake()
    {
        lastTimeOnGround = -coyoteTime;
        lastTimeHitJump = -jumpPressEarlyTime;

        groundLayerMask = LayerMask.GetMask(groundLayersNames);
        wallJumpLayerMask = LayerMask.GetMask(wallJumpLayersNames);

        canStartNewJump = true;
    }

    void FixedUpdate()
    {
        // TODO: optimize this
        IsGrounded = IsOnGround();
        isOnWall = IsOnWall();

        if (IsGrounded)
        {
            // I am standing on the ground.
            OnGround();
        }
        else
        {
            if (isOnWall != 0)
            {
                // I am up against a wall.

            }
            else
            {
                // I am in the air
                InAir();
            }
        }
    }

    void OnGround()
    {
        // For coyote time
        lastTimeOnGround = Time.time;

        freeJumps = 0;

        // If we hit the jump button a frame before we hit the ground
        if (Time.time - lastTimeHitJump < jumpPressEarlyTime)
            Jump();
    }

    void InAir()
    {
        falling = (rb.velocity.y < yVelFallingStart);

        if (falling)
            rb.gravityScale = fallingGravity;
        else
            rb.gravityScale = jumpingGravity;
    }


    public void PressJump()
    {
        // If we just pressed the jump button
        if (currentActiveJumpExists == false)
            JumpJustPressedDown();
        else
            GetJumpButton();
    }

    void JumpJustPressedDown()
    {
        // If we haven't released the jump button from last jump
        if (canStartNewJump == false)
            return;

        // This is the first time I pressed the jump button

        lastTimeHitJump = Time.time;

        if (CanStartNewJump())
        {
            // Wall jump
            if (isOnWall != 0 && IsGrounded == false)
                mover.AddExternalForce(new Vector2(-isOnWall * wallJumpSideVelocity, wallJumpUpVelocity));
            else
                JumpedOffGround?.Invoke();

            // Start new jump
            currentActiveJumpExists = true;
            canStartNewJump = false;

            if (freeJumps > 0)
                freeJumps--;

            StartCoroutine(DoJumpTime());

            if (spring)
                mover.SetYVelocity(jumpVelocity * spring.GetComponentInParent<Spring>().Bounce());

            SoundManager.I.PlaySound(jumpSound, transform.position);
            GetJumpButton();
        }
    }

    void GetJumpButton()
    {
        if (currentActiveJumpExists == true)
            Jump();
    }

    bool CanStartNewJump()
    {
        return (IsGrounded || (isOnWall != 0) || freeJumps > 0);
    }

    void Jump()
    {
        rb.gravityScale = jumpingGravity;

        if (mover.Velocity.y < jumpVelocity)
            mover.SetYVelocity(jumpVelocity);
        //rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
    }

    public void ReleaseJumpButton()
    {
        currentActiveJumpExists = false;
        canStartNewJump = true;
    }

    IEnumerator DoJumpTime()
    {
        yield return new WaitForSeconds(spacebarTime);
        currentActiveJumpExists = false;
    }

    bool IsOnGround()
    {
        Vector2 sizeOfSprite = GetSizeOfSprite;

        Vector2 bottomLeft = new Vector2(transform.position.x - (sizeOfSprite.x / 2) - jumpRaycastSpacing.x, transform.position.y - (sizeOfSprite.y / 2) + jumpRaycastSpacing.y);
        Vector2 bottomRight = new Vector2(transform.position.x + (sizeOfSprite.x / 2) + jumpRaycastSpacing.x, transform.position.y - (sizeOfSprite.y / 2) + jumpRaycastSpacing.y);

        RaycastHit2D leftGround = Physics2D.Raycast(bottomLeft, -transform.up, jumpRaycastDepth, groundLayerMask);
        RaycastHit2D rightGround = Physics2D.Raycast(bottomRight, -transform.up, jumpRaycastDepth, groundLayerMask);

        if (leftGround)
        {
            if (leftGround.collider.gameObject.CompareTag("Spring"))
                spring = leftGround.collider.gameObject;
            else
                spring = null;

            return leftGround;
        }
        else if (rightGround)
        {
            if (rightGround.collider.gameObject.CompareTag("Spring"))
                spring = rightGround.collider.gameObject;
            else
                spring = null;

            return rightGround;
        }
        else
        {
            spring = null;
            return false;
        }
    }

    int IsOnWall()
    {
        Vector2 sizeOfSprite = GetSizeOfSprite;

        Vector2 middleLeft = new Vector2(transform.position.x - (sizeOfSprite.x / 2) - wallJumpRaycastOffset.x, transform.position.y + wallJumpRaycastOffset.y);
        Vector2 middleRight = new Vector2(transform.position.x + (sizeOfSprite.x / 2) + wallJumpRaycastOffset.x, transform.position.y + wallJumpRaycastOffset.y);

        RaycastHit2D leftWall = Physics2D.Raycast(middleLeft, -transform.right, wallJumpRaycastDepth, wallJumpLayerMask);
        RaycastHit2D rightWall = Physics2D.Raycast(middleRight, transform.right, wallJumpRaycastDepth, wallJumpLayerMask);

        if (leftWall)
            return -1;
        else if (rightWall)
            return 1;

        return 0;
    }

    private void OnDrawGizmos()
    {
        Vector2 sizeOfSprite = GetSizeOfSprite;

        Vector2 middleLeft = new Vector2(transform.position.x - (sizeOfSprite.x / 2) - wallJumpRaycastOffset.x, transform.position.y + wallJumpRaycastOffset.y);
        Vector2 middleRight = new Vector2(transform.position.x + (sizeOfSprite.x / 2) + wallJumpRaycastOffset.x, transform.position.y + wallJumpRaycastOffset.y);

        Vector2 bottomLeft = new Vector2(transform.position.x - (sizeOfSprite.x / 2) - jumpRaycastSpacing.x, transform.position.y - (sizeOfSprite.y / 2) + jumpRaycastSpacing.y);
        Vector2 bottomRight = new Vector2(transform.position.x + (sizeOfSprite.x / 2) + jumpRaycastSpacing.x, transform.position.y - (sizeOfSprite.y / 2) + jumpRaycastSpacing.y);

        DrawRaycast(middleLeft, -transform.right, wallJumpRaycastDepth);
        DrawRaycast(middleRight, transform.right, wallJumpRaycastDepth);

        DrawRaycast(bottomLeft, -transform.up, jumpRaycastDepth);
        DrawRaycast(bottomRight, -transform.up, jumpRaycastDepth);
    }

    void DrawRaycast(Vector2 origin, Vector2 direction, float length)
    {
        Vector2 endPoint = origin + (direction * length);
        Gizmos.DrawLine(origin, endPoint);
    }

    Vector2 GetSizeOfSprite => new Vector2(sr.sprite.rect.size.x / sr.sprite.pixelsPerUnit, sr.sprite.rect.size.y / sr.sprite.pixelsPerUnit) * transform.localScale;

    public void MultiplyGravity(float multiplier)
    {
        jumpingGravity *= multiplier;
        fallingGravity *= multiplier;
    }

    public void AddFreeJump()
    {
        freeJumps++;
    }
}