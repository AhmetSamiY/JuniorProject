using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRain : MonoBehaviour
{
    public int damage = 10;
    private bool hasHitGround = false; // Track if the arrow has hit the ground
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().TakeDamage(damage);
        }
        else if (collision.CompareTag("Ground") && !hasHitGround)
        {
            StickToGround();
            Destroy(gameObject, 3f);
        }
    }

    private void StickToGround()
    {
        hasHitGround = true;

        // Disable the arrow's Rigidbody2D to stop movement
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            animator.SetBool("TouchedGround", true);
            rb.velocity = Vector2.zero; // Stop any movement
            rb.angularVelocity = 0; // Stop rotation
            rb.isKinematic = true; // Prevent further physics interaction
        }

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }
}