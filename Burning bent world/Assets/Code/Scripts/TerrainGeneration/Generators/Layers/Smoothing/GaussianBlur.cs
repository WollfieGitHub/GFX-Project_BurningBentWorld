using System.Collections.Generic;
using Code.Scripts.TerrainGeneration.Components;
using Code.Scripts.TerrainGeneration.Generators.Layers.Smoothing;
using TerrainGeneration;
using UnityEngine;

namespace Code.Scripts.TerrainGeneration.Layers.Smoothing
{
    public class GaussianBlur : IVoxelSmoother
    {
        private const int BlendRadius = 16;

        public int NecessaryNeighboursCount => BlendRadius;

        public float GetBiomeHeightAt(
            int globX, int globZ, int x, int z, int width, int height, CellInfo[,] cells
        ) {
            var cellHeight = 0f;
            
            // Loop over every point in the square containing the circle representing the blending radius.
            for (var ix = 0; ix < BlendRadius*2+1; ix++)
            {
                var idx = x + ix - BlendRadius;
                
                for (var iy = 0; iy < BlendRadius*2+1; iy++)
                {
                    var idy = z + iy - BlendRadius;

                    if (!(0 <= idx && idx < width && 0 <= idy && idy < height)) { continue; }
                    // Biome at this square within the blending circle
                    var cellAttribute = cells[idx, idy].BiomeAttribute;
                    
                    // Weight of the blur kernel over this point
                    var weight = BlurKernel[ix, iy];
                    if (weight <= 0) { continue; } // We can skip when it's zero

                    // Add weighted result of biome height
                    cellHeight += weight * cellAttribute.FBm(idx, idy);
                }
            }

            return cellHeight;
        }

        // Precompute the blending kernel
        // We use a blending kernel reminiscent of Gaussian blur, but with a continuous derivative at the truncation.
        private static readonly float[,] BlurKernel = new float[BlendRadius*2+1,BlendRadius*2+1];
        static GaussianBlur() {
            var weightTotal = 0.0f;

            var r2 = BlendRadius * BlendRadius;
            
            for (var ix = 0; ix < BlendRadius*2+1; ix++) {
                var idx = ix - BlendRadius;
                
                for (var iy = 0; iy < BlendRadius*2+1; iy++) {
                    var idy = iy - BlendRadius;
                    
                    float weight = (r2 - idy*idy - idx*idx);
                    if (weight <= 0) continue; // We only compute for the circle of positive values of the blending function.
                    weight *= weight; // Make transitions smoother.
                    
                    weightTotal += weight;
                    BlurKernel[ix, iy] = weight;
                }
            }
		
            // Rescale the weights, so they all add up to 1.
            for (var x = 0; x < BlurKernel.GetLength(0); x++) {
                for (var y = 0; y < BlurKernel.GetLength(1); y++) {
                    BlurKernel[x, y] /= weightTotal;
                }
            }
        }
    }
}