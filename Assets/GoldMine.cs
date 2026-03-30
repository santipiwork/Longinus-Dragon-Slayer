public class GoldMine : Generator
{
    public GoldMine(float rate = 1f) : base("Gold Mine", ResourceType.Gold, rate) { }

    public override void Produce(ref float resourceAmount)
    {
        resourceAmount += productionRate;
    }
}