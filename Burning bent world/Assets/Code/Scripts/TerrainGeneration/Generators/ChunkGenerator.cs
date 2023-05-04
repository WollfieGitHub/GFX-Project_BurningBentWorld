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
        public static Cell[,] GenerateHeight(int xChunk, int yChunk, ChunkMap mapGenerator)
        {
            var cells = new Cell[Chunk.Size, Chunk.Size];

            for (var x = 0; x < Chunk.Size; x++)
            {
                for (var y = 0; y < Chunk.Size; y++)
                {
                    var cellInfo = mapGenerator(
                        xChunk * Chunk.Size + x,
                        yChunk * Chunk.Size + y
                    );

                    // TODO
                    var cell = new Cell(cellInfo.Land ? 1f : 0f);
                    cell.Info = cellInfo;
                    cells[x, y] = cell;
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
        public static float[,] GenerateWater(int xChunk, int yChunk, GenerationMap<float> mapGenerator)
        {
            var cells = new float[Chunk.Size, Chunk.Size];

            for (var x = 0; x < Chunk.Size; x++)
            {
                for (var y = 0; y < Chunk.Size; y++)
                {
                    var cellOffset = mapGenerator(
                        xChunk * Chunk.Size + x,
                        yChunk * Chunk.Size + y,
                        Chunk.Size, Chunk.Size
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