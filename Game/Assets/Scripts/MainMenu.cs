using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject player;

    public void StartButton()
    {
        SceneManager.LoadScene("Level1");
    }

    public void ExitButton()
    {
        Debug.Log("Game Closed!"); 
        Application.Quit();
    }
    public void SaveGame()
    {
        PlayerPrefs.SetString("SavedScene", SceneManager.GetActiveScene().name);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 playerPos = player.transform.position;
            PlayerPrefs.SetFloat("PlayerX", playerPos.x);
            PlayerPrefs.SetFloat("PlayerY", playerPos.y);
            PlayerPrefs.SetFloat("PlayerZ", playerPos.z);

            if (player.TryGetComponent(out PlayerController health))
            {
                PlayerPrefs.SetInt("PlayerHP", health.currentHealth);
            }
        }

        PlayerPrefs.Save();
        Debug.Log("Game Saved!");
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("SavedScene") && PlayerPrefs.GetString("SavedScene") == SceneManager.GetActiveScene().name)
        {
            float x = PlayerPrefs.GetFloat("PlayerX");
            float y = PlayerPrefs.GetFloat("PlayerY");
            float z = PlayerPrefs.GetFloat("PlayerZ");
            player.transform.position = new Vector3(x, y, z);

            // Load Player Health
            if (player.TryGetComponent(out PlayerController controller))
            {
                controller.currentHealth = PlayerPrefs.GetInt("PlayerHealth", controller.currentHealth);
            }
        }
    }
}

