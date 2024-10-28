using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    [SerializeField] private float OffSetX;

    void Start()
    {

    }

    void Update()
    {

        follow();
    }

    private void follow()
    {
        transform.position = new Vector3(player.position.x + OffSetX, 0, -10);

    }
}
