using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResourceManager : MonoBehaviour
{
    // =========================
    // RESOURCES
    // =========================
    public Dictionary<ResourceType, float> resources = new Dictionary<ResourceType, float>();

    public float souls;
    public float gold;
    public float combatPower;
    public float trueSouls;
    public float lives;

    // =========================
    // SYSTEMS
    // =========================
    public List<Generator> generators = new List<Generator>();
    public List<Upgrade> upgrades = new List<Upgrade>();

    float timer = 0f;
    int enemyLevel = 1;
    bool dragonAttempted = false;

    // =========================
    // EVENT SYSTEM
    // =========================
    public delegate void ResourceChangedHandler();
    public event ResourceChangedHandler OnResourcesChanged;

    void Start()
    {
        // Starting resources
        resources[ResourceType.Souls] = 0;
        resources[ResourceType.Gold] = 0;
        resources[ResourceType.CombatPower] = 10;
        resources[ResourceType.TrueSoul] = 0;
        resources[ResourceType.Lives] = 3;

        // Starting generator
        generators.Add(new GoldMine());

        UpdateInspectorValues();
        FireResourceEvent();

        Debug.Log("Game Started");
    }

    void Update()
    {
        // Click anywhere = gain souls
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            AddResource(ResourceType.Souls, 1);
        }

        // Passive generator loop
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            foreach (var gen in generators)
            {
                float value = resources[gen.resourceType];
                gen.Produce(ref value); // ref parameter requirement
                resources[gen.resourceType] = value;
            }

            UpdateInspectorValues();
            FireResourceEvent();

            timer = 0f;
        }
    }

    // =========================
    // RESOURCE METHODS
    // =========================
    public void AddResource(ResourceType type, float amount)
    {
        resources[type] += amount;

        UpdateInspectorValues();
        FireResourceEvent();
    }

    void UpdateInspectorValues()
    {
        souls = Mathf.FloorToInt(resources[ResourceType.Souls]);
        gold = Mathf.FloorToInt(resources[ResourceType.Gold]);
        combatPower = Mathf.FloorToInt(resources[ResourceType.CombatPower]);
        trueSouls = Mathf.FloorToInt(resources[ResourceType.TrueSoul]);
        lives = Mathf.FloorToInt(resources[ResourceType.Lives]);
    }

    void FireResourceEvent()
    {
        OnResourcesChanged?.Invoke();
    }

    // =========================
    // SHOP
    // =========================
    public void BuyLife()
    {
        if (resources[ResourceType.Gold] >= 5000 &&
            resources[ResourceType.Lives] < 3)
        {
            resources[ResourceType.Gold] -= 5000;
            resources[ResourceType.Lives] += 1;

            UpdateInspectorValues();
            FireResourceEvent();
        }
    }

    public void BuyCombatPower()
    {
        if (resources[ResourceType.Souls] >= 10)
        {
            resources[ResourceType.Souls] -= 10;
            resources[ResourceType.CombatPower] += 1;

            UpdateInspectorValues();
            FireResourceEvent();
        }
    }

    // =========================
    // UPGRADE SYSTEM
    // =========================
    public bool BuyUpgrade(int index, out string message)
    {
        message = "";

        try
        {
            if (index < 0 || index >= upgrades.Count)
            {
                message = "Invalid upgrade";
                return false;
            }

            Upgrade u = upgrades[index];
            float cost = u.GetCost();

            if (resources[u.costResource] < cost)
            {
                message = "Not enough " + u.costResource;
                return false;
            }

            // Pay cost
            resources[u.costResource] -= cost;

            // Apply effect
            ApplyUpgradeEffect(u);

            u.level++;
            u.state = UpgradeState.Purchased;

            UpdateInspectorValues();
            FireResourceEvent();

            message = "Purchased " + u.name;
            return true;
        }
        catch (Exception ex)
        {
            message = "Upgrade failed: " + ex.Message;
            Debug.LogError(message);
            return false;
        }
    }

    void ApplyUpgradeEffect(Upgrade upgrade)
    {
        if (upgrade.effect.targetResource == ResourceType.Souls)
        {
            generators.Add(new SoulWell(upgrade.effect.multiplier));
        }

        if (upgrade.effect.targetResource == ResourceType.Gold)
        {
            generators.Add(new GoldMine(upgrade.effect.multiplier));
        }
    }

    // =========================
    // COMBAT
    // =========================
    public void StartFight()
    {
        int roll = UnityEngine.Random.Range(1, 21);

        float playerPower =
            resources[ResourceType.CombatPower] * roll;

        float enemyPower =
            Mathf.Pow(1.5f, enemyLevel) * 10;

        Debug.Log("Player: " + playerPower +
                  " Enemy: " + enemyPower);

        if (playerPower >= enemyPower)
        {
            Debug.Log("WIN");

            resources[ResourceType.TrueSoul] += 1;
            enemyLevel++;

            if (resources[ResourceType.TrueSoul] >= 10)
            {
                FightDragon();
            }
        }
        else
        {
            Debug.Log("LOSE");

            resources[ResourceType.Lives] -= 1;

            if (resources[ResourceType.Lives] <= 0)
            {
                GameOver();
            }
        }

        UpdateInspectorValues();
        FireResourceEvent();
    }

    void FightDragon()
    {
        if (dragonAttempted) return;

        dragonAttempted = true;

        float player =
            resources[ResourceType.CombatPower] *
            UnityEngine.Random.Range(1, 21);

        float dragon = 500;

        if (player >= dragon)
        {
            Debug.Log("YOU BEAT THE DRAGON (TRUE END)");
        }
        else
        {
            Debug.Log("DRAGON DEFEATED YOU");
        }

        GameOver();
    }

    // =========================
    // RESET
    // =========================
    void GameOver()
    {
        Debug.Log("GAME OVER");

        resources[ResourceType.Souls] = 0;
        resources[ResourceType.Gold] = 0;
        resources[ResourceType.CombatPower] = 10;
        resources[ResourceType.TrueSoul] = 0;
        resources[ResourceType.Lives] = 3;

        enemyLevel = 1;
        dragonAttempted = false;

        generators.Clear();
        generators.Add(new GoldMine());

        UpdateInspectorValues();
        FireResourceEvent();
    }
}