using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{

    [Header("Parameters")]
    [SerializeField] private Transform RayCast;
    [SerializeField] private LayerMask RayCastMask;
    [SerializeField] private float RayCastLength;
    [SerializeField] private float AttackDistance;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float timer;

    [Header("Variables")]
    [SerializeField] private RaycastHit2D hit;
    [SerializeField] private GameObject Target;
    [SerializeField] private Animator Animatorr;
    [SerializeField] private float distance;
    [SerializeField] private bool AttackMode;
    [SerializeField] private bool inRange;
    [SerializeField] private bool Cooling;
    [SerializeField] private float intTimer;

    void Start()
    {
        
    }

    void Awake()
    {
        intTimer = timer;
        Animatorr = GetComponent<Animator>();
    }

    void Update()
    {
        if (inRange)
        {
            hit = Physics2D.Raycast(RayCast.position, Vector2.right, RayCastLength, RayCastMask);
            RayCastDebugger();
        }

        if (hit.collider != null)
        {
            EnemyLogic();
        }

        else if (hit.collider == null)
        {
            inRange = false;
        }

        if (inRange == false)
        {
            Animatorr.SetBool("CanWalk", false);
            StopAttack();
        }
    }

    void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, Target.transform.position);

        if (distance > AttackDistance)
        {
            Move();
            StopAttack();
        }
        
        else if (distance < AttackDistance && Cooling ==false)
        {
            Attack();
        }
        
        if (Cooling)
        {
            Cooldown();
            Animatorr.SetBool("Attack", false );
        }
    }

    void Move()
    {
        Animatorr.SetBool("CanWalk", true);
        if (!Animatorr.GetCurrentAnimatorStateInfo(0).IsName("Attack")) 
        {
            Vector2 targetPosition = new Vector2(Target.transform.position.x, transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);

        }
    }

    void Attack()
    {
        timer = intTimer;
        AttackMode = true;

        Animatorr.SetBool("CanWalk", false);
        Animatorr.SetBool("Attack", true);
    }

    void StopAttack()
    {
        Cooling = false;
        AttackMode= false;
        Animatorr.SetBool("Attack", false);
    }

    void Cooldown()
    {
        timer -= Time.deltaTime;

        if (timer < 0 && Cooling && AttackMode)
        {
            Cooling=false;
            timer = intTimer;
        }

    }

    void TriggerCooling()
    {
        Cooling=true;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Target = collision.gameObject;
            inRange = true;
        }
    }
    void RayCastDebugger()
    {
        if (distance > AttackDistance)
        {
            Debug.DrawRay(RayCast.position, Vector2.right * RayCastLength, Color.red);
        }
        else if (distance < AttackDistance)
        {
            Debug.DrawRay(RayCast.position, Vector2.right * RayCastLength, Color.green);
        }

    }
}
