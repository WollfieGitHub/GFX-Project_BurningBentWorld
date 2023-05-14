namespace Code.Scripts.TerrainGeneration.Vegetation.Plants
{
    public class Herb : IPlant
    {
        public float MinTemperature => 9f;
        public float MaxTemperature => 32.2f;
        public float MinHumidity => 130f;
        public float MaxHumidity => 400f;
    }
}