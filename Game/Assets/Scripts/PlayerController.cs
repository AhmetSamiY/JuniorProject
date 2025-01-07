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
    [SerializeField] private float doubleClickTimeLimit;
    [SerializeField] private float lastClickTime;


    [Header("WeaponParameters")]
    [SerializeField] Projectile ProjectilePF;
    [SerializeField] Transform LaunchOffset;

    [Header("Booleans")]

    [SerializeField] public bool isFacingRight;
    [SerializeField] private bool canDash;
    [SerializeField] private bool isDashing;
    [SerializeField] private bool AnimationPlaying;
    [SerializeField] private bool isBlocking;
    [SerializeField] private bool isDoubleClick;
    [SerializeField] private bool isAttackInProgress;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask GroundLayers;
    [SerializeField] private Animator animatorr;

    [Header("Ranged")]
    public int arrowCount = 0; // Current arrow count
    public int ShurikenCount = 0; // Current arrow count
    public bool CanThrowShuriken;
    public GameObject arrowPrefab; // Prefab for the arrow to shoot
    public Transform shootPoint; // Position where the arrow spawns

    [Header("Melee")]

    public int attackDamage = 25;
    public Transform attackPoint; // Empty GameObject at the player's weapon position
    public float attackRange = 0.5f; // Attack radius
    public LayerMask enemyLayers; // Define the layer enemies are on



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating(nameof(AddShuriken), 0f, 5f);

    }

    void Update()
    {
        if (isDashing || AnimationPlaying)
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

        if (Input.GetMouseButtonDown(0) && !isAttackInProgress)
        {
            float currentTime = Time.time;

            if (lastClickTime > 0f && currentTime - lastClickTime <= doubleClickTimeLimit)
            {
                isDoubleClick = true;
                isAttackInProgress = true;
                HeavyAttack();
            }
            else
            {
                isDoubleClick = false;
                lastClickTime = currentTime;
                Invoke("StartAttack", doubleClickTimeLimit);
            }
        }

        if (arrowCount >= 1f)
        {
            CanThrowShuriken = false;
        }
        else
        {
            CanThrowShuriken = true;
        }
        Flip();
        Throw();
        Block();
        animatorCheck();
        RangedAttack();
        
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        rb.velocity = new Vector2(Horizontal * Speed * Time.deltaTime, rb.velocity.y);
    }

    void animatorCheck()
    {
        if (Horizontal != 0)
        {
            animatorr.SetBool("Walking", true);
        }
        else if (Horizontal == 0)
        {
            animatorr.SetBool("Walking", false);

        }
        if (IsGrounded())
        {
            animatorr.SetBool("IsGrounded", true);
        }
        else if (!IsGrounded())
        {
            animatorr.SetBool("IsGrounded", false);

        }
        if (rb.velocity.y > 0f && !IsGrounded())
        {
            animatorr.SetBool("Falling", false);

            animatorr.SetBool("Jumping", true);
        }
        else if (rb.velocity.y < 0f && !IsGrounded())
        {
            animatorr.SetBool("Jumping", false);

            animatorr.SetBool("Falling", true);
        }
        else
        {
            animatorr.SetBool("Jumping", false);
            animatorr.SetBool("Falling", false);

        }
    }
    void Flip()
    {
        if (isFacingRight && Horizontal < 0f || !isFacingRight && Horizontal > 0f)
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
    void StartAttack()
    {
        if (!isDoubleClick)
        {
            RegularAttack();
        }

    }
    void RegularAttack()
    {
        animatorr.SetTrigger("Attack");
    }
    void HeavyAttack()
    {
        animatorr.SetTrigger("HeavyAttack");

    }
    void ResetAttack()
    {
        isAttackInProgress = false;
    }
    void ResetDoubleAttack()
    {
        isAttackInProgress = false;

        isDoubleClick = false;

    }
    private void Throw()
    {
        if (Input.GetKeyDown(KeyCode.E) && ShurikenCount > 0 && CanThrowShuriken)
        {
            animatorr.SetTrigger("Throw");
            ShurikenCount--;
        }     
    }
    void RangedAttack()
    {
        if (Input.GetKeyDown(KeyCode.E) && arrowCount > 0)
        {
            animatorr.SetTrigger("Arrow");
        }
    }
    void spawnShuriken()
    {
        Instantiate(ProjectilePF, LaunchOffset.position, transform.rotation);

    }
    private void Block()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            if (!isBlocking)
            {
                isBlocking = true;
                animatorr.SetTrigger("StartBlock");
            }
        }
        else
        {
            isBlocking = false;
        }
    }
    private void BlockBool()
    {
        isBlocking = false;
    }
    /*void IdleBlock()
    {
        animatorr.SetBool("IsBlocking", true);

    }*/
    public void AddArrow()
    {
        arrowCount++;
        Debug.Log("Arrow Collected! Total Arrows: " + arrowCount);
    }

    private void ShootArrow()
    {
        // Decrease arrow count
        arrowCount--;

        // Instantiate the arrow at the shoot point
        if (arrowPrefab != null && shootPoint != null)
        {
            Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
        }

        Debug.Log("Arrow Shot! Remaining Arrows: " + arrowCount);
    }


    void AddShuriken()
    {
        if (ShurikenCount >= 10) // Example: Stop at 10 shurikens
        {
            CancelInvoke(nameof(AddShuriken)); // Stops the repeated calls
            Debug.Log("Max shurikens reached!");
            return;
        }

        ShurikenCount += 1;
        Debug.Log("Added 1 shuriken! Total: " + ShurikenCount);
    }
    void StopInput()
    {
        AnimationPlaying = true;
    }
    void StartInput()
    {
        AnimationPlaying = false;
    }
}