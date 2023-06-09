﻿using Code.Scripts.TerrainGeneration.Components;
using UnityEngine;
using Utils;
using static UnityEngine.Mathf;
using static Utils.Utils;

namespace Code.Scripts.TerrainGeneration.Rendering
{
    public static class ChunkTexture
    {
        private static readonly Color WaterColor = Color255(164, 197, 235);
        private static readonly Color BurntColor = new (0.77f, 0.29f, 0.20f);
        private const float NoiseFrequency = 0.5f;
        private const float NoiseAmplitude = 0.25f;

        public const int Resolution = 2;

        public enum DisplayType
        {
            Default, Temperature, Humidity, Height, Biome, Ocean, Cell
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

            // Add 2 to be able to represent neighbours' color
            return CreateTexture(
                (width + 2)*Resolution, 
                (height + 2)*Resolution, 
                (x, y) =>
            {
                var dx = x/Resolution;
                var dy = y/Resolution;
                // If one of the neighbour
                if (dx == 0) { return Color255(0, 0, 0, 0); }
                if (dy == 0) { return Color255(0, 0, 0, 0); }
                if (dx == width + 1 && dy == height + 1) { return Color255(0, 0, 0, 0); }
                if (dx == width + 1 || dy == height + 1) { return Color255(0, 0, 0, 0); }

                dx -= 1;
                dy -= 1;
                
                var cell = chunk.GetCellAt(dx, dy);
                var cellInfo = cell.Info;

                var color = displayType switch
                {
                    DisplayType.Default => cellInfo.Ocean || cellInfo.Biome.IsRiver ? (
                            Color.black.Mix(WaterColor, InverseLerp(
                                GeneratedTerrain.SeaLevel, GeneratedTerrain.MinHeight / 4f, cell.Height
                            ))
                        )
                        : cell.Burnt ? Color.Lerp(BurntColor, cellInfo.Biome.Color, 0.2f)
                        : cellInfo.Biome.Color,
                    DisplayType.Temperature => GetTemperatureColor(cellInfo.Temperature),
                    DisplayType.Humidity => GetPrecipitationColor(cellInfo.Precipitation),
                    DisplayType.Height => GetHeightColor(cell.Height),
                    DisplayType.Ocean => GetOceanColor(cellInfo.Ocean),
                    DisplayType.Biome => GetBiomeColor(cellInfo),
                    DisplayType.Cell => (dx + dy) % 2 == 0 ? Color.black : Color.magenta,
                    _ => Color.magenta // Indicates a bug
                };

                var noise = Clamp01(PerlinNoise(
                    (chunk.ChunkX * Chunk.Size * Resolution + x) * NoiseFrequency,
                    (chunk.ChunkZ * Chunk.Size * Resolution + y) * NoiseFrequency
                )) * NoiseAmplitude - NoiseAmplitude/2.0f;

                Color.RGBToHSV(color, out var h, out var s, out var v);
                v *= 1 - noise;
                s *= 1 - noise * 0.5f;
                
                return Color.HSVToRGB(h, s, v);
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
            var relativeHeight = InverseLerp(
                GeneratedTerrain.MinHeight, GeneratedTerrain.MaxHeight, cellHeight
            );
            return Color.black.Mix(Color.white, relativeHeight);
        }

        private static Color GetPrecipitationColor(float cellPrecipitation)
        {
            var relativeTemperature = InverseLerp(
                Biome.MinPrecipitationCm, Biome.MaxPrecipitationCm, cellPrecipitation
            );
            return Color255(82, 72, 28).Mix(Color255(34, 255, 0), relativeTemperature);
        }

        private static Color GetTemperatureColor(float cellTemperature)
        {
            var relativeTemperature = InverseLerp(
                Biome.MinTemperatureDeg, Biome.MaxTemperatureDeg, cellTemperature
            );
            return Color.red.Mix(Color.blue, relativeTemperature);
        }
    }
}