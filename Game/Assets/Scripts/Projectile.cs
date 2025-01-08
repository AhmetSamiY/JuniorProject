using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed;

    public int attackDamage = 20;
    void Start()
    {
        Destroy(gameObject,5);
    }

    void Update()
    {
        transform.position += transform.right * Time.deltaTime * Speed;
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
