using System.Collections.Generic;
using TerrainGeneration.Noises;
using TerrainGeneration.Vegetation;
using UnityEngine;
using static Utils.Utils;

namespace Code.Scripts.TerrainGeneration.Components
{
    /**
     * Source : http://www-cs-students.stanford.edu/~amitp/game-programming/polygon-map-generation/
     */
    public class Biome
    {
        public static readonly HashSet<Biome> AllBiomes = new();

        public readonly int Id;
        public readonly string Name;
        public readonly Color Color;
        public readonly FractalBrownianMotion FBm;
        public readonly VegetationInfo Vegetation;

        public static int MaxId { private set; get; } = 0;

        private Biome(
            int id,
            string name,
            Color color,
            VegetationInfo vegetation,
            float fbmFrequency = 0.005f,
            // 0.1 is good amplitude for hills
            float fbmAmplitude = 0.02f
        ) {
            Id = id;
            if (id > MaxId) { MaxId = id; }
            Name = name;
            Color = color;
            FBm = new FractalBrownianMotion(
                initialAmplitude: fbmAmplitude,
                initialFrequency: fbmFrequency
            );
            Vegetation = vegetation;
            AllBiomes.Add(this);
        }
        
        public static readonly Biome Snow = new (0, "Snow", Color255(248, 248, 248), new (0, 0.05f, 0, 0));
        public static readonly Biome Tundra = new (1, "Tundra", Color255(221, 221, 187), new (0, 0.2f, 0.05f, 0.01f));
        public static readonly Biome Bare = new (2, "Bare", Color255(187, 187, 187), new (0f, 0f, 0f, 0f));
        public static readonly Biome Scorched = new (3, "Scorched", Color255(142, 98, 88), new (0.01f, 0f, 0.1f, 0f));
        
        public static readonly Biome Taiga = new (4, "Taiga", Color255(163, 163, 161), new (0.05f, 0.8f, 0.5f, 0.02f));
        public static readonly Biome Shrubland = new (5, "Shrubland", Color255(175, 153, 120), new (0f, 0f, 0.5f, 0.02f));
        public static readonly Biome TemperateDesert = new (6, "Temperate Desert", Color255(178, 117, 90), new (0f, 0f, 0.3f, 0f));
        
        public static readonly Biome TemperateRainForest = new (7, "Temperate Rain Forest", Color255(115, 170, 35), new (0f, 0.8f, 1.0f, 0.05f));
        public static readonly Biome TemperateDeciduousForest = new (8, "Temperate Deciduous Forest", Color255(180, 201, 169), new (0.8f, 0.1f, 1.0f, 0.05f));
        public static readonly Biome Grassland = new (9, "Grassland", Color255(196, 212, 170), new (0f, 0.005f, 1f, 0f));
        
        public static readonly Biome TropicalRainForest = new (10, "Tropical Rain Forest", Color255(156, 187, 169), new (1f, 0f, 1f, 0.5f));
        public static readonly Biome TropicalSeasonalForest = new (11, "Tropical Seasonal Forest", Color255(169, 204, 164), new (0.9f, 0f, 1.0f, 0.2f));
        public static readonly Biome SubtropicalDesert = new (12, "Subtropical Desert", Color255(233, 221, 199), new (0f, 0f, 0.05f, 0f));

        public static readonly Biome Shore = new (13, "Shore", Color255(255, 245, 150), new (0, 0, 0, 0), fbmFrequency: 0.005f, fbmAmplitude: 0.05f);
        public static readonly Biome River = new(14, "River", Color255(150, 255, 250), new (0, 0, 0, 0), fbmFrequency: 0.005f, fbmAmplitude: 0.05f);
        public static readonly Biome FrozenRiver = new (15, "FrozenRiver", Color255(217, 255, 253), new (0, 0, 0, 0), fbmFrequency: 0.005f, fbmAmplitude: 0.05f);
        
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
        
        /// <summary>
        /// Finds the biome that best corresponds to the given
        /// temperature and precipitation 
        /// </summary>
        /// <param name="temperature">The temperature (in degrees celsius)</param>
        /// <param name="precipitation">The precipitation (in centimeters)</param>
        /// <returns>The corresponding <see cref="Biome"/></returns>
        public static Biome From(float temperature, float precipitation)
        {
            temperature = Mathf.Clamp(temperature, MinTemperatureDeg, MaxTemperatureDeg);
            precipitation = Mathf.Clamp(precipitation, MinPrecipitationCm, MaxPrecipitationCm);

            var temperatureRegionsCount = BiomeMap.GetLength(0);
            var precipitationRegionsCount = BiomeMap.GetLength(1);

            var relTemperature = Mathf.InverseLerp(MinTemperatureDeg, MaxTemperatureDeg, temperature);
            var relPrecipitation = Mathf.InverseLerp(MinPrecipitationCm, MaxPrecipitationCm, precipitation);

            var temperatureIdx = relTemperature * (temperatureRegionsCount - 1);
            var precipitationIdx = relPrecipitation * (precipitationRegionsCount - 1);
            
            var temperatureRefIdx = Mathf.RoundToInt(temperatureIdx);
            var precipitationRefIdx = Mathf.RoundToInt(precipitationIdx);

            
            var resultBiome = BiomeMap[temperatureRefIdx, precipitationRefIdx];
            return resultBiome;
        }

        public static Biome FromId(int biomeId)
        {
            foreach (var biome in AllBiomes)
            {
                if (biome.Id == biomeId) { return biome; }
            }

            return null;
        }
    }
}