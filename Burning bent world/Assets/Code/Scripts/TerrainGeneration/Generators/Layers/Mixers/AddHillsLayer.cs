using System;
using System.Linq;
using Code.Scripts.TerrainGeneration.Layers;
using Code.Scripts.TerrainGeneration.Layers.Optimization;
using Unity.VisualScripting;
using UnityEngine;

namespace Code.Scripts.TerrainGeneration.Generators.Layers.Mixers
{
    public class AddHillsLayer : MixLayer
    {
        public override CellMap Apply()
        {
            return (x, z, width, height) =>
            {
                var resultCells = MapAllocator.GetNew(width, height);

                // Increase map size to not be bothered with indices
                var parentWidth = width + 2;
                var parentHeight = height + 2;
                
                var baseCells = ParentMap(x, z, parentWidth, parentHeight);
                var riverCells = FilterMap(x, z, width, height);

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rZ = 0; rZ < height; rZ++)
                    {
                        InitChunkSeed(x + rX, z + rZ);
                        
                        var center = baseCells[rX + 1, rZ + 1];
                        var centerRiverIndicator = riverCells[rX, rZ].RiverIndicator;

                        var mutateToHill = centerRiverIndicator >= 2
                                           && (centerRiverIndicator - 2) % 29 == 0;
                        
                        if (center.Land 
                            && centerRiverIndicator >= 2
                            && (centerRiverIndicator - 2) % 29 == 1
                        ) {
                            center.BiomeAttribute.IsHill = true;
                            resultCells[rX, rZ] = center;
                        }
                        else if (OneIn(3) && !mutateToHill)
                        {
                            // Simply copy center
                            resultCells[rX, rZ] = center;
                        }
                        else
                        {
                            // Find neighbours
                            var north = baseCells[rX + 1, rZ + 2];
                            var east = baseCells[rX + 2, rZ + 1];
                            var south = baseCells[rX + 1, rZ];
                            var west = baseCells[rX, rZ + 1];
                            // Center  cell

                            var neighbours = new[] { north, east, south, west };

                            try
                            {
                                // If enough neighbours are of the same biome, then turn into hill
                                if (neighbours.Count(_ => _.Biome.Equals(center.Biome)) >= 3)
                                {
                                    center.BiomeAttribute.IsHill = true;
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.LogError(e);
                                Debug.LogError($"Center = {rX}, {rZ}");
                                Debug.LogError(center.Biome);
                                Debug.LogError(neighbours.Select(_ => _.Biome).ToCommaSeparatedString());
                                throw e;
                            }
                            
                            resultCells[rX, rZ] = center;
                        }
                    }
                }

                return resultCells;
            };
        }

        public AddHillsLayer(long baseSeed) : base(baseSeed) { }
    }
}