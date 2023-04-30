using System;
using TerrainGeneration.Components;
using TerrainGeneration.Noises;
using UnityEngine;
using Terrain = TerrainGeneration.Components.Terrain;

namespace TerrainGeneration.Generators
{
    public static class ChunkGenerator
    {
        /// <summary>
        /// Generates the chunk's content information based on the chunk's position relative to the terrain's origin
        /// </summary>
        /// <param name="xChunk">X coordinate relative to terrain's origin</param>
        /// <param name="yChunk">Y coordinate relative to terrain's origin</param>
        /// <returns>Tuple[Map of cell integer heights, moistureLevel]</returns>
        public static Tuple<int[,], int> Generate(int xChunk, int yChunk)
        {
            var cells = new int[Chunk.Size, Chunk.Size];

            for (var x = 0; x < Chunk.Size; x++)
            {
                for (var y = 0; y < Chunk.Size; y++)
                {
                    var cellHeight = FractalBrownianMotion.Apply(
                        xChunk * Chunk.Size + x,
                        yChunk * Chunk.Size + y,
                        octaveCount: 8
                    );
                    // Be sure we never reach max
                    cellHeight = Mathf.Clamp(cellHeight, 0f, .999999f);

                    cellHeight = cellHeight * (Terrain.MaxHeight - Terrain.MinHeight) + Terrain.MinHeight;

                    cells[x, y] = Mathf.FloorToInt(cellHeight);
                }
            }

            // Be sure we never reach max
            var moistureNoise = Mathf.Clamp(Mathf.PerlinNoise(xChunk, yChunk), 0, .999999f);
            var moistureLevel = Mathf.FloorToInt(
                moistureNoise * (Biome.MaxMoisture - Biome.MinMoisture) + Biome.MinMoisture
            );

            return new Tuple<int[,], int>(cells, moistureLevel);
        }
    }
}