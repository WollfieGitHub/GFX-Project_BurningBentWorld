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
        /// Generates the chunk's height content information based on the chunk's position relative to the terrain's origin
        /// </summary>
        /// <param name="xChunk">X coordinate relative to terrain's origin</param>
        /// <param name="yChunk">Y coordinate relative to terrain's origin</param>
        /// <param name="mapGenerator">The object generating the height map given the chunk</param>
        /// <returns>Map of cell integer height</returns>
        public static int[,] GenerateHeight(int xChunk, int yChunk, MapGenerator mapGenerator)
        {
            var cells = new int[Chunk.Size, Chunk.Size];

            for (var x = 0; x < Chunk.Size; x++)
            {
                for (var y = 0; y < Chunk.Size; y++)
                {
                    var cellHeight = mapGenerator.ApplyHeight(
                        (float)xChunk * Chunk.Size + x,
                        (float)yChunk * Chunk.Size + y 
                    );
                    
                    // Be sure we never reach max
                    cellHeight = Mathf.Clamp(cellHeight, 0f, .999999f);

                    cellHeight = Mathf.Lerp(Terrain.MinHeight, Terrain.MaxHeight, cellHeight);

                    cells[x, y] = Mathf.FloorToInt(cellHeight);
                }
            }
            
            return cells;
        }

        /// <summary>
        /// Generates the chunk's water content information based on the chunk's position relative to the terrain's origin
        /// </summary>
        /// <param name="xChunk">X coordinate relative to terrain's origin</param>
        /// <param name="yChunk">Y coordinate relative to terrain's origin</param>
        /// <param name="mapGenerator">The object generating the water map given the chunk</param>
        /// <returns>Map of depth offsets</returns>
        public static float[,] GenerateWater(int xChunk, int yChunk, MapGenerator mapGenerator)
        {
            var cells = new float[Chunk.Size, Chunk.Size];

            for (var x = 0; x < Chunk.Size; x++)
            {
                for (var y = 0; y < Chunk.Size; y++)
                {
                    var cellOffset = mapGenerator.ApplyWater(
                        xChunk * Chunk.Size + x,
                        yChunk * Chunk.Size + y
                    );
                    // Be sure we never reach max
                    cellOffset = Mathf.Clamp(cellOffset, 0f, .999999f);
                    cellOffset = Mathf.Lerp(Terrain.MinWaterDepth, Terrain.MaxWaterDepth, cellOffset);
                    
                    cells[x, y] = cellOffset;
                }
            }

            return cells;
        }
    }
}