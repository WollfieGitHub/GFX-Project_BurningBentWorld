using System.Collections.Generic;
using TerrainGeneration.Noises;
using UnityEngine;
using static Utils.Utils;

namespace TerrainGeneration.Components
{
    /**
     * Source : http://www-cs-students.stanford.edu/~amitp/game-programming/polygon-map-generation/
     */
    public class Biome
    {

        public readonly int Id;
        public readonly string Name;
        public readonly Color Color;
        public readonly FractalBrownianMotion FBm;

        private Biome(
            int id,
            string name,
            Color color,
            float fbmFrequency = 0.005f,
            float fbmAmplitude = 0.2f
        ) {
            Id = id;
            Name = name;
            Color = color;
            FBm = new FractalBrownianMotion(
                initialAmplitude: fbmAmplitude,
                initialFrequency: fbmFrequency
            );
        }
        
        public static readonly Biome Snow = new (0, "Snow", Color255(248, 248, 248));
        public static readonly Biome Tundra = new (1, "Tundra", Color255(221, 221, 187));
        public static readonly Biome Bare = new (2, "Bare", Color255(187, 187, 187));
        public static readonly Biome Scorched = new (3, "Scorched", Color255(153, 153, 153));
        
        public static readonly Biome Taiga = new (4, "Taiga", Color255(204, 212, 187));
        public static readonly Biome Shrubland = new (5, "Shrubland", Color255(196, 204, 187));
        public static readonly Biome TemperateDesert = new (6, "Temperate Desert", Color255(228, 232, 202));
        
        public static readonly Biome TemperateRainForest = new (7, "Temperate Rain Forest", Color255(164, 196, 168));
        public static readonly Biome TemperateDeciduousForest = new (8, "Temperate Deciduous Forest", Color255(180, 201, 169));
        public static readonly Biome Grassland = new (9, "Grassland", Color255(196, 212, 170));
        
        public static readonly Biome TropicalRainForest = new (10, "Tropical Rain Forest", Color255(156, 187, 169));
        public static readonly Biome TropicalSeasonalForest = new (11, "Tropical Seasonal Forest", Color255(169, 204, 164));
        public static readonly Biome SubtropicalDesert = new (12, "Subtropical Desert", Color255(233, 221, 199));

        public static readonly Biome Shore = new (13, "Shore", Color255(255, 245, 150), fbmFrequency: 0.005f, fbmAmplitude: 0.05f);
        public static readonly Biome River = new(14, "River", Color255(150, 255, 250), fbmFrequency: 0.005f, fbmAmplitude: 0.05f);
        public static readonly Biome FrozenRiver = new (15, "FrozenRiver", Color255(217, 255, 253), fbmFrequency: 0.005f, fbmAmplitude: 0.05f);
        
        public bool IsRiver => River.Equals(this) || FrozenRiver.Equals(this);

        public override string ToString() => $"Biome[{Name}]";
        public override int GetHashCode() => Name.GetHashCode();
        public override bool Equals(object obj) => obj != null && obj is Biome that && that.Name.Equals(Name);

        public const int MinTemperatureDeg = -10;
        public const int MaxTemperatureDeg = 30;
        
        public const int WaterFreezingTemperature = 0;

        public const int MinPrecipitationCm = 0;
        public const int MaxPrecipitationCm = 400;

        private static readonly Biome[,] BiomeMap =
        {
            { Snow, Snow, Snow, Tundra, Bare, Scorched, },
            { Taiga, Taiga, Shrubland, Shrubland, TemperateDesert, TemperateDesert, },
            { TemperateRainForest, TemperateDeciduousForest, TemperateDeciduousForest, Grassland, Grassland, TemperateDesert, },
            { TropicalRainForest, TropicalRainForest, TropicalSeasonalForest, TropicalSeasonalForest, Grassland, SubtropicalDesert, },
        };
        
        public static readonly HashSet<Biome> RepresentedBiomes = new();
        
        /// <summary>
        /// Finds the biome that best corresponds to the given
        /// temperature and precipitation 
        /// </summary>
        /// <param name="temperature">The temperature (in degrees celsius)</param>
        /// <param name="precipitation">The precipitation (in centimeters)</param>
        /// <returns>The corresponding <see cref="Biome"/> and biome intensity factor
        /// The biome intensity factor is a number from 1 to 0 where 1 means the
        /// parameters correspond to the middle of the biome and 0 means
        /// the parameters correspond to a biome boundary</returns>
        public static (Biome, float) From(float temperature, float precipitation)
        {
            temperature = Mathf.Clamp(temperature, MinTemperatureDeg, MaxTemperatureDeg);
            precipitation = Mathf.Clamp(precipitation, MinPrecipitationCm, MaxPrecipitationCm);

            var temperatureRegionsCount = BiomeMap.GetLength(0);
            var precipitationRegionsCount = BiomeMap.GetLength(1);

            var relTemperature = Mathf.InverseLerp(MinTemperatureDeg, MaxTemperatureDeg, temperature);
            var relPrecipitation = Mathf.InverseLerp(MinPrecipitationCm, MaxPrecipitationCm, precipitation);

            var temperatureIdx = Mathf.RoundToInt(relTemperature * (temperatureRegionsCount - 1));
            var precipitationIdx = Mathf.RoundToInt(relPrecipitation * (precipitationRegionsCount - 1));

            var referenceRelTemperature = (float)temperatureIdx / (temperatureRegionsCount - 1);
            var referenceRelPrecipitation = (float)precipitationIdx / (precipitationRegionsCount - 1);

            var biomeIntensityFactor = Mathf.Min(
                Mathf.Abs(relTemperature - referenceRelTemperature),
                Mathf.Abs(relPrecipitation - referenceRelPrecipitation)
            );
            
            var result = BiomeMap[temperatureIdx, precipitationIdx];
            RepresentedBiomes.Add(result);
            return (result, biomeIntensityFactor);
        }

    }
}