using TerrainGeneration;
using TerrainGeneration.Components;
using UnityEngine;
using Utils;

namespace Code.Scripts.TerrainGeneration.Layers
{
    public class AddHumidityLayer : ITransformLayer
    {
        private const float Frequency = 0.010f * 64; // Precipitation varies more than temperature
        
        public CellMap Apply(CellMap inputMap)
        {
            return (x, y, width, height) =>
            {
                var xOffset = Constants.URandom.Next(1000);
                var yOffset = Constants.URandom.Next(1000);
                
                var cells = inputMap(x, y, width, height);
                
                for (var rX = 0; rX < width; rX++)
                {
                    for (var rY = 0; rY < height; rY++)
                    {
                        cells[rX,rY].Precipitation = Mathf.Lerp(
                            Biome.MinPrecipitationCm, Biome.MaxPrecipitationCm,
                            Mathf.Clamp01(Mathf.PerlinNoise(
                                Frequency * (rX + xOffset),
                                Frequency * (rY + yOffset)
                            ))
                        );
                    }
                }

                return cells;
            };
        }
    }
}