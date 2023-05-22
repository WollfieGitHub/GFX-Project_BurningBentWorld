using Code.Scripts.TerrainGeneration.Generators.Layers.Smoothing;
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
                var cells = ParentMap(x, z, width, height);

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rZ = 0; rZ < height; rZ++)
                    {
                        // TODO Instantiate FBM Here 
                        InitChunkSeed(x + rX, z + rZ);
                        var cell = cells[rX, rZ];
                        
                        var cellHeight = Smoother.GetBiomeHeightAt(rX, rZ, width, height, cells);

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
                
                Debug.Log("Height Layer Stack processed ");

                return cells;
            };
        }

        public AddHeightLayer(long baseSeed) : base(baseSeed) { }
    }
}