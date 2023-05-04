using TerrainGeneration;
using TerrainGeneration.Components;
using UnityEngine;

namespace Code.Scripts.TerrainGeneration.Layers
{
    public class AddHumidityLayer : ITransformLayer
    {
        private const float Frequency = 0.010f * 32; // Precipitation varies more than temperature
        
        public CellMap Apply(CellMap inputMap)
        {
            return (x, y, width, height) =>
            {
                var cells = inputMap(x, y, width, height);
                
                for (var rX = 0; rX < width; rX++)
                {
                    for (var rY = 0; rY < height; rY++)
                    {
                        cells[rX,rY].Precipitation = Mathf.Lerp(
                            Biome.MinPrecipitationCm, Biome.MaxPrecipitationCm,
                            Mathf.Clamp01(Mathf.PerlinNoise(
                                Frequency * rX,
                                Frequency * rY
                            ))
                        );
                    }
                }

                return cells;
            };
        }
    }
}