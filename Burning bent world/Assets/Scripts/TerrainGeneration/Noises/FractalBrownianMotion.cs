using UnityEngine;

namespace TerrainGeneration.Noises
{
    public class FractalBrownianMotion
    {
        private float _initialAmplitude;
        private float _initialFrequency;

        private float _gain;
        private float _lacunarity;

        private int _octaveCount;

        public FractalBrownianMotion(
            float initialAmplitude = 0.5f, 
            float initialFrequency = 0.005f, 
            float gain = 0.5f, 
            float lacunarity = 2.0f,
            int octaveCount = 8
        ) {
            _initialAmplitude = initialAmplitude;
            _initialFrequency = initialFrequency;
            _gain = gain;
            _lacunarity = lacunarity;
            _octaveCount = octaveCount;
        }

        /**
         * <summary>
         * Applies a Fractal Brownian Motion to the specified coordinates, taking
         * the given <see cref="_octaveCount"/> to give variations on the perlin noise
         * </summary>
         * <param name="x">X coordinate between 0 and 1</param>
         * <param name="y">Y coordinate between 0 and 1</param>
         * <returns>The value of the fractal brownian motion</returns>
         * <remarks><a href="https://rtouti.github.io/graphics/perlin-noise-algorithm">Source</a></remarks>
         */
        public float Apply(float x, float y)
        {
            var result = 0.0f;

            var frequency = _initialFrequency;
            var amplitude = _initialAmplitude;

            for (var octave = 0; octave < _octaveCount; octave++) {
                var n = amplitude * Perlin(x * frequency, y * frequency);
                result += n;
		
                amplitude *= _gain;
                frequency *= _lacunarity;
            }

            return result;
        }

        private static float Perlin(float x, float y) => Mathf.Clamp01(Mathf.PerlinNoise(x, y));
    }
    
}