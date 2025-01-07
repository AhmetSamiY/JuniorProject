using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySamurai : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpForce;
    public float chaseRange;
    public float attackRange;

    [Header("Attack")]
    public float attackCooldown; 
    public int attackDamage;

    [Header("Jump Detection")]
    public Transform groundCheck;
    public Transform obstacleCheck;
    public float groundCheckDistance;
    public float obstacleCheckDistance;
    public LayerMask groundLayer;

    [Header("References")]
    public Transform player;
    public Transform attackPoint;
    public float attackRadius;
    public LayerMask playerLayer;

    private bool isAttacking = false;
    private float attackTimer = 0f;

    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    void Update()
    {
        if (player == null) return;
        if (IsGrounded())
        {
            animator.SetBool("IsGrounded", true);
        }
        else if (!IsGrounded())
        {
            animator.SetBool("IsGrounded", false);

        }
        if (rb.velocity.y > 0f && !IsGrounded())
        {
            //animatorr.SetBool("Falling", false);

            animator.SetBool("Jumping", true);
        }
        /*else if (rb.velocity.y < 0f && !IsGrounded())
        {
            animatorr.SetBool("Jumping", false);

            animatorr.SetBool("Falling", true);
        }*/
        else
        {
            animator.SetBool("Jumping", false);
        }
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            animator.SetBool("isMoving", false);

            if (attackTimer <= 0f && !isAttacking)
            {
                StartAttack();
            }
        }
        else if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetBool("isMoving", false);
        }

        attackTimer -= Time.deltaTime;
    }

    void ChasePlayer()
    {
        animator.SetBool("isMoving", true);

        Vector2 direction = (player.position - transform.position).normalized;

        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        bool isObstacleAhead = Physics2D.Raycast(obstacleCheck.position, Vector2.right * Mathf.Sign(direction.x), obstacleCheckDistance, groundLayer);

        // Flip the sprite based on direction
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        // Jump if there's an obstacle or a gap
        if (!isGroundAhead)
        {
            Jump();
        }
        else if (isObstacleAhead)
        {
            Jump();
        }
        else
        {
            // Move horizontally
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        }
    }

    void Jump()
    {
        if (IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    void StartAttack()
    {
        isAttacking = true;
        rb.velocity = new Vector2(0, rb.velocity.y); // Stop horizontal movement
        animator.SetTrigger("Attack");
    }

    public void PerformAttack()
    {
        // Called via Animation Event during the attack animation
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);

        if (hitPlayer != null)
        {
            // Damage the player
            /*/PlayerHealth playerHealth = hitPlayer.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }*/
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        attackTimer = attackCooldown;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }

        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }

        if (obstacleCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(obstacleCheck.position, obstacleCheck.position + Vector3.right * obstacleCheckDistance * Mathf.Sign(transform.localScale.x));
        }
    }

    /*public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead == true)
        {
            return;
        }
        else if (isDead == false)
        {
            isDead = true;
            animator.SetTrigger("Dead");

        }
    }*/
}

