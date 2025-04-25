using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OlderEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpForce;
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

    private Animator animator;
    private Rigidbody2D rb;
    public int maxHealth; // Maximum health of the enemy
    public int currentHealth;
    public bool dead;
    void Start()
    {
        currentHealth = maxHealth;

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
    {   if (dead) return;
        if (player == null) return;
        
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

        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        
    }
 

    void StartAttack()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        isAttacking = true;
        rb.velocity = new Vector2(0, rb.velocity.y); // Stop horizontal movement
        animator.SetTrigger("Attack");
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

    }
    public void PerformAttack()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);

        if (hitPlayer != null)
        {
            // Get the PlayerHealth component of the hit player
            PlayerController playerHealth = hitPlayer.GetComponent<PlayerController>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("HitTaken");

        Debug.Log($"{gameObject.name} took {damage} damage! Remaining health: {currentHealth}");

        // Check if the enemy is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (dead) return;
        dead = true;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        CapsuleCollider2D col = GetComponent<CapsuleCollider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        Debug.Log($"{gameObject.name} has died!");
        animator.SetTrigger("Dead");
    }

}
