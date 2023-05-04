using System.Linq;
using TerrainGeneration;
using TerrainGeneration.Components;

namespace Code.Scripts.TerrainGeneration.Layers
{
    public class ShoreLayer : ITransformLayer
    {
        public CellMap Apply(CellMap inputMap)
        {
            return (x, y, width, height) =>
            {
                var resultCells = new CellInfo[width, height];

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
                        var north = cells[rX + 1, rY + 2];
                        var east = cells[rX + 2, rY + 1];
                        var south = cells[rX + 1, rY];
                        var west = cells[rX, rY + 1];
                        // Center  cell
                        var center = cells[rX + 1, rY + 1];
                        // Collect all neighbours
                        var neighbours = new []{ north, east, south, west };
                        
                        if (center.Land)
                        {
                            // TODO Adapt to special biomes
                            if (neighbours.Any(_ => _.Ocean))
                            {
                                center.Biome = Biome.Shore;
                            }
                        }
                        
                        resultCells[rX, rY] = center;
                    }
                }

                return resultCells;
            };
        }
    }
}