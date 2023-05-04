using TerrainGeneration.Components;
using UnityEngine;
using Utils;
using static Utils.Utils;
using Terrain = TerrainGeneration.Components.Terrain;

namespace TerrainGeneration.Rendering
{
    public static class ChunkTexture
    {
        public enum DisplayType
        {
            Default, Temperature, Humidity, Height, Biome
        }
        
        /// <summary>
        /// Creates a texture to visualize the height map of the chunk
        /// </summary>
        /// <param name="chunk">The chunk</param>
        /// <param name="displayType">The type of display needed for the texture</param>
        /// <returns>The texture with height map</returns>
        public static Texture2D From(Chunk chunk, DisplayType displayType)
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
                    var cell = chunk.GetCellAt(x, y);
                    var cellInfo = cell.Info;

                    var color = displayType switch
                    {
                        DisplayType.Default => cellInfo.Ocean || cellInfo.Biome.IsRiver ? Color.blue : cellInfo.Biome.Color,
                        DisplayType.Temperature => GetTemperatureColor(cellInfo.Temperature),
                        DisplayType.Humidity => GetPrecipitationColor(cellInfo.Precipitation),
                        DisplayType.Height => GetHeightColor(cell.Height),
                        DisplayType.Biome => GetBiomeColor(cellInfo),
                        _ => Color.magenta // Indicates a bug
                    };
                    
                    pixels[y * width + x] = color;
                }
            }
            
            var texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }

        private static Color GetBiomeColor(CellInfo cellInfo)
        {
            return Color.white.Mix(Color.black, cellInfo.BiomeIntensityFactor);
        }

        private static Color GetHeightColor(float cellHeight)
        {
            var relativeHeight = Mathf.InverseLerp(
                Terrain.MinHeight, Terrain.MaxHeight, cellHeight
            );
            return Color.black.Mix(Color.white, relativeHeight);
        }

        private static Color GetPrecipitationColor(float cellPrecipitation)
        {
            var relativeTemperature = Mathf.InverseLerp(
                Biome.MinPrecipitationCm, Biome.MaxPrecipitationCm, cellPrecipitation
            );
            return Color255(82, 72, 28).Mix(Color255(34, 255, 0), relativeTemperature);
        }

        private static Color GetTemperatureColor(float cellTemperature)
        {
            var relativeTemperature = Mathf.InverseLerp(
                Biome.MinTemperatureDeg, Biome.MaxTemperatureDeg, cellTemperature
            );
            return Color.red.Mix(Color.blue, relativeTemperature);
        }
    }
}