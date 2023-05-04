using TerrainGeneration.Components;
using UnityEngine;

namespace TerrainGeneration.Layers
{
    public class AddBiomesLayer : ITransformLayer
    {
        public CellMap Apply(CellMap inputMap)
        {
            return (x, y, width, height) =>
            {
                var cells = inputMap(x, y, width, height);

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rY = 0; rY < height; rY++)
                    {
                        var cell = cells[rX, rY];

                        var (biome, intensity) = Biome.From(
                            cell.Temperature,
                            cell.Precipitation
                        );

                        cells[rX, rY].Biome = biome;
                        cells[rX, rY].BiomeIntensityFactor = intensity;
                    }
                }

                return cells;
            };
        }
    }
}