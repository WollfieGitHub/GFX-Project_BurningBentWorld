using Code.Scripts.TerrainGeneration.Layers.Optimization;
using Code.Scripts.TerrainGeneration;
using TerrainGeneration;
using static Utils.Utils;

namespace Code.Scripts.TerrainGeneration.Layers
{
    public class SmoothLayer : TransformLayer
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
                    for (var rZ = 0; rZ < height; rZ++)
                    {
                        
                        // Find neighbours
                        var north = cells[rX + 1, rZ + 2];
                        var east = cells[rX + 2, rZ + 1];
                        var south = cells[rX + 1, rZ];
                        var west = cells[rX, rZ + 1];
                        // Center  cell
                        var center = cells[rX + 1, rZ + 1];
                        
                        if (north.Equals(south) && west.Equals(east))
                        {
                            InitChunkSeed(x + rX, z + rZ);
                            center = CoinFlip(north, west, 0.5f);
                        }
                        else
                        {
                            if (north.Equals(south)) { center = north; }
                            if (west.Equals(east)) { center = west; }
                        }

                        resultCells[rX, rZ] = center;
                    }
                }

                return resultCells;
            };
        }

        public SmoothLayer(long baseSeed) : base(baseSeed) { }
    }
}