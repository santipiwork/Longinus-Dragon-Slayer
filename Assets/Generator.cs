using UnityEngine;

public abstract class Generator
{
    public string name;
    public ResourceType resourceType;
    public float productionRate;

    public Generator(string name, ResourceType type, float rate)
    {
        this.name = name;
        this.resourceType = type;
        this.productionRate = rate;
    }

    public abstract void Produce(ref float resourceAmount);
}