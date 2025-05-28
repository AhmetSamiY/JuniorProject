using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BossController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float chaseRange;
    public float attackRange;

    [Header("Attack")]
    public float attackCooldown;
    public int attackDamage;
    public bool Stage2;

    [Header("References")]
    public Transform player;
    public Transform attackPoint;
    public float attackRadius;
    public LayerMask playerLayer;

    public bool isAttacking = false;
    public float attackTimer = 0f;

    private Animator animator;
    private Rigidbody2D rb;

    public float maxHealth; // Maximum health of the enemy
    public float currentHealth;
    public GameObject hpBarPrefab;
    public Image hpFill;
    private Transform hpBarInstance;
    public bool dead;


    [Header("Damage Feedback")]
    public Color damageColor = new Color(1f, 0f, 0f, 0.6f); // Color when damaged (R, G, B, Alpha)
    public float colorChangeDuration = 0.2f; // Duration the color stays changed
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    private Color originalColor; // Store the original color of the enemy


    public GameObject arrowPrefab; // Drag your arrow prefab here
    public Transform[] arrowSpawnPoints; // Points where arrows spawn
    public float arrowSpawnInterval; // Interval between arrows
    public int numberOfArrows = 1; // Total arrows in the rain
    public Transform ThunderSpawn;
    public GameObject Thunder; // Drag your arrow prefab here
    public GameObject WaveObject;
    void Start()
    {
        currentHealth = maxHealth;

        GameObject bar = Instantiate(hpBarPrefab, transform);
        bar.transform.localPosition = new Vector3(0, 2f, 0);

        hpBarInstance = bar.transform;
        hpFill = bar.transform.Find("HPBar_BG/HPBar_Fill").GetComponent<Image>();

        UpdateHPBar();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // Save the original color
        }

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
        if (dead) return;
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;

        if (direction.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(1, 1, 1);
        if (distanceToPlayer <= attackRange)
        {
            animator.SetBool("isMoving", false);

            if (attackTimer <= 0f && !isAttacking)
            {
                if (Stage2 == true)
                {
                    Stage2Attacks();
                }
                else
                {
                    StartAttack();
                }
            }
        }
        else if (distanceToPlayer > attackRange && distanceToPlayer < 3f)
        {
            if (attackTimer <= 0f && !isAttacking)
            {
                MovingAttack();
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
            transform.localScale = new Vector3(-1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(1, 1, 1);

        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

    }


    void StartAttack()
    {

        Vector2 direction = (player.position - transform.position).normalized;

        if (direction.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(1, 1, 1);
        isAttacking = true;
        rb.velocity = new Vector2(0, rb.velocity.y); // Stop horizontal movement
        animator.SetTrigger("Attack");
        animator.SetInteger("BossAttack", Random.Range(0, 2));


    }
    void MovingAttack()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        if (direction.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(1, 1, 1);
        isAttacking = true;
        animator.SetTrigger("Attack");
        animator.SetInteger("BossAttack", 5);
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

    void Stage2Attacks()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        if (direction.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(1, 1, 1);
        isAttacking = true;
        rb.velocity = new Vector2(0, rb.velocity.y); // Stop horizontal movement
        animator.SetTrigger("Attack");
        animator.SetInteger("BossAttack", Random.Range(0, 5));

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
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHPBar();

        Debug.Log($"{gameObject.name} took {damage} damage! Remaining health: {currentHealth}");
        if (spriteRenderer != null)
        {
            StartCoroutine(ShowDamageEffect());
        }
        // Check if the enemy is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void UpdateHPBar()
    {
        if (hpFill != null && maxHealth > 0f)
        {
            hpFill.fillAmount = currentHealth / maxHealth;
            Debug.Log($"Health: {currentHealth} / {maxHealth}, Fill: {currentHealth / maxHealth}");

        }
    }
    private IEnumerator ShowDamageEffect()
    {
        spriteRenderer.color = damageColor;

        yield return new WaitForSeconds(colorChangeDuration);

        spriteRenderer.color = originalColor;
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


    public void CallArrowRain()
    {
        StartCoroutine(SpawnArrowRain());
    }

    private System.Collections.IEnumerator SpawnArrowRain()
    {
        for (int i = 0; i < numberOfArrows; i++)
        {
            foreach (Transform spawnPoint in arrowSpawnPoints)
            {
                Instantiate(arrowPrefab, spawnPoint.position, Quaternion.identity);
            }
            yield return new WaitForSeconds(arrowSpawnInterval);
        }
    }

    void SpawnThunder()
    {
        Instantiate(Thunder, ThunderSpawn.position, Quaternion.identity);

    }
    void SpawnWave()
    {
        GameObject spawnedObject = Instantiate(WaveObject, ThunderSpawn.position, transform.rotation);

    }
}
    
