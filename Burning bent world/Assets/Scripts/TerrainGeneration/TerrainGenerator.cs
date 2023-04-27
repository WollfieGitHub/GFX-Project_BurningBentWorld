using System.Collections;
using System.Collections.Generic;
using TerrainGeneration.Components;
using TerrainGeneration.Noises;
using UnityEngine;
using Terrain = TerrainGeneration.Components.Terrain;

namespace TerrainGeneration
{
    public static class TerrainGenerator
    {
        /**
         * <summary>Generate new <see cref="Terrain"/> from the specified width and height</summary>
         * <param name="width">Width of the <see cref="Terrain"/> to generate in Number of Chunks</param>
         * <param name="height">Height of the <see cref="Terrain"/> to generate in Number of Chunks</param>
         * <returns>The newly generated <see cref="Terrain"/></returns>
         */
        public static Terrain GenerateNew(int width, int height)
        {
            var chunks = new Chunk[width, height];
            
            for (var xChunk = 0; xChunk < width; xChunk++)
            {
                for (var yChunk = 0; yChunk < height; yChunk++)
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

                    chunks[xChunk, yChunk] = new Chunk(cells, moistureLevel);
                }
            }

            return new Terrain(chunks);
        }
    }

}