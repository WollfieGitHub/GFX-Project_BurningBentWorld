using UnityEngine;

namespace TerrainGeneration.Noises
{
    public static class FractalBrownianMotion
    {
        /**
         * <summary>
         * Applies a Fractal Brownian Motion to the specified coordinates, taking
         * the given <see cref="octaveCount"/> to give variations on the perlin noise
         * </summary>
         * <param name="x">X coordinate</param>
         * <param name="y">Y coordinate</param>
         * <param name="octaveCount">Number of octaves</param>
         * <returns>The value of the fractal brownian motion</returns>
         * <remarks><a href="https://rtouti.github.io/graphics/perlin-noise-algorithm">Source</a></remarks>
         */
        public static float Apply(float x, float y, int octaveCount)
        {
            var result = 0.0f;
            var amplitude = 1.0f;
            var frequency = 0.005f;

            var persistence = 0.5f;
            var lacunarity = 2.0f;

            for (var octave = 0; octave < octaveCount; octave++) {
                var n = amplitude * Mathf.PerlinNoise(x * frequency, y * frequency);
                result += n;
		
                amplitude *= persistence;
                frequency *= lacunarity;
            }

            return result;
        } 
    }
    
}