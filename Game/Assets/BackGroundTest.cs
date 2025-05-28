using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundTest : MonoBehaviour
{
    public float parallaxEffectX;  // The parallax effect for X-axis
    public float parallaxEffectY;  // The parallax effect for Y-axis
    public float offsetY;          // The offset for the Y-axis (optional)

    private float lengthX, lengthY, startPosX, startPosY;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Get the initial position and size of the sprite
        startPosX = transform.position.x;
        startPosY = transform.position.y;
        lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
        lengthY = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {
        // Calculate the parallax distance
        float distanceX = mainCamera.transform.position.x * (1 - parallaxEffectX);
        float distanceY = mainCamera.transform.position.y * (1 - parallaxEffectY);

        // Move the background
        transform.position = new Vector3(
            startPosX + (mainCamera.transform.position.x * parallaxEffectX),
            startPosY + (mainCamera.transform.position.y * parallaxEffectY) + offsetY,
            transform.position.z);

        // Repeat background horizontally
        if (distanceX > startPosX + lengthX)
        {
            startPosX += lengthX;
        }
        else if (distanceX < startPosX - lengthX)
        {
            startPosX -= lengthX;
        }
    }
}
