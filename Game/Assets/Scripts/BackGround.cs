using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    /* [SerializeField] private float length;
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
        startPosY = transform.position.y + offsetY; // Apply the Y offset
        lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
        lengthY = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {
        // Calculate the parallax effect for both X and Y axes
        float distanceX = mainCamera.transform.position.x * (1 - parallaxEffectX);
        float distanceY = mainCamera.transform.position.y * (1 - parallaxEffectY);

        // Apply the effect on X and Y positions, including the Y offset
        transform.position = new Vector3(startPosX + (mainCamera.transform.position.x * parallaxEffectX),
                                          startPosY + (mainCamera.transform.position.y * parallaxEffectY),
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

        if (distanceY > startPosY + lengthY)
        {
            startPosY += lengthY;
        }
        else if (distanceY < startPosY - lengthY)
        {
            startPosY -= lengthY;
        }
    }
}
