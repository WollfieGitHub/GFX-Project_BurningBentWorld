using System;
using System.Collections.Generic;
using TerrainGeneration.Components;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Utils;
using static Utils.Utils;

namespace TerrainGeneration
{
    public delegate T GenerationMap<out T>(int x, int y, int width, int height);
    
    public delegate CellInfo[,] CellMap(int x, int y, int width, int height);

    public delegate CellInfo ChunkMap(int x, int y);

    public static class GenerationMaps
    {
        /// <summary>
        /// Creates a white noise map with fraction of ones being the specified value
        /// </summary>
        /// <param name="fractionOfLand">The fraction of ones compared to number of zeros in the
        /// distribution</param>
        /// <returns>The white noise map</returns>
        public static CellMap WhiteNoise(float fractionOfLand)
        {
            return (x, y, width, height) =>
            {
                var cells = new CellInfo[width, height];

                for (var px = 0; px < width; px++)
                {
                    for (var py = 0; py < height; py++)
                    {
                        cells[px, py] = new CellInfo();
                        cells[px, py].Land = CoinFlip(true, false, fractionOfLand);
                    }
                }
                
                return cells;
            };
        }
    }
}