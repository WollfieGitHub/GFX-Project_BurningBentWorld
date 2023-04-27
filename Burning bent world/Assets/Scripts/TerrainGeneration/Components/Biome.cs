using System;
using UnityEngine;
using Utils;

namespace TerrainGeneration.Components
{
    /**
     * Source : http://www-cs-students.stanford.edu/~amitp/game-programming/polygon-map-generation/
     */
    public class Biome {
        
        public Biome(string name) { Name = name; }

        public string Name { get; }

        public static readonly Biome Snow = new ("Snow");
        public static readonly Biome Tundra = new ("Tundra");
        public static readonly Biome Bare = new ("Bare");
        public static readonly Biome Scorched = new ("Scorched");
        public static readonly Biome Taiga = new ("Taiga");
        public static readonly Biome Shrubland = new ("Shrubland");
        public static readonly Biome TemperateDesert = new ("Temperate Desert");
        public static readonly Biome TemperateRainForest = new ("Temperate Rain Forest");
        public static readonly Biome TemperateDeciduousForest = new ("Temperate Deciduous Forest");
        public static readonly Biome Grassland = new ("Grassland");
        public static readonly Biome TropicalRainForest = new ("Tropical Rain Forest");
        public static readonly Biome TropicalSeasonalForest = new ("Tropical Seasonal Forest");
        public static readonly Biome SubtropicalDesert = new ("Subtropical Desert");

        private static readonly Biome[,] BiomeLookupTable =
        {
            {  TropicalRainForest,       TropicalRainForest,   TropicalSeasonalForest, TropicalSeasonalForest,       Grassland, SubtropicalDesert },
            { TemperateRainForest, TemperateDeciduousForest, TemperateDeciduousForest,              Grassland,       Grassland,   TemperateDesert },
            {               Taiga,                    Taiga,                Shrubland,        TemperateDesert, TemperateDesert,   TemperateDesert },
            {                Snow,                     Snow,                     Snow,                 Tundra,            Bare,          Scorched }
        };

        /** Max elevation a biome can have */
        public static int MaxElevation => BiomeLookupTable.GetLength(0);

        /** Min elevation a biome can have */
        public const int MinElevation = 0;
        
        /** Max elevation a biome can have */
        public static int MaxMoisture => BiomeLookupTable.GetLength(1);
        /** Min elevation a biome can have */
        public const int MinMoisture = 0;
        
        /**
         * <summary>Finds the biome corresponding to the specified elevation level and moisture level</summary>
         * <param name="moistureLevel">Level of moisture in the <see cref="Biome"/>. Ranges from 0 (dry) to 5 (wet)</param>
         * <param name="elevationLevel">Level of elevation in the <see cref="Biome"/>. Ranges from 0 (low) to 3 (high)</param>
         * <returns>The <see cref="Biome"/> corresponding the best to the specified criteria</returns>
         */
        public static Biome GetFrom(int elevationLevel, int moistureLevel)
        {
            Debug.Log($"{elevationLevel}, {moistureLevel}");
            Preconditions.CheckArgument(0<= elevationLevel && elevationLevel < BiomeLookupTable.GetLength(0));
            Preconditions.CheckArgument(0<= moistureLevel && moistureLevel < BiomeLookupTable.GetLength(1));
            // Return corresponding biome
            return BiomeLookupTable[elevationLevel, moistureLevel];
        }
        
    }
}