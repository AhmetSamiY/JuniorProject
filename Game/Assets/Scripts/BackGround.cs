using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    [SerializeField] private float length;
    [SerializeField] private float StartPos;
    [SerializeField] private GameObject Cam;
    [SerializeField] private float ParallaxEffect;


    void Start()
    {
        StartPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;

    }

    void Update()
    {
        float temp = (Cam.transform.position.x * (1 - ParallaxEffect));
        float dist = (Cam.transform.position.x * ParallaxEffect);

        transform.position = new Vector3(StartPos + dist, transform.position.y, transform.position.z);

        if (temp > StartPos + length)
        {
            StartPos += length;
        }
        else if (temp < StartPos - length)
        {
            StartPos -= length;
        }
    }
}
