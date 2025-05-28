using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{

    public float parallaxEffectX;  // The parallax effect for X-axis
    public float parallaxEffectY;  // The parallax effect for Y-axis
    public float offsetY;            // The offset for the Y-axis (e.g., for positioning at the bottom)

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
        // Calculate the parallax effect for both X and Y axes
        float distanceX = mainCamera.transform.position.x * (1 - parallaxEffectX);
        float distanceY = mainCamera.transform.position.y * (1 - parallaxEffectY);

        transform.position = new Vector3(
        startPosX + (mainCamera.transform.position.x * parallaxEffectX),
        startPosY + (mainCamera.transform.position.y * parallaxEffectY) + offsetY,
        transform.position.z);

        // Reset positions when the background is off-screen in the X and Y directions
        if (distanceX > startPosX + lengthX)
        {
            startPosX += lengthX;
        }
        else if (distanceX < startPosX - lengthX)
        {
            startPosX -= lengthX;
        }

        /*if (distanceY > startPosY + lengthY)
        {
            startPosY += lengthY;
        }
        else if (distanceY < startPosY - lengthY)
        {
            startPosY -= lengthY;
        }*/
    }
}
