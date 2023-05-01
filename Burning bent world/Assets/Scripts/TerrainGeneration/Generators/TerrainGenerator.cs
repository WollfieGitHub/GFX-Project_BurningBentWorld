using System;
using System.Collections;
using System.Collections.Generic;
using TerrainGeneration.Components;
using TerrainGeneration.Noises;
using UnityEngine;
using static Utils.Utils;
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
                    // Determine elevation and moisture level (Use perlin noise to have well-behaved variation
                    // between close locations)
                    float ElevationMap(float x, float y) => ClampedPerlinAt(x, y, Biome.MinElevation, Biome.MaxElevation, 0.05f);
                    float MoistureMap(float x, float y) => ClampedPerlinAt(x, y, Biome.MinMoisture, Biome.MaxMoisture, 0.005f);
                    
                    // Chunks are lazily loaded to have decent performance
                    // chunks[xChunk, yChunk] = new Chunk(
                    //     xChunk, yChunk, Biome.CreateHeightGenerator(ElevationMap(xChunk, yChunk), MoistureMap(xChunk, yChunk))
                    // );
                }
            }

            return new Terrain(chunks);
        }
        
    }

}