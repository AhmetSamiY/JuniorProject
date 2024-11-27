using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySamurai : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float chaseRange;
    public float attackRange;

    [Header("Attack")]
    public float attackCooldown;
    public int attackDamage;

    [Header("References")]
    public Transform player;
    public Transform attackPoint;
    public float attackRadius;
    public LayerMask playerLayer;

    public bool isAttacking = false;
    public float attackTimer = 0f;

    public Animator animator;
    public Rigidbody2D rb;

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
        Vector2 direction = (player.position - transform.position).normalized;
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            animator.SetBool("isMoving", false);

            // Attack the player
            if (attackTimer <= 0f && !isAttacking)
            {
                StartAttack();

            }
        }
        else if (distanceToPlayer <= chaseRange)
        {
            // Chase the player
            ChasePlayer();
        }
        else
        {
            // Stop moving
            rb.velocity = Vector2.zero;
            animator.SetBool("isMoving",false);
        }

        attackTimer -= Time.deltaTime;
    }

    void ChasePlayer()
    {
        animator.SetBool("isMoving", true);

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

        // Flip the sprite based on direction
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void StartAttack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero; // Stop movement
        animator.SetBool("isMoving", true);
        animator.SetTrigger("Attack");
    }

    public void PerformAttack()
    {
        // Called via Animation Event during the attack animation
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);

        /*if (hitPlayer != null)
        {
            // Damage the player
            PlayerHealth playerHealth = hitPlayer.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }*/
    }

    public void EndAttack()
    {
        // Called at the end of the attack animation
        isAttacking = false;
        attackTimer = attackCooldown;
        animator.SetBool("isMoving", true);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}

