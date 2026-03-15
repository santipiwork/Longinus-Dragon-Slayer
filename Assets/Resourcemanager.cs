using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResourceManager : MonoBehaviour
{
    // Resource storage using enum instead of strings
    public Dictionary<ResourceType, float> resources = new Dictionary<ResourceType, float>();

    // Inspector display values
    public float souls;
    public float gold;

    // Passive income
    public float passiveSouls = 0;
    public float passiveGold = 0.5f;

    // Upgrade system
    public List<Upgrade> upgrades = new List<Upgrade>();

    float timer = 0f;

    void Start()
    {
        // Initialize resources
        resources[ResourceType.Souls] = 0;
        resources[ResourceType.Gold] = 0;

        // Create upgrades
        upgrades.Add(new Upgrade(
            "Soul Echo",
            10,
            1,
            new UpgradeEffect(ResourceType.Souls, 1)
        ));

        upgrades.Add(new Upgrade(
            "Sword Spirit",
            25,
            1,
            new UpgradeEffect(ResourceType.Gold, 1)
        ));

        Debug.Log("Resource Manager Started");
    }

    void Update()
    {
        // Click anywhere to gain Souls
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            AddResource(ResourceType.Souls, 1);
        }

        // Passive income timer
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            AddResource(ResourceType.Souls, passiveSouls);
            AddResource(ResourceType.Gold, passiveGold);
            timer = 0f;
        }

        // Sync values to Inspector (no decimals)
        souls = Mathf.FloorToInt(resources[ResourceType.Souls]);
        gold = Mathf.FloorToInt(resources[ResourceType.Gold]);
    }

    public void AddResource(ResourceType type, float amount)
    {
        if (resources.ContainsKey(type))
        {
            resources[type] += amount;
            Debug.Log(type + " = " + resources[type]);
        }
    }

    public void BuyUpgrade(int index)
    {
        if (index >= upgrades.Count) return;

        Upgrade u = upgrades[index];
        float cost = u.GetCost();

        if (resources[u.costResource] >= cost)
        {
            resources[u.costResource] -= cost;

            ApplyUpgradeEffect(u);

            u.level++;
            u.state = UpgradeState.Purchased;

            Debug.Log("Purchased " + u.name);
        }
        else
        {
            Debug.Log("Not enough " + u.costResource);
        }
    }

    void ApplyUpgradeEffect(Upgrade upgrade)
    {
        if (upgrade.effect.targetResource == ResourceType.Souls)
        {
            passiveSouls += upgrade.effect.multiplier;
        }

        if (upgrade.effect.targetResource == ResourceType.Gold)
        {
            passiveGold += upgrade.effect.multiplier;
        }
    }
}