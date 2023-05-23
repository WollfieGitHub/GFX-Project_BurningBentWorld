using Code.Scripts.TerrainGeneration.Components;
using Code.Scripts.TerrainGeneration.Generators.Layers.Smoothing;
using Code.Scripts.TerrainGeneration.Layers.Optimization;
using Code.Scripts.TerrainGeneration.Layers.Smoothing;
using TerrainGeneration;
using UnityEngine;
using Terrain = TerrainGeneration.Components.Terrain;

namespace Code.Scripts.TerrainGeneration.Generators.Layers
{
    public class AddHeightLayer : TransformLayer
    {
        private static readonly IVoxelSmoother Smoother = new EdgeFallOff();

        public override CellMap Apply()
        {
            return (x, z, width, height) =>
            {
                // Number of neighbours on which we need information on
                var dX = Smoother.NecessaryNeighboursCount;
                var dZ = Smoother.NecessaryNeighboursCount;
                
                var pWidth = width + 2 * dX;
                var pHeight = height + 2 * dZ;
                
                var cells = MapAllocator.GetNew(width, height);
                var parentCells = ParentMap(x, z, pWidth, pHeight);

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rZ = 0; rZ < height; rZ++)
                    {
                        // Account for parent array being bigger
                        var pX = rX + dX;
                        var pZ = rZ + dZ;

                        InitChunkSeed(x + rX, z + rZ);
                        var cell = parentCells[pX, pZ];
                        
                        var cellHeight = Smoother.GetBiomeHeightAt(
                            x, z,
                            pX, pZ, pWidth, pHeight, parentCells
                        );

                        // TODO Problem occurs because the BiomeAttribute may have the same biome
                        // but different IsOcean value, which messes up everything
                        
                        cell.Height = Mathf.RoundToInt(
                            cellHeight > 0
                                ? Mathf.Lerp(Terrain.SeaLevel, Terrain.MaxHeight, cellHeight)
                                : Mathf.Lerp(Terrain.SeaLevel-1.1f, Terrain.MinHeight, -cellHeight)
                        );

                        cells[rX, rZ] = cell;
                    }
                }
                
                return cells;
            };
        }

        public AddHeightLayer(long baseSeed) : base(baseSeed) { }
    }
}