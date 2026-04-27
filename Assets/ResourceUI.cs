using UnityEngine;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    public ResourceManager resourceManager;
    public TMP_Text resourceText;

    void Start()
    {
        // Subscribe to event
        resourceManager.OnResourcesChanged += UpdateUI;

        // Update once when game starts
        UpdateUI();
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent errors
        if (resourceManager != null)
        {
            resourceManager.OnResourcesChanged -= UpdateUI;
        }
    }

    void UpdateUI()
    {
        resourceText.text =
            "Souls: " + Mathf.FloorToInt(resourceManager.souls) +
            "\nGold: " + Mathf.FloorToInt(resourceManager.gold) +
            "\nCombatPower: " + Mathf.FloorToInt(resourceManager.combatPower) +
            "\nLives: " + Mathf.FloorToInt(resourceManager.lives);
    }
}