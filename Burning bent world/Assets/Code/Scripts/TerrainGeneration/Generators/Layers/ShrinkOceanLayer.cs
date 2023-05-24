using Code.Scripts.TerrainGeneration;
using Code.Scripts.TerrainGeneration.Generators.Layers.Optimization;
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

                // Increase by one maps' dimension to not be bothered by maps limit
                var parentWidth = width + 2;
                var parentHeight = height + 2;
                
                var parentCells = ParentMap(x, z, parentWidth, parentHeight);

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rZ = 0; rZ < height; rZ++)
                    {
                        InitChunkSeed(x + rX, z + rZ);
                        
                        var north = parentCells[rX + 1, rZ + 2];
                        var east = parentCells[rX + 2, rZ + 1];
                        var south = parentCells[rX + 1, rZ];
                        var west = parentCells[rX, rZ + 1];
                        
                        var center = parentCells[rX, rZ];

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