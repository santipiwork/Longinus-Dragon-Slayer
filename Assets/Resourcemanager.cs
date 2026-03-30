using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResourceManager : MonoBehaviour
{
    public Dictionary<ResourceType, float> resources = new Dictionary<ResourceType, float>();

    public float souls;
    public float gold;
    public float combatPower;
    public float trueSouls;
    public float lives;

    public List<Generator> generators = new List<Generator>();
    public List<Upgrade> upgrades = new List<Upgrade>();

    float timer = 0f;

    void Start()
    {
        resources[ResourceType.Souls] = 0;
        resources[ResourceType.Gold] = 0;
        resources[ResourceType.CombatPower] = 10;
        resources[ResourceType.TrueSoul] = 0;
        resources[ResourceType.Lives] = 3;

        generators.Add(new GoldMine());

        Debug.Log("Game Started");
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            AddResource(ResourceType.Souls, 1);
        }

        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            foreach (var gen in generators)
            {
                float value = resources[gen.resourceType];
                gen.Produce(ref value);
                resources[gen.resourceType] = value;
            }

            timer = 0f;
        }

        souls = Mathf.FloorToInt(resources[ResourceType.Souls]);
        gold = Mathf.FloorToInt(resources[ResourceType.Gold]);
        combatPower = Mathf.FloorToInt(resources[ResourceType.CombatPower]);
        trueSouls = Mathf.FloorToInt(resources[ResourceType.TrueSoul]);
        lives = Mathf.FloorToInt(resources[ResourceType.Lives]);
    }

    public void AddResource(ResourceType type, float amount)
    {
        resources[type] += amount;
    }

    // =========================
    // SHOP
    // =========================
    public void BuyLife()
    {
        if (resources[ResourceType.Gold] >= 5000 && resources[ResourceType.Lives] < 3)
        {
            resources[ResourceType.Gold] -= 5000;
            resources[ResourceType.Lives] += 1;
        }
    }

    public void BuyCombatPower()
    {
        if (resources[ResourceType.Souls] >= 10)
        {
            resources[ResourceType.Souls] -= 10;
            resources[ResourceType.CombatPower] += 1;
        }
    }

    // =========================
    // UPGRADE SYSTEM (FIX)
    // =========================
    public bool BuyUpgrade(int index, out string message)
    {
        message = "";

        if (index >= upgrades.Count)
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

        // Apply effect (simple version)
        ApplyUpgradeEffect(u);

        u.level++;
        u.state = UpgradeState.Purchased;

        message = "Purchased " + u.name;
        return true;
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
    int enemyLevel = 1;

    public void StartFight()
    {
        int roll = Random.Range(1, 21); // player only

        float playerPower = resources[ResourceType.CombatPower] * roll;

        float enemyPower = Mathf.Pow(1.5f, enemyLevel) * 10;

        Debug.Log("Player: " + playerPower + " Enemy: " + enemyPower);

        if (playerPower >= enemyPower)
        {
            Debug.Log("WIN");

            resources[ResourceType.TrueSoul] += 1;

            enemyLevel++; // exponential scaling

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
    }

    int RollDice()
    {
        return Random.Range(1, 21); // 1–20
    }

    bool dragonAttempted = false;

    void FightDragon()
    {
        if (dragonAttempted) return;

        dragonAttempted = true;

        float player = resources[ResourceType.CombatPower] * Random.Range(1, 21);
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

    void GameOver()
    {
        Debug.Log("GAME OVER");

        resources[ResourceType.Souls] = 0;
        resources[ResourceType.Gold] = 0;
        resources[ResourceType.CombatPower] = 10;
        resources[ResourceType.TrueSoul] = 0;
        resources[ResourceType.Lives] = 3;

        generators.Clear();
        generators.Add(new GoldMine());
    }
}