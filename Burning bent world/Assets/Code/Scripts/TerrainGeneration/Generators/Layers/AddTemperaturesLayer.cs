using Code.Scripts.TerrainGeneration;
using Code.Scripts.TerrainGeneration.Components;
using TerrainGeneration.Components;
using UnityEngine;
using Utils;

namespace TerrainGeneration.Layers
{
    public class AddTemperaturesLayer : TransformLayer
    {
        private const float Frequency = 0.005f * 64;
        
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
                        InitChunkSeed(x + rX, z + rZ);
                        
                        cells[rX,rZ].Temperature = Mathf.Lerp(
                            Biome.MinTemperatureDeg, Biome.MaxTemperatureDeg,
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

        public AddTemperaturesLayer(long baseSeed) : base(baseSeed) { }
    }
}