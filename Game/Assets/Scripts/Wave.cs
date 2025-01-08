using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    public int damage = 10;
    public float speed = 5f; // Speed of movement

    private Animator animator;
    public bool DamageCalled;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Move the object in the direction of its rotation
        transform.Translate(Vector2.right * speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && DamageCalled)
        {
            collision.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }

    void CallDamage()
    {
        DamageCalled = true;
    }

    void destroytheobject()
    {
        Destroy(gameObject);
    }
}
