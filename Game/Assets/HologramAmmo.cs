using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramAmmo : MonoBehaviour
{
    public float speed;
    public float lifeTime;

    public int attackDamage = 50;

    public LayerMask enemyLayers; // Define the layer enemies are on

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        BossController BossHP = collision.GetComponent<BossController>();
        OlderEnemy olderEnemyHP = collision.GetComponent<OlderEnemy>();
        EnemySamurai EnemySamuraiHP = collision.GetComponent<EnemySamurai>();
        EnemyArcher ArcherHP = collision.GetComponent<EnemyArcher>();
        if (BossHP != null)
        {
            BossHP.TakeDamage(attackDamage); Destroy(gameObject);

        }
        else if (olderEnemyHP != null)
        {
            olderEnemyHP.TakeDamage(attackDamage); Destroy(gameObject);

        }
        else if (ArcherHP != null)
        {
            ArcherHP.TakeDamage(attackDamage); Destroy(gameObject);

        }
        else if (EnemySamuraiHP != null)
        {
            EnemySamuraiHP.TakeDamage(attackDamage);
            Destroy(gameObject);

        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
