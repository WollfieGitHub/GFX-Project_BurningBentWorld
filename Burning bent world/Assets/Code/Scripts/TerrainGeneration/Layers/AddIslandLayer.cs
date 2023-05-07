using System;
using System.Diagnostics;
using System.Linq;
using Code.Scripts.TerrainGeneration.Layers.Optimization;
using UnityEngine;
using static Utils.Utils;

namespace TerrainGeneration.Layers
{
    public class AddIslandLayer : ITransformLayer
    {
        public CellMap Apply(CellMap inputMap)
        {
            return (x, y, width, height) =>
            {
                var resultCells = MapAllocator.GetNew(width, height);

                // Increase map size to not be bothered with indices
                var pX = x - 1;
                var pY = y - 1;
                var parentWidth = width + 2;
                var parentHeight = height + 2;
                
                var cells = inputMap(pX, pY, parentWidth, parentHeight);

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
                        
                        var neighbours = new []{ north, east, south, west }.ToList();
                        
                        if (center.Land || neighbours.All(_ => _.Ocean))
                        {
                            // If any neighbour is ocean and the center is land
                            if (center.Land && neighbours.Any(_ => _.Ocean))
                            {
                                // The 20% chance to get eroded
                                center.Land = CoinFlip(false, true, 0.2f);
                            }
                            
                            // Finally, copy the cell state into the result array
                            resultCells[rX, rY] = center;
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

                            if (OneIn(3)) { resultCells[rX, rY] = changedCell; }
                            else { resultCells[rX, rY] = center; }
                        }

                    }
                }
                
                return resultCells;
            };
        }
    }
}