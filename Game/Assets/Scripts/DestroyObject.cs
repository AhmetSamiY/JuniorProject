using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public float destroyTime;
    void Start()
    {
    }

    void Update()
    {
        
    }

    void DestroyFlame()
    {
        Destroy(gameObject);
    }
}
