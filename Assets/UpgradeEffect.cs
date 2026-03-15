using System;

[Serializable]
public struct UpgradeEffect
{
    public ResourceType targetResource;
    public float multiplier;

    public UpgradeEffect(ResourceType resource, float multi)
    {
        targetResource = resource;
        multiplier = multi;
    }
}