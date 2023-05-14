using Code.Scripts.TerrainGeneration.Layers.Optimization;
using UnityEngine;
using static Utils.Utils;

namespace TerrainGeneration.Layers
{
    public class ShrinkOceanLayer : ITransformLayer
    {
        public CellMap Apply(CellMap inputMap)
        {
            return (x, y, width, height) =>
            {
                var resultCells = MapAllocator.GetNew(width, height);

                var pX = x - 1;
                var pY = y - 1;

                // Increase by one maps' dimension to not be bothered by maps limit
                var parentWidth = width + 2;
                var parentHeight = height + 2;
                
                var cells = inputMap(pX, pY, parentWidth, parentHeight);

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rY = 0; rY < height; rY++)
                    {
                        var north = cells[rX + 1, rY + 2];
                        var east = cells[rX + 2, rY + 1];
                        var south = cells[rX + 1, rY];
                        var west = cells[rX, rY + 1];
                        var center = cells[rX + 1, rY + 1];

                        if (!north.Land && !east.Land && !south.Land && !west.Land && !center.Land)
                        {
                            center.Land = CoinFlip(true, false);
                        }

                        resultCells[rX, rY] = center;
                    }
                }

                return resultCells;
            };
        }
    }
}