using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public int dropIndex; // 0 to 3, set by Enemy.cs

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            switch (dropIndex)
            {
                case 0:
                    playerController.CannonFired = false;
                    break;
                case 1:
                    playerController.UsedFlameThrower = false;
                    break;
                case 2:
                    playerController.UsedHologram = false;
                    break;
                case 3:
                    playerController.DroneCalled = false;
                    break;
            }

            Destroy(gameObject); // Remove the drop after pickup
        }
    }
}

