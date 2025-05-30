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

    public int maxHealth; // Maximum health of the enemy
    public int currentHealth;
    public bool dead;

    public GameObject portalPrefab;
    public GameObject enemyOldPrefab;
    public Transform spawnOffsetBehind;


    public Sprite[] dropImages; // Assign 4 images
    public GameObject imageDropPrefab;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip arrowShotClip;
    public AudioClip deathClip;
    public AudioClip damageTakenClip;
    public AudioClip katanaClip1;
    public AudioClip katanaClip2;

    private int katanaToggle = 0;

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
        // Start the 30-second check coroutine when spawned
        StartCoroutine(CheckAndSpawnOldAfterTime(30f));
    }


    void Update()
    {
        if (dead == true) return;
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
        Vector2 direction = (player.position - transform.position).normalized;

        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        isAttacking = true;
        rb.velocity = new Vector2(0, rb.velocity.y); // Stop horizontal movement
        animator.SetTrigger("Attack");
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
    public void TakeDamage(int damage)
    {
        if (dead) return;
        currentHealth -= damage;
        animator.SetTrigger("HitTaken");
        Debug.Log($"{gameObject.name} took {damage} damage! Remaining health: {currentHealth}");

        // Check if the enemy is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private IEnumerator CheckAndSpawnOldAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!dead)
        {   
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Static;
            }
            Die();
            GameObject portal = Instantiate(portalPrefab, spawnOffsetBehind.position, Quaternion.identity);
            StartCoroutine(SpawnEnemyOldWithDelay(portal.transform.position, portal));
        }
    }


    private IEnumerator SpawnEnemyOldWithDelay(Vector3 spawnPosition, GameObject portal)
    {
        yield return new WaitForSeconds(1.5f); // Wait for portal animation
        Instantiate(enemyOldPrefab, spawnPosition, Quaternion.identity);
        Destroy(portal); // Clean up the portal
    }
    
    public void PlayArrowShot()
    {
        audioSource.PlayOneShot(arrowShotClip);
    }

    public void PlayDeath()
    {
        audioSource.PlayOneShot(deathClip);
    }

    public void PlayDamageTaken()
    {
        audioSource.PlayOneShot(damageTakenClip);
    }

    public void PlayKatana()
    {
        // Alternate between the two katana sounds
        AudioClip clip = katanaToggle == 0 ? katanaClip1 : katanaClip2;
        audioSource.PlayOneShot(clip);
        katanaToggle = 1 - katanaToggle;
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

        int dropIndex = Random.Range(0, dropImages.Length);
        GameObject dropped = Instantiate(imageDropPrefab, transform.position, Quaternion.identity);

        SpriteRenderer sr = dropped.GetComponent<SpriteRenderer>();
        sr.sprite = dropImages[dropIndex];

        // Tell the drop which index it is
        DropItem dropItem = dropped.GetComponent<DropItem>();
        dropItem.dropIndex = dropIndex;


    }
}

