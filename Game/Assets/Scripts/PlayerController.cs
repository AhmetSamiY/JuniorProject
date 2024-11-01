using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float Horizontal;
    [SerializeField] private float Vertical;
    [SerializeField] private float Speed;
    [SerializeField] private float JumpPower;
    [SerializeField] private float CoyoteTime;
    [SerializeField] private float CoyoteTimer;
    [SerializeField] private float JumpBufferTime; 
    [SerializeField] private float JumpBufferTimer;
    [SerializeField] private float DashPower;  
    [SerializeField] private float DashTime;
    [SerializeField] private float DashCooldown;

    [Header("WeaponParameters")]
    [SerializeField] Projectile ProjectilePF;
    [SerializeField] Transform LaunchOffset;

    [Header("Booleans")]

    [SerializeField] public bool isFacingRight;
    [SerializeField] private bool canDash;
    [SerializeField] private bool isDashing;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask GroundLayers;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDashing)
        {
            return;
        }

        Horizontal = Input.GetAxisRaw("Horizontal");
        //Vertical = Input.GetAxisRaw("Vertical");

        if (IsGrounded())
        {
            CoyoteTimer = CoyoteTime;
        }
        else
        {
            CoyoteTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            JumpBufferTimer = JumpBufferTime;
        }
        else
        {
            JumpBufferTimer -= Time.deltaTime;
        }


        if (CoyoteTimer > 0f && JumpBufferTimer > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpPower);
            JumpBufferTimer = 0f;
        }
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            CoyoteTimer = 0f;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
        Flip();
        Throw();

    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        rb.velocity = new Vector2(Horizontal * Speed * Time.deltaTime, rb.velocity.y);
    }

     void Flip()
    {
        if (isFacingRight && Horizontal< 0f || !isFacingRight && Horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            /*
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale; */
            transform.Rotate(0f, 180f, 0f);

        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(GroundCheck.position, 0.2f, GroundLayers);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        if (isFacingRight)
        {
            rb.velocity = new Vector2(DashPower, 0f);

        }
        if (!isFacingRight)
        {
            rb.velocity = new Vector2(-DashPower, 0f);

        }
        yield return new WaitForSeconds(DashTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(DashCooldown);
        canDash = true;

    }

    private void Throw()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Instantiate(ProjectilePF , LaunchOffset.position, transform.rotation);
        }
    }
}
