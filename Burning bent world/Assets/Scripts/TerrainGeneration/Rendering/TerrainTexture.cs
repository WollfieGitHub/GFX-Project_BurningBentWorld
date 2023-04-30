using UnityEngine;

namespace TerrainGeneration
{
    public static class TerrainTexture
    {
        /// <summary>
        /// Creates a texture to visualize the height map
        /// </summary>
        /// <param name="heightMap">The height map</param>
        /// <returns>The texture with height map</returns>
        public static Texture2D FromHeightMap(float[,] heightMap)
        {
            // Compute the texture parameters
            var width = heightMap.GetLength(0);
            var height = heightMap.GetLength(1);
            
            // Create the color map
            Color[] pixels = new Color[width * height];
            
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    pixels[y * width + x] = Color.Lerp(
                        Color.black, 
                        Color.white,
                        heightMap[x, y]
                    );
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