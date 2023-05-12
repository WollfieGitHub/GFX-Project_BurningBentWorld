namespace TerrainGeneration.Vegetation
{
    public class VegetationInfo
    {
        public float DeciduousTreeCoefficient;
        public float EvergreenTreeCoefficient;
        public float GrassCoefficient;
        public float ShrubsCoefficient;

        public VegetationInfo(
            float deciduousTreeCoefficient,
            float evergreenTreeCoefficient,
            float grassCoefficient,
            float shrubsCoefficient
        ) {
            DeciduousTreeCoefficient = deciduousTreeCoefficient;
            EvergreenTreeCoefficient = evergreenTreeCoefficient;
            GrassCoefficient = grassCoefficient;
            ShrubsCoefficient = shrubsCoefficient;
        }
        
    }
}