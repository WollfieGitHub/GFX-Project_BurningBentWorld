using System.Linq;
using Code.Scripts.TerrainGeneration.Components;
using Code.Scripts.TerrainGeneration.Generators.Layers.Optimization;
using TerrainGeneration;

namespace Code.Scripts.TerrainGeneration.Generators.Layers
{
    public class ShoreLayer : TransformLayer
    {
        public override CellMap Apply()
        {
            return (x, z, width, height) =>
            {
                var resultCells = MapAllocator.GetNew(width, height);

                // Increase map size to not be bothered with indices
                var parentWidth = width + 2;
                var parentHeight = height + 2;

                var cells = ParentMap(x, z, parentWidth, parentHeight);

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

        public ShoreLayer(long baseSeed) : base(baseSeed) { }
    }
}