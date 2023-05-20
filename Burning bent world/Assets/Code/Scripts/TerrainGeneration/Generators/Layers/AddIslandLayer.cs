using System;
using System.Diagnostics;
using System.Linq;
using Code.Scripts.TerrainGeneration.Layers.Optimization;
using Code.Scripts.TerrainGeneration;
using UnityEngine;
using static Utils.Utils;
using Debug = UnityEngine.Debug;

namespace TerrainGeneration.Layers
{
    public class AddIslandLayer : TransformLayer
    {
        public override CellMap Apply()
        {
            return (x, z, width, height) =>
            {
                var resultCells = MapAllocator.GetNew(width, height);

                // Increase map size to not be bothered with indices
                var pX = x - 1;
                var pZ = z - 1;
                var parentWidth = width + 2;
                var parentHeight = height + 2;
                
                var cells = ParentMap(pX, pZ, parentWidth, parentHeight);

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rZ = 0; rZ < height; rZ++)
                    {
                        InitChunkSeed(x + rX, z + rZ);
                        // Find neighbours
                        var north = cells[rX + 1, rZ + 2];
                        var east = cells[rX + 2, rZ + 1];
                        var south = cells[rX + 1, rZ];
                        var west = cells[rX, rZ + 1];
                        // Center  cell
                        var center = cells[rX + 1, rZ + 1];
                        
                        var neighbours = new []{ north, east, south, west };
                        
                        if (center.Land || neighbours.All(_ => _.Ocean))
                        {
                            // If any neighbour is ocean and the center is land
                            if (center.Land && neighbours.Any(_ => _.Ocean))
                            {
                                // The 20% chance to get eroded
                                center.Land = CoinFlip(false, true, 0.2f);
                            }
                            
                            // Finally, copy the cell state into the result array
                            resultCells[rX, rZ] = center;
                        }
                        else
                        {
                            var oddsOfChange = 1;
                            var changedCell = center;

                            if (north.Land)
                            {
                                if (OneIn(oddsOfChange)) { changedCell = north; }
                                oddsOfChange++;
                            }

                            if (east.Land)
                            {
                                if (OneIn(oddsOfChange)) { changedCell = east; }
                                oddsOfChange++;
                            }

                            if (south.Land)
                            {
                                if (OneIn(oddsOfChange)) { changedCell = south; }
                                oddsOfChange++;
                            }

                            if (west.Land)
                            {
                                if (OneIn(oddsOfChange)) { changedCell = west; }
                            }

                            if (OneIn(3)) { resultCells[rX, rZ] = changedCell; }
                            else { resultCells[rX, rZ] = center; }
                        }

                    }
                }
                
                return resultCells;
            };
        }

        public AddIslandLayer(long baseSeed) : base(baseSeed) { }
    }
}