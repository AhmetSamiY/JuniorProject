using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrone : MonoBehaviour
{
    public GameObject rocketPrefab;
    public Transform firePoint;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void SpawnRocket()
    {
        Instantiate(rocketPrefab, firePoint.position, firePoint.rotation);
    }

    void SpawnRocketRight()
    {
        Instantiate(rocketPrefab, firePoint.position, Quaternion.Euler(0,-180, 0));
    }

    void DestroyDrone()
    {
        Destroy(gameObject);
    }
}

