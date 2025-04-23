using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramAttack : MonoBehaviour
{
    public GameObject AmmoPrefab;
    public Transform FirePoint;
    public Transform FirePoint2;
    public Transform FirePoint3;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void FireRight()
    {
        Instantiate(AmmoPrefab, FirePoint.position, FirePoint.rotation);
    }

    void FireRight2()
    {
        Instantiate(AmmoPrefab, FirePoint3.position, FirePoint3.rotation);
    }

    void FireLeft()
    {
        Instantiate(AmmoPrefab, FirePoint2.position, FirePoint2.rotation);
    }

    void destroyHolo()
    {
        Destroy(gameObject);
    }
}
