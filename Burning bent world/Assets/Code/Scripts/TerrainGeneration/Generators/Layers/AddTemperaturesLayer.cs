﻿using Code.Scripts.TerrainGeneration.Components;
using TerrainGeneration;
using UnityEngine;

namespace Code.Scripts.TerrainGeneration.Generators.Layers
{
    public class AddTemperaturesLayer : TransformLayer
    {
        private int _xOffset;
        private int _zOffset;

        public override void InitWorldGenSeed(long seed)
        {
            base.InitWorldGenSeed(seed);
            _xOffset = _worldGenRandom.Next(1000);
            _zOffset = _worldGenRandom.Next(1000);
        }
        
        private const float Frequency = 0.005f * 64;
        
        public override CellMap Apply()
        {
            return (x, z, width, height) =>
            {

                var cells = ParentMap(x, z, width, height);

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rZ = 0; rZ < height; rZ++)
                    {
                        var cell = cells[rX, rZ];
                        
                        cell.Temperature = Mathf.Lerp(
                            Biome.MinTemperatureDeg, Biome.MaxTemperatureDeg,
                            Mathf.Clamp01(Mathf.PerlinNoise(
                                Frequency * (x + rX + _xOffset),
                                Frequency * (z + rZ + _zOffset)
                            ))
                        );

                        cells[rX, rZ] = cell;
                    }
                }
                return cells;
            };
        }

        public AddTemperaturesLayer(long baseSeed) : base(baseSeed) { }
    }
}