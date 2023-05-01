using System;
using TerrainGeneration.Noises;
using UnityEngine;
using Utils;
using static Utils.Utils;

namespace TerrainGeneration.Components
{
    /**
     * Source : http://www-cs-students.stanford.edu/~amitp/game-programming/polygon-map-generation/
     */
    public class Biome {

        public Biome(
            string name,
            Color color
        ) {
            Name = name;
            Color = color;
        }

        public readonly string Name;
        public readonly Color Color;

        public static readonly Biome Snow = new ("Snow", Color255(248, 248, 248));
        public static readonly Biome Tundra = new ("Tundra", Color255(221, 221, 187));
        public static readonly Biome Bare = new ("Bare", Color255(187, 187, 187));
        public static readonly Biome Scorched = new ("Scorched", Color255(153, 153, 153));
        public static readonly Biome Taiga = new ("Taiga", Color255(204, 212, 187));
        public static readonly Biome Shrubland = new ("Shrubland", Color255(196, 204, 187));
        public static readonly Biome TemperateDesert = new ("Temperate Desert", Color255(228, 232, 202));
        public static readonly Biome TemperateRainForest = new ("Temperate Rain Forest", Color255(164, 196, 168));
        public static readonly Biome TemperateDeciduousForest = new ("Temperate Deciduous Forest", Color255(180, 201, 169));
        public static readonly Biome Grassland = new ("Grassland", Color255(196, 212, 170));
        public static readonly Biome TropicalRainForest = new ("Tropical Rain Forest", Color255(156, 187, 169));
        public static readonly Biome TropicalSeasonalForest = new ("Tropical Seasonal Forest", Color255(169, 204, 164));
        public static readonly Biome SubtropicalDesert = new ("Subtropical Desert", Color255(233, 221, 199));

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
        public static Biome GetFrom(float elevationLevel, float moistureLevel)
        {
            elevationLevel = Mathf.Clamp(0, BiomeLookupTable.GetLength(0), elevationLevel);
            moistureLevel = Mathf.Clamp(0, BiomeLookupTable.GetLength(1), moistureLevel);
            // Return corresponding biome
            return BiomeLookupTable[Mathf.RoundToInt(elevationLevel), Mathf.RoundToInt(moistureLevel)];
        }

        /// <summary>
        /// Creates a map generator (height map generator) from a given elevation and moisture level
        /// This enables us to customize biome appearance to a certain degree 
        /// </summary>
        /// <param name="elevation">Level of elevation in the <see cref="Biome"/>. Ranges from 0 (low) to 3 (high)</param>
        /// <param name="moisture">Level of moisture in the <see cref="Biome"/>. Ranges from 0 (dry) to 5 (wet)</param>
        /// <returns>The map generator object</returns>
        public static GenerationMap<float> CreateHeightGenerator(
            float elevation,
            float moisture
        ) {
            /*
             * Frequency :
             *  - 0.001 -> Plains
             *  - 0.005 -> Mountains
             */
            var relativeElevation = Mathf.InverseLerp(MinElevation, MaxElevation, elevation);
            throw new NotImplementedException();
        }
        
    }
}