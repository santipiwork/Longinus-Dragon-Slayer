using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResourceManager : MonoBehaviour
{
    // Resource storage (assignment requirement)
    public Dictionary<string, float> resources = new Dictionary<string, float>();

    // Inspector values (to view resources while playing)
    public float souls;
    public float gold;

    // Passive income variables
    public float passiveSouls = 0;
    public float passiveGold = 0.5f;

    // Upgrade system (assignment requirement)
    public List<Upgrade> upgrades = new List<Upgrade>();

    float timer = 0f;

    void Start()
    {
        // Initialize resources
        resources["Souls"] = 0;
        resources["Gold"] = 0;

        // Create upgrades
        upgrades.Add(new Upgrade("Soul Echo", 10, 1));
        upgrades.Add(new Upgrade("Sword Spirit", 25, 2));

        Debug.Log("Resource Manager Started");
    }

    void Update()
    {
        // Manual click attack (press C)
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            AddResource("Souls", 1);
        }

        // Buy first upgrade (press U)
        if (Input.GetKeyDown(KeyCode.U))
        {
            BuyUpgrade(0);
        }

        // Passive income timer (1 second)
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            AddResource("Souls", passiveSouls);
            AddResource("Gold", passiveGold);
            timer = 0f;
        }

        // Sync values so they appear in Inspector
        souls = resources["Souls"];
        gold = resources["Gold"];
    }

    public void AddResource(string type, float amount)
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

        if (resources["Souls"] >= cost)
        {
            resources["Souls"] -= cost;

            passiveSouls += u.passiveGain;
            u.level++;

            Debug.Log("Purchased " + u.name + " Level " + u.level);
        }
        else
        {
            Debug.Log("Not enough Souls");
        }
    }
}

[System.Serializable]
public class Upgrade
{
    public string name;
    public float baseCost;
    public float passiveGain;
    public int level;

    public Upgrade(string n, float cost, float gain)
    {
        name = n;
        baseCost = cost;
        passiveGain = gain;
        level = 0;
    }

    public float GetCost()
    {
        return baseCost * Mathf.Pow(1.5f, level);
    }
}