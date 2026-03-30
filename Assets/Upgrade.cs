using System;
using UnityEngine;

[Serializable]
public class Upgrade
{
    public string name;
    public float baseCost;
    public int tier;

    public UpgradeState state;

    public ResourceType costResource; // NEW

    public UpgradeEffect effect;

    public int level;

    // ✅ Updated constructor (5 parameters)
    public Upgrade(string name, float cost, int tier, ResourceType costResource, UpgradeEffect effect)
    {
        this.name = name;
        this.baseCost = cost;
        this.tier = tier;
        this.costResource = costResource;
        this.effect = effect;

        level = 0;
        state = UpgradeState.Available;
    }

    public float GetCost()
    {
        return baseCost * Mathf.Pow(1.5f, level);
    }
}