using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    public float explosionRadius = 3f;
    public float explosionForce = 500f;
    public int damage = 50;
    public GameObject explosionEffect;

    public LayerMask playerLayer;
    public LayerMask enemyLayer;

    private bool exploded = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!exploded && ((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            Explode(other.transform);
        }
    }

    void Explode(Transform player)
    {
        exploded = true;

        // Visual FX
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Deal with enemies
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);
        foreach (Collider2D enemy in enemies)
        {
            // Apply damage
            var enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.TakeDamage(damage);

            // Optional: add force to enemy
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 dir = (enemy.transform.position - transform.position).normalized;
                enemyRb.AddForce(dir * explosionForce);
            }
        }

        // Push the player
        Collider2D[] players = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);
        foreach (Collider2D p in players)
        {
            Rigidbody2D rb = p.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = (rb.transform.position - transform.position).normalized;
                rb.AddForce(dir * explosionForce);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
