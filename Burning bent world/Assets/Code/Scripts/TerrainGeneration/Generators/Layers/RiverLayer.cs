using System.Linq;
using Code.Scripts.TerrainGeneration.Components;
using Code.Scripts.TerrainGeneration.Generators.Layers.Optimization;
using TerrainGeneration;

namespace Code.Scripts.TerrainGeneration.Generators.Layers
{
    public class RiverLayer : TransformLayer
    {
        private static int RiverFilter(int riverIndicator)
        {
            if (riverIndicator >= 2) { return 2 + (riverIndicator & 1); }
            return riverIndicator;
        }
        
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
                    for (var rZ = 0; rZ < height; rZ++)
                    {
                        // Find neighbours
                        var north = RiverFilter(cells[rX + 1, rZ + 2].RiverIndicator);
                        var east = RiverFilter(cells[rX + 2, rZ + 1].RiverIndicator);
                        var south = RiverFilter(cells[rX + 1, rZ].RiverIndicator);
                        var west = RiverFilter(cells[rX, rZ + 1].RiverIndicator);
                        // Center  cell
                        var centerCell = cells[rX + 1, rZ + 1];
                        var center = RiverFilter(centerCell.RiverIndicator);

                        // Collect all neighbours
                        var neighbours = new[] { north, east, south, west };
                        
                        // If any neighbour is different from the center, turn into river biome, otherwise
                        // leave as default
                        centerCell.Biome = neighbours.All(_ => _ == center) ? default : Biome.River;

                        resultCells[rX, rZ] = centerCell;
                    }
                }

                return resultCells;
            };
        }

        public RiverLayer(long baseSeed) : base(baseSeed) { }
    }
}