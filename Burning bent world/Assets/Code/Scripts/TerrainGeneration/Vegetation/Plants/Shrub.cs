namespace Code.Scripts.TerrainGeneration.Vegetation.Plants
{
    public class Shrub : IPlant
    {
        public float MinTemperature { get; }
        public float MaxTemperature { get; }
        public float MinHumidity { get; }
        public float MaxHumidity { get; }
    }
}