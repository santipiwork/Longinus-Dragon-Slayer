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
    public int rerollCost = 100;

    // =========================
    // SYSTEMS
    // =========================
    public List<Generator> generators = new List<Generator>();
    public List<Upgrade> upgrades = new List<Upgrade>();

    float timer = 0f;
    int enemyLevel = 1;
    bool dragonAttempted = false;

    [Header("Win Screen")]
    public GameObject winScreen;

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
        if (resources[ResourceType.Gold] >= 500 &&
            resources[ResourceType.Lives] < 3)
        {
            resources[ResourceType.Gold] -= 500;
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

    public void BuyCombatPowerAmount(int amount)
    {
        int costPer = 10;

        int totalCost = amount * costPer;

        if (resources[ResourceType.Souls] >= totalCost)
        {
            resources[ResourceType.Souls] -= totalCost;
            resources[ResourceType.CombatPower] += amount;

            UpdateInspectorValues();
            FireResourceEvent();
        }
    }

    public void ReRollDice()
    {
        if (resources[ResourceType.Gold] >= rerollCost)
        {
            resources[ResourceType.Gold] -= rerollCost;

            hasRolled = false;
            RollDice();

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

    int currentRoll = 10;
    bool hasRolled = false;

    public void RollDice()
    {
        if (hasRolled) return;

        currentRoll = UnityEngine.Random.Range(1, 21);
        hasRolled = true;

        float multiplier = GetRollMultiplier(currentRoll);

        float rolledCombat =
            resources[ResourceType.CombatPower] * multiplier;

        CombatUI ui = FindObjectOfType<CombatUI>();

        if (ui != null)
        {
            ui.ShowRoll(currentRoll);
            ui.ShowCurrentCombat(rolledCombat);
        }

        Debug.Log("Rolled: " + currentRoll);
    }

    public void StartFight()
    {
        CombatUI ui = FindObjectOfType<CombatUI>();

        // If player has 10 True Souls, this fight is the dragon
        if (resources[ResourceType.TrueSoul] >= 10)
        {
            FightDragon();
            return;
        }

        int roll = hasRolled ? currentRoll : 10;

        float multiplier = GetRollMultiplier(roll);

        float playerPower =
            resources[ResourceType.CombatPower] * multiplier;

        float enemyPower = GetEnemyPower();

        Debug.Log("Roll: " + roll);
        Debug.Log("Player: " + playerPower + " Enemy: " + enemyPower);

        if (playerPower >= enemyPower)
        {
            resources[ResourceType.TrueSoul] += 1;
            enemyLevel++;

            if (ui != null)
                ui.ShowResult("WIN");
        }
        else
        {
            resources[ResourceType.Lives] -= 1;

            if (ui != null)
                ui.ShowResult("LOSE");

            if (resources[ResourceType.Lives] <= 0)
            {
                GameOver();
                return;
            }
        }

        // Reset roll after normal battle
        currentRoll = 10;
        hasRolled = false;

        UpdateInspectorValues();
        FireResourceEvent();

        if (ui != null)
        {
            ui.ShowRoll(10);
            ui.RefreshUI();
        }
    }

    public float GetRollMultiplier(int roll)
    {
        // 1 = x0.5
        // 10 = x1
        // 20 = x2

        if (roll == 10)
            return 1f;

        if (roll > 10)
            return 1f + ((roll - 10) / 10f);

        return 0.5f + ((roll - 1) / 18f) * 0.5f;
    }

    void FightDragon()
    {
        if (dragonAttempted) return;

        dragonAttempted = true;

        int roll = hasRolled ? currentRoll : 10;

        float player =
            resources[ResourceType.CombatPower] *
            GetRollMultiplier(roll);

        float dragon = 1000f;

        CombatUI ui = FindObjectOfType<CombatUI>();

        if (ui != null)
        {
            ui.ShowRoll(roll);
            ui.ShowResult("Kurosaki Appears!");
        }

        if (player >= dragon)
        {
            if (ui != null)
                ui.ShowResult("KUROSAKI DEFEATED!");

            ShowWinScreen();
            return;
        }

        if (ui != null)
            ui.ShowResult("KUROSAKI WINS");

        GameOver();
    }

    public int GetEnemyLevel()
    {
        return enemyLevel;
    }

    public float GetEnemyPower()
    {
        return Mathf.Pow(1.5f, enemyLevel) * 10;
    }

    // =========================
    // RESET
    // =========================
    void GameOver()
    {
        Debug.Log("GAME OVER");

        // Reset resources
        resources[ResourceType.Souls] = 0;
        resources[ResourceType.Gold] = 0;
        resources[ResourceType.CombatPower] = 10;
        resources[ResourceType.TrueSoul] = 0;
        resources[ResourceType.Lives] = 3;

        // Reset enemy progress
        enemyLevel = 1;
        dragonAttempted = false;

        // Reset generators
        generators.Clear();
        generators.Add(new GoldMine());

        // Reset upgrades
        for (int i = 0; i < upgrades.Count; i++)
        {
            upgrades[i].level = 0;
            upgrades[i].state = UpgradeState.Available;
        }

        UpdateInspectorValues();
        FireResourceEvent();
    }

    void ShowWinScreen()
    {
        if (winScreen != null)
        {
            winScreen.SetActive(true);
        }

        Time.timeScale = 0f;
    }
}