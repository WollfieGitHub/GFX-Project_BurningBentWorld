using System;

namespace TerrainGeneration.Noises
{
    public class MapGenerator
    {
        private readonly FractalBrownianMotion _fractalBrownianMotion;
        private readonly RiverNoise _riverNoise;
        public readonly int MoistureLevel;

        public MapGenerator(FractalBrownianMotion fractalBrownianMotion, RiverNoise riverNoise, int moistureLevel)
        {
            _fractalBrownianMotion = fractalBrownianMotion;
            _riverNoise = riverNoise;
            MoistureLevel = moistureLevel;
        }

        /// <summary>
        /// Generates a value from a position in 2D space
        /// </summary>
        /// <param name="x">X Coordinate between 0 and 1</param>
        /// <param name="y">Y Coordinate between 0 and 1</param>
        /// <returns>A value computed from 2D space position</returns>
        public float ApplyHeight(float x, float y)
        {
            var fbm = _fractalBrownianMotion.Apply(x, y);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a value from a position in 2D space
        /// </summary>
        /// <param name="x">X Coordinate</param>
        /// <param name="y">Y Coordinate</param>
        /// <returns>A value computed from 2D space position</returns>
        public float ApplyWater(float x, float y) => _riverNoise.Apply(x, y);
    }
}