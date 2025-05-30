using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyArcher : MonoBehaviour
{
    public Sprite[] dropImages; // Assign 4 images
    public GameObject imageDropPrefab;

    public Transform player;
    public GameObject arrowPrefab;
    public GameObject SpecialarrowPrefab;

    public float shootInterval;
    public float shootForce;
    public float detectionRange;
    public float moveSpeed;
    public Transform pointA;
    public Transform pointB;

    private float lastShotTime;
    public bool movingToB = true;
    public GameObject arrowDropPrefab;
    public Rigidbody2D rb;
    Animator animatorr;

    [Header("Dash Settings")]
    public float dashSpeed = 10f; // Speed of the dash
    public float dashDuration = 0.2f; // How long the dash lasts
    public float dashCooldown = 2f; // Cooldown between dashes

    [Header("Detection Settings")]
    public float dashdetectionrange = 5f; // Range to detect the player

    private Transform playertagged;
    private bool canDash = true;
    private Vector2 dashDirection;
    private float dashTimer = 0f;
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer

    public int maxHealth; // Maximum health of the enemy
    public int currentHealth;
    public bool dead;
    public bool Rolling;
    private void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playertagged = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playertagged == null)
        {
            Debug.LogError("Player not found! Make sure your Player GameObject has the 'Player' tag.");
        }
        rb = GetComponent<Rigidbody2D>();
        animatorr = GetComponent<Animator>(); 

    }
    private void Update()
    {
        if (Rolling)
        {
            return;
        }
        if (dead)
        {
            return;
        }
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            AimAtPlayer();
            animatorr.SetBool("IsMoving", false);
            //ShootArrow();
        }
        else
        {
            animatorr.SetBool("IsMoving", true);

            Patrol();
        }
        if (playertagged == null) return;


        if (distanceToPlayer <= dashdetectionrange && canDash)
        {
            StartDash();
            animatorr.SetTrigger("Roll");
        }

        // Handle the dash duration timer
        if (!canDash && dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                StopDash();
                animatorr.ResetTrigger("Roll");


            }
        }
    }

    private void AimAtPlayer()
    {
        if (Time.time - lastShotTime >= shootInterval)
        {
            float specialChance = animatorr.GetFloat("Special"); // Get current "Special" value

            if (Random.value <= specialChance)
            {
                animatorr.SetTrigger("ArcherSpecial");
            }
            else
            {
                animatorr.SetTrigger("Attack");
            }

            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, angle, 0);

            
        }
    }
   private void CheckSpecial()
    {
        float newSpecialChance = Random.Range(0f, 1f);
        animatorr.SetFloat("Special", newSpecialChance);

    }

    public Transform ArrowSpawn;
    private void ShootArrow()
    {
        if (Time.time - lastShotTime >= shootInterval)
        {
            lastShotTime = Time.time;
            // Instantiate the arrow
            GameObject arrow = Instantiate(arrowPrefab, ArrowSpawn.transform.position, Quaternion.identity);

            // Get the arrow's Rigidbody2D component and apply force
            Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
            Vector2 direction = (player.position - transform.position).normalized;
            rb.AddForce(direction * shootForce, ForceMode2D.Impulse);
            animatorr.ResetTrigger("Attack");// Flip the sprite based on dash direction
            if (direction.x > 0 && !spriteRenderer.flipX)
            {
                spriteRenderer.flipX = true;
            }
            else if (direction.x < 0 && spriteRenderer.flipX)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    public Transform SpecialArrowSpawn;
    private void SpecialShootArrow()
    {
        if (Time.time - lastShotTime >= shootInterval)
        {
            lastShotTime = Time.time;
            // Instantiate the arrow
            GameObject arrow = Instantiate(SpecialarrowPrefab, SpecialArrowSpawn.transform.position, Quaternion.identity);

            // Get the arrow's Rigidbody2D component and apply force
            Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
            Vector2 direction = (player.position - transform.position).normalized;
            rb.AddForce(direction * shootForce * 2, ForceMode2D.Impulse);
            animatorr.ResetTrigger("SpecialAttack");// Flip the sprite based on dash direction
            if (direction.x > 0 && !spriteRenderer.flipX)
            {
                spriteRenderer.flipX = true;
            }
            else if (direction.x < 0 && spriteRenderer.flipX)
            {
                spriteRenderer.flipX = false;
            }
        }
    }
    public void FixFlip()
    {
        spriteRenderer.flipX = false;

    }

    private void Patrol()
    {
        Transform targetPoint = movingToB ? pointB : pointA;

        // Move the enemy smoothly towards the target point
        float newX = Mathf.MoveTowards(transform.position.x, targetPoint.position.x, moveSpeed * Time.deltaTime);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        // Handle rotation: if moving right, face right; if moving left, face left
        if (targetPoint.position.x > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);  // Facing right
        }
        else if (targetPoint.position.x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);  // Facing left
        }

        // Switch direction when the enemy is close to the target point
        if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.1f)  // Use a small threshold (0.1f)
        {
            movingToB = !movingToB;
        }
    }

    public float HP = 10f;

    
    

    private void StartDash()
    {
        canDash = false;

        // Calculate direction AWAY from the player
        dashDirection = (transform.position - playertagged.position).normalized;

        // Flip the enemy sprite based on the dash direction
        FlipEnemy(dashDirection);

        // Apply velocity for the dash
        rb.velocity = dashDirection * dashSpeed;

        // Set dash timer
        dashTimer = dashDuration;

        // Start cooldown coroutine
        Invoke(nameof(ResetDash), dashCooldown);
    }

    private void StopDash()
    {
        rb.velocity = Vector2.zero;
        FlipEnemy(-dashDirection);
    }

    private void ResetDash()
    {
        canDash = true;
    }

    private void FlipEnemy(Vector2 direction)
    {
        // Flip the sprite based on dash direction
        if (direction.x > 0 && !spriteRenderer.flipX) // Dash right
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x < 0 && spriteRenderer.flipX) // Dash left
        {
            spriteRenderer.flipX = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection range in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dashdetectionrange);
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage! Remaining health: {currentHealth}");

        // Check if the enemy is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip arrowShotClip;
    public AudioClip deathClip;
    public AudioClip damageTakenClip;
    public AudioClip katanaClip1;
    public AudioClip katanaClip2;

    private int katanaToggle = 0;

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
        int dropIndex = Random.Range(0, dropImages.Length);
        GameObject dropped = Instantiate(imageDropPrefab, transform.position, Quaternion.identity);

        SpriteRenderer sr = dropped.GetComponent<SpriteRenderer>();
        sr.sprite = dropImages[dropIndex];

        // Tell the drop which index it is
        DropItem dropItem = dropped.GetComponent<DropItem>();
        dropItem.dropIndex = dropIndex;

        Debug.Log($"{gameObject.name} has died!");
        animatorr.SetTrigger("Dead");
    }
}
