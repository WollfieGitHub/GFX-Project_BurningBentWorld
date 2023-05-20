using Code.Scripts.TerrainGeneration;
using Code.Scripts.TerrainGeneration.Components;
using TerrainGeneration;
using TerrainGeneration.Components;
using UnityEngine;
using Utils;

namespace Code.Scripts.TerrainGeneration.Layers
{
    public class AddHumidityLayer : TransformLayer
    {
        private const float Frequency = 0.010f * 64; // Precipitation varies more than temperature
        
        public override CellMap Apply()
        {
            return (x, z, width, height) =>
            {
                InitChunkSeed(x, z);
                var xOffset = NextInt(1000);
                var zOffset = NextInt(1000);
                
                var cells = ParentMap(x, z, width, height);
                
                for (var rX = 0; rX < width; rX++)
                {
                    for (var rZ = 0; rZ < height; rZ++)
                    {
                        cells[rX,rZ].Precipitation = Mathf.Lerp(
                            Biome.MinPrecipitationCm, Biome.MaxPrecipitationCm,
                            Mathf.Clamp01(Mathf.PerlinNoise(
                                Frequency * (rX + xOffset),
                                Frequency * (rZ + zOffset)
                            ))
                        );
                    }
                }

                return cells;
            };
        }

        public AddHumidityLayer(long baseSeed) : base(baseSeed) { }
    }
}