using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    [SerializeField] private float length;
    [SerializeField] private float startPosX;
    [SerializeField] private float startPosY;
    [SerializeField] private GameObject player; // Reference to the player
    [SerializeField] private float parallaxEffectX; // Parallax effect for X-axis
    [SerializeField] private float parallaxEffectY; // Parallax effect for Y-axis

    void Start()
    {
        startPosX = transform.position.x;
        startPosY = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x; // Get the width of the background
    }

    void Update()
    {
        // Get the player's position on both axes
        float playerX = player.transform.position.x;
        float playerY = player.transform.position.y;

        // Calculate the distance the background should move based on the player's movement
        float distX = (playerX * parallaxEffectX);
        float distY = (playerY * parallaxEffectY);

        // Update the background's position
        transform.position = new Vector3(startPosX + distX, startPosY + distY, transform.position.z);

        // Check if the background has moved far enough to repeat its position (looping effect)
        if (playerX * (1 - parallaxEffectX) > startPosX + length)
        {
            startPosX += length;
        }
        else if (playerX * (1 - parallaxEffectX) < startPosX - length)
        {
            startPosX -= length;
        }

        // Loop the Y-axis if needed (in case you want looping or repositioning for the vertical background)
        if (playerY * (1 - parallaxEffectY) > startPosY + length)
        {
            startPosY += length;
        }
        else if (playerY * (1 - parallaxEffectY) < startPosY - length)
        {
            startPosY -= length;
        }
    }
        /*
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

            transform.position = new Vector3(StartPos + dist, StartPos, transform.position.z);

            if (temp > StartPos + length)
            {
                StartPos += length;
            }
            else if (temp < StartPos - length)
            {
                StartPos -= length;
            }
        }*/

    }
