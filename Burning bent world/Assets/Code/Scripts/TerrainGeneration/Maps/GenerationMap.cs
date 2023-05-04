using System;
using System.Collections.Generic;
using TerrainGeneration.Components;
using TerrainGeneration.Noises;
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

        /// <summary>
        /// Returns a map of cells with their <see cref="CellInfo.Height"/>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static CellMap FBmNoise(int minHeight, int maxHeight, float frequency)
        {
            var fbm = new FractalBrownianMotion(
                initialAmplitude: 1.0f,
                initialFrequency: frequency
            );

            return (x, y, width, height) =>
            {
                var cells = new CellInfo[width, height];

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rY = 0; rY < height; rY++)
                    {
                        var cell = new CellInfo();

                        cell.Height = Mathf.Lerp(
                            minHeight, maxHeight, fbm.Apply(rX, rY)
                        );
                        
                        cells[rX, rY] = cell;
                    }
                }

                return cells;
            };
        }
    }
}