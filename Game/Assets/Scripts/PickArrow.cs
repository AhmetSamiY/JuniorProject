using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickArrow : MonoBehaviour
{
    private bool playerInRange;
    private PlayerController player; // Reference to the Player script

    void Update()
    {
        // Check if the player presses F to collect the arrow
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            CollectArrow();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            player = collision.GetComponent<PlayerController>(); // Get the Player script
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }

    private void CollectArrow()
    {
        if (player != null)
        {
            player.AddArrow(); // Add arrow to the player's inventory
            Destroy(gameObject); // Remove the arrow from the scene
        }
    }
}


