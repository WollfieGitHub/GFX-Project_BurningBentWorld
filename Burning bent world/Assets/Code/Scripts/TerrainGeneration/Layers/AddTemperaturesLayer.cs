using TerrainGeneration.Components;
using UnityEngine;

namespace TerrainGeneration.Layers
{
    public class AddTemperaturesLayer : ITransformLayer
    {
        private const float Frequency = 0.005f * 64;
        
        public CellMap Apply(CellMap inputMap)
        {
            return (x, y, width, height) =>
            {
                var cells = inputMap(x, y, width, height);

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rY = 0; rY < height; rY++)
                    {
                        cells[rX,rY].Temperature = Mathf.Lerp(
                            Biome.MinTemperatureDeg, Biome.MaxTemperatureDeg,
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