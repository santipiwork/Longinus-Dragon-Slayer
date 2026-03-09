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
            "\nCost: " + u.GetCost().ToString("F0") +
            "\nLevel: " + u.level;
    }

    public void Buy()
    {
        manager.BuyUpgrade(upgradeIndex);
    }
}