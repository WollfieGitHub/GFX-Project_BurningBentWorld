using System.Linq;
using Code.Scripts.TerrainGeneration.Layers.Optimization;
using TerrainGeneration;
using TerrainGeneration.Components;

namespace Code.Scripts.TerrainGeneration.Layers
{
    public class RiverLayer : ITransformLayer
    {
        private static int RiverFilter(int riverIndicator)
        {
            if (riverIndicator >= 2) { return 2 + (riverIndicator & 1); }
            return riverIndicator;
        }
        
        public CellMap Apply(CellMap inputMap)
        {
            return (x, y, width, height) =>
            {
                var resultCells = MapAllocator.GetNew(width, height);

                // Increase map size to not be bothered with indices
                var pX = x - 1;
                var pY = y - 1;
                var parentWidth = width + 2;
                var parentHeight = height + 2;
                
                var cells = inputMap(pX, pY, parentWidth, parentHeight);

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rY = 0; rY < height; rY++)
                    {
                        // Find neighbours
                        var north = RiverFilter(cells[rX + 1, rY + 2].RiverIndicator);
                        var east = RiverFilter(cells[rX + 2, rY + 1].RiverIndicator);
                        var south = RiverFilter(cells[rX + 1, rY].RiverIndicator);
                        var west = RiverFilter(cells[rX, rY + 1].RiverIndicator);
                        // Center  cell
                        var centerCell = cells[rX + 1, rY + 1];
                        var center = RiverFilter(centerCell.RiverIndicator);

                        // Collect all neighbours
                        var neighbours = new[] { north, east, south, west };
                        
                        // If any neighbour is different from the center, turn into river biome, otherwise
                        // leave as default
                        centerCell.Biome = neighbours.All(_ => _ == center) ? default : Biome.River;

                        resultCells[rX, rY] = centerCell;
                    }
                }

                return resultCells;
            };
        }
    }
}