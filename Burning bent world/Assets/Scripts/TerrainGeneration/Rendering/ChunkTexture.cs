using TerrainGeneration.Components;
using UnityEngine;
using Utils;
using Terrain = TerrainGeneration.Components.Terrain;

namespace TerrainGeneration.Rendering
{
    public static class ChunkTexture
    {
        /// <summary>
        /// Creates a texture to visualize the height map of the chunk
        /// </summary>
        /// <param name="chunk">The chunk</param>
        /// <returns>The texture with height map</returns>
        public static Texture2D From(Chunk chunk)
        {
            // Compute the texture parameters
            var width = chunk.Width;
            var height = chunk.Height;
            
            // Create the color map
            var pixels = new Color[width * height];
            
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var cellHeight = chunk.GetHeightAt(x, y);
                    cellHeight = Mathf.InverseLerp(Terrain.MinHeight, Terrain.MaxHeight, cellHeight);
                    
                    pixels[y * width + x] = chunk.Biome.Color.Darken(cellHeight);
                }
            }
            
            var texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }
    }
}