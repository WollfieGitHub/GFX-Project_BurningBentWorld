using System;
using System.Collections.Generic;
using Code.Scripts.TerrainGeneration.Components;
using Code.Scripts.TerrainGeneration.Generators.Layers.Smoothing;
using TerrainGeneration;
using Unity.Mathematics;
using UnityEngine;

namespace Code.Scripts.TerrainGeneration.Layers.Smoothing
{
    public class BilinearInterpolation : IVoxelSmoother
    {
        private const int InterpolationDistance = 1;
        
        public float GetBiomeHeightAt(
            int x, int y, int width, int height, CellInfo[,] cells
        )
        {
            var px = math.min(x + InterpolationDistance, width-1);
            var nx = math.max(x - InterpolationDistance, 0);

            var py = math.min(y + InterpolationDistance, height - 1);
            var ny = math.max(y - InterpolationDistance, 0);

            var ne = cells[px, py].BiomeAttribute.FBm(px, py);
            var no = cells[nx, py].BiomeAttribute.FBm(nx, py);

            var se = cells[px, ny].BiomeAttribute.FBm(px, ny);
            var so = cells[nx, ny].BiomeAttribute.FBm(nx, ny);

            var north = Mathf.Lerp(ne, no, px - nx);
            var south = Mathf.Lerp(se, so, px - nx);

            var center = Mathf.Lerp(north, south, py - ny);

            return center;
        }
    }
}