using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    public int damage = 10;
    Animator animator;
    public bool DamageCalled;
    private void Start()
    {
        animator = GetComponent<Animator>();
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
    void Update()
    {
        
    }
}
