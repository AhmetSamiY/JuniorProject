using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoolDowns : MonoBehaviour
{
    public TextMeshProUGUI shurikenAmountText; // Displays the shuriken count
    public TextMeshProUGUI arrowAmountText;    // Displays the arrow count
    public PlayerController playerController;

    void Start()
    {
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        UpdateAmountDisplay();
    }

    void Update()
    {
        UpdateAmountDisplay();
    }

    void UpdateAmountDisplay()
    {
        // Update shuriken amount text
        if (shurikenAmountText != null)
        {
            shurikenAmountText.text = playerController.ShurikenCount.ToString();
        }

        // Update arrow amount text
        if (arrowAmountText != null)
        {
            arrowAmountText.text = playerController.arrowCount.ToString();
        }
    }
}