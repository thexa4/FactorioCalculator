namespace FactorioCalculator.Models
{
    public class FluidBox
    {
        public bool IsOutput { get; protected set; }
        public Vector2 Position { get; protected set; }

        public FluidBox(bool isOutput, Vector2 position)
        {
            IsOutput = isOutput;
            Position = position;
        }
    }
}
