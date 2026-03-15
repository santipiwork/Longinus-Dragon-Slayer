using System;
using UnityEngine;

[Serializable]
public class Upgrade
{
    public string name;
    public float baseCost;
    public int tier;

    public UpgradeState state;

    public ResourceType costResource;

    public UpgradeEffect effect;

    public int level;

    public Upgrade(string name, float cost, int tier, UpgradeEffect effect)
    {
        this.name = name;
        this.baseCost = cost;
        this.tier = tier;
        this.effect = effect;

        state = UpgradeState.Available;
        level = 0;
    }

    public float GetCost()
    {
        return baseCost * Mathf.Pow(1.5f, level);
    }
}