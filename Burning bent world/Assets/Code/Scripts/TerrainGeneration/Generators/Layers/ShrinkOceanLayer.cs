using Code.Scripts.TerrainGeneration.Layers.Optimization;
using Code.Scripts.TerrainGeneration;
using UnityEngine;
using static Utils.Utils;

namespace TerrainGeneration.Layers
{
    public class ShrinkOceanLayer : TransformLayer
    {
        public override CellMap Apply()
        {
            return (x, z, width, height) =>
            {
                var resultCells = MapAllocator.GetNew(width, height);

                var pX = x - 1;
                var pZ = z - 1;

                // Increase by one maps' dimension to not be bothered by maps limit
                var parentWidth = width + 2;
                var parentHeight = height + 2;
                
                var cells = ParentMap(pX, pZ, parentWidth, parentHeight);

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rZ = 0; rZ < height; rZ++)
                    {
                        InitChunkSeed(x + rX, z + rZ);
                        
                        var north = cells[rX + 1, rZ + 2];
                        var east = cells[rX + 2, rZ + 1];
                        var south = cells[rX + 1, rZ];
                        var west = cells[rX, rZ + 1];
                        var center = cells[rX + 1, rZ + 1];

                        if (!north.Land && !east.Land && !south.Land && !west.Land && !center.Land)
                        {
                            center.Land = CoinFlip(true, false);
                        }

                        resultCells[rX, rZ] = center;
                    }
                }

                return resultCells;
            };
        }

        public ShrinkOceanLayer(long baseSeed) : base(baseSeed) { }
    }
}