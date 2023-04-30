using System.Collections;
using System.Collections.Generic;
using TerrainGeneration.Components;
using TerrainGeneration.Noises;
using UnityEngine;
using Terrain = TerrainGeneration.Components.Terrain;

namespace TerrainGeneration.Generators
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
                    // Chunks are lazily loaded to have decent performance
                    chunks[xChunk, yChunk] = new Chunk(xChunk, yChunk);
                }
            }

            return new Terrain(chunks);
        }
    }

}