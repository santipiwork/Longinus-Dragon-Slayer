public class SoulWell : Generator
{
    public SoulWell(float rate = 1f) : base("Soul Well", ResourceType.Souls, rate) { }

    public override void Produce(ref float resourceAmount)
    {
        resourceAmount += productionRate;
    }
}