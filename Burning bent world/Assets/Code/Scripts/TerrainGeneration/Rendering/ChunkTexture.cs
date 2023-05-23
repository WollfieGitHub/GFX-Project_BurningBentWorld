using Code.Scripts.TerrainGeneration.Components;
using TerrainGeneration;
using TerrainGeneration.Components;
using UnityEngine;
using Utils;
using static Utils.Utils;
using Terrain = TerrainGeneration.Components.Terrain;

namespace Code.Scripts.TerrainGeneration.Rendering
{
    public static class ChunkTexture
    {
        //TODO: Check if this is a good way of doing this
        public static Color burntColor = Color255(77, 29, 20);

        public enum DisplayType
        {
            Default, Temperature, Humidity, Height, Biome, Ocean
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
            const int width = Chunk.Size;
            const int height = Chunk.Size;

            return CreateTexture(width, height, (x, y) =>
            {
                var cell = chunk.GetCellAt(x, y);
                var cellInfo = cell.Info;

                return displayType switch
                {
                    DisplayType.Default => cellInfo.Ocean || cellInfo.Biome.IsRiver ? Color.blue 
                        : cell.burnt ? Color.Lerp(new Color(0.77f, 0.29f, 0.20f), cellInfo.Biome.Color, 0.2f)
                        : cellInfo.Biome.Color,
                    DisplayType.Temperature => GetTemperatureColor(cellInfo.Temperature),
                    DisplayType.Humidity => GetPrecipitationColor(cellInfo.Precipitation),
                    DisplayType.Height => GetHeightColor(cell.Height),
                    DisplayType.Ocean => GetOceanColor(cellInfo.Ocean),
                    DisplayType.Biome => GetBiomeColor(cellInfo),
                    _ => Color.magenta // Indicates a bug
                };
            });
            
        }

        private static Color GetBiomeColor(CellInfo cellInfo)
        {
            return Color.HSVToRGB(
                cellInfo.Biome.Id / (float)Biome.MaxId,
                0.8f, cellInfo.BiomeAttribute.IsHill ? 0.4f : 0.8f
            );
        }

        private static Color GetOceanColor(bool ocean)
        {
            return ocean ? Color.blue : Color.yellow;
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