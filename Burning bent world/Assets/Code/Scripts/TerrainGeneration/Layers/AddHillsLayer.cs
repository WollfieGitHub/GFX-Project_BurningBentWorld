using System.Linq;
using Code.Scripts.TerrainGeneration.Layers.Optimization;
using TerrainGeneration;
using static Utils.Utils;

namespace Code.Scripts.TerrainGeneration.Layers
{
    public class AddHillsLayer : IMixLayer
    {
        public CellMap Mix(CellMap baseMap, CellMap riverMap)
        {
            return (x, y, width, height) =>
            {
                var resultCells = MapAllocator.GetNew(width, height);

                // Increase map size to not be bothered with indices
                var pX = x - 1;
                var pY = y - 1;
                var parentWidth = width + 2;
                var parentHeight = height + 2;
                
                var baseCells = baseMap(pX, pY, parentWidth, parentHeight);
                var riverCells = riverMap(x, y, width, height);

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rY = 0; rY < height; rY++)
                    {
                        var center = baseCells[rX + 1, rY + 1];
                        var centerRiverIndicator = riverCells[rX, rY].RiverIndicator;

                        var mutateToHill = centerRiverIndicator >= 2
                                           && (centerRiverIndicator - 2) % 29 == 0;
                        
                        if (center.Land 
                            && centerRiverIndicator >= 2
                            && (centerRiverIndicator - 2) % 29 == 1
                        ) {
                            center.BiomeAttribute.IsHill = true;
                            resultCells[rX, rY] = center;
                        }
                        else if (OneIn(3) && !mutateToHill)
                        {
                            // Simply copy center
                            resultCells[rX, rY] = center;
                        }
                        else
                        {
                            // Find neighbours
                            var north = baseCells[rX + 1, rY + 2];
                            var east = baseCells[rX + 2, rY + 1];
                            var south = baseCells[rX + 1, rY];
                            var west = baseCells[rX, rY + 1];
                            // Center  cell

                            var neighbours = new[] { north, east, south, west };

                            // If enough neighbours are of the same biome, then turn into hill
                            if (neighbours.Count(_ => _.Biome.Equals(center.Biome)) >= 3)
                            {
                                center.BiomeAttribute.IsHill = true;
                            }
                            
                            resultCells[rX, rY] = center;
                        }
                    }
                }

                return resultCells;
            };
        }
    }
}