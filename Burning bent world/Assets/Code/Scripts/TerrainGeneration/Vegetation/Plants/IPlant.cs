namespace Code.Scripts.TerrainGeneration.Vegetation.Plants
{
    public interface IPlant
    {
        float MinTemperature { get; }
        float MaxTemperature { get; }
        
        float MinHumidity { get; }
        float MaxHumidity { get; }
    }
}