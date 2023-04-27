using System.Collections.Generic;
using System.Linq;
using Utils;

namespace TerrainGeneration.Noises
{
    /**
     * Source : https://rtouti.github.io/graphics/perlin-noise-algorithm
     */
    public static class PerlinNoise
    {
        private const int PermutationsCount = 256;
        
        /**
         * <summary>Creates the permutations for the pelin noise</summary>
         */
        private static List<int> MakePermutation()
        {
            var permutation = new List<int>();
            
            for(var i = 0; i < PermutationsCount; i++)
            {
                permutation.Add(i);
            }

            permutation.Shuffle();
	
            for(var i = 0; i < PermutationsCount; i++) {
                permutation.Add(permutation[i]);
            }
	
            return permutation;
        }
        
        private static readonly List<int> Permutation = MakePermutation();
        
        /**
         * <param name="t">Parameter for which we evaluate the function</param>
         * <returns>An evaluation of a smooth curve at point T</returns>
         */
        private static float Fade(float t) =>  ((6 * t - 15) * t + 10) * t * t * t;
    }
}