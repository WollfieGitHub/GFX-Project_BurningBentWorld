using Code.Scripts.TerrainGeneration.Components;
using Code.Scripts.TerrainGeneration.Generators.Layers.Smoothing;
using TerrainGeneration;
using Unity.VisualScripting;
using UnityEngine;
using static Utils.Utils;

namespace Code.Scripts.TerrainGeneration.Layers.Smoothing
{
    public class EdgeFallOff : IVoxelSmoother
    {
        private const int Distance = 16;

        public int NecessaryNeighboursCount => Distance; 

        private static float FallOffFunction(float x)
        {
            var sin = Mathf.Sin((x+2) / 2f * Mathf.PI);
            return sin * sin;
        }

        public float GetBiomeHeightAt(int globX, int globZ, int x, int z, int width, int height, CellInfo[,] cells)
        {
            var spiral = Spiral(Distance);

            var centerCell = cells[x, z];

            var minDistSqr = Distance * Distance;

            foreach (var (dx, dy) in spiral)
            {
                var distSqr = dx * dx + dy * dy;
                // Check if the x,y coordinates are within the grid and the cell
                // has different attributes than center cell
                if (distSqr < minDistSqr
                    && IsInBounds(x+dx, z+dy, width, height)
                    && !centerCell.HasSameParametersAs(cells[x+dx, z+dy]))
                {
                    minDistSqr = distSqr;
                }
            }

            var minDist = Mathf.Sqrt(minDistSqr);
            // Normalize distance from (0 to Distance) to (0 to 1)
            minDist /= Distance;

            // If the biome transition is further away than Distance, set Biome strength to 1,
            // Otherwise, transition between biome height and base height
            return Mathf.Lerp(
                0f, centerCell.BiomeAttribute.FBm(globX + x, globZ + z), 
                FallOffFunction(minDist)
            );
        }
    }
}