using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoolDowns : MonoBehaviour
{
    public TextMeshProUGUI shurikenAmountText;
    public TextMeshProUGUI arrowAmountText;
    public PlayerController playerController;

    // Ability icons
    public Image flameThrowerIcon;
    public Image cannonIcon;
    public Image hologramIcon;
    public Image droneIcon;

    // Faded color (black with some transparency)
    private Color fadedColor = new Color(1f, 1f, 1f, 0.5f);
    private Color normalColor = Color.white;

    void Start()
    {
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        UpdateAmountDisplay();
    }

    void Update()
    {
        UpdateAmountDisplay();
        UpdateAbilityIcons();
    }

    void UpdateAmountDisplay()
    {
        if (shurikenAmountText != null)
            shurikenAmountText.text = playerController.ShurikenCount.ToString();

        if (arrowAmountText != null)
            arrowAmountText.text = playerController.arrowCount.ToString();
    }

    void UpdateAbilityIcons()
    {
        if (flameThrowerIcon != null)
            SetIconColor(flameThrowerIcon, playerController.UsedFlameThrower);

        if (cannonIcon != null)
            SetIconColor(cannonIcon, playerController.CannonFired);

        if (hologramIcon != null)
            SetIconColor(hologramIcon, playerController.UsedHologram);

        if (droneIcon != null)
            SetIconColor(droneIcon, playerController.DroneCalled);
    }

    void SetIconColor(Image icon, bool used)
    {
        icon.color = used ? fadedColor : normalColor;
    }
}
