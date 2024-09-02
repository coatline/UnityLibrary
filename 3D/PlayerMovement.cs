using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float jumpHeight = 5;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float flySpeed;
    [SerializeField] Rigidbody rb;

    Vector2 currentDirection;

    float currentSpeed;
    bool flying;
    bool jump;

    void Start()
    {
        currentSpeed = walkSpeed;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        var fwd = ((transform.forward * currentDirection.y) + (transform.right * currentDirection.x)).normalized * currentSpeed * Time.fixedDeltaTime;
        rb.velocity = new Vector3(fwd.x, rb.velocity.y, fwd.z);

        if (flying)
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (jump)
        {
            rb.AddForce(new Vector3(0, jumpHeight), ForceMode.Impulse);
            jump = false;
        }
    }

    public void TryJump()
    {
        if (CanJump())
            if (rb.velocity.y <= 0)
                jump = true;
    }

    public void TryFly(float dir)
    {
        if (flying)
            transform.Translate(new Vector3(0, dir * Time.fixedDeltaTime * flySpeed, 0));
    }

    public void ToggleFlying()
    {
        flying = !flying;
    }

    public void ToggleRunning()
    {
        if (currentSpeed == walkSpeed)
            currentSpeed = runSpeed;
        else
            currentSpeed = walkSpeed;
    }

    bool CanJump()
    {
        if (Physics.Raycast(transform.position, -transform.up, 1.01f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetDirection(Vector2 direction) => currentDirection = direction;
}
