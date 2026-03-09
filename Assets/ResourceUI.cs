using UnityEngine;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    public ResourceManager resourceManager;
    public TMP_Text resourceText;

    void Update()
    {
        resourceText.text =
            "Souls: " + Mathf.FloorToInt(resourceManager.souls) +
            "\nGold: " + Mathf.FloorToInt(resourceManager.gold);
    }
}