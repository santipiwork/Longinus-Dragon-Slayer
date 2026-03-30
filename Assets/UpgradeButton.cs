using UnityEngine;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    public ResourceManager manager;
    public int upgradeIndex;

    public TMP_Text buttonText;

    void Update()
    {
        Upgrade u = manager.upgrades[upgradeIndex];

        buttonText.text =
            u.name +
            "\nCost: " + Mathf.FloorToInt(u.GetCost()) + " " + u.costResource +
            "\nLevel: " + u.level;
    }

    public void Buy()
    {
        string message;
        bool success = manager.BuyUpgrade(upgradeIndex, out message);

        Debug.Log(message);
    }
}