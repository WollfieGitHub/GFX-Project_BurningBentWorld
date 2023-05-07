﻿using TerrainGeneration.Components;
using UnityEngine;

namespace Utils
{
    public static class Utils
    {
        /// <summary>
        /// Generates a perlin noise value, rounded to an int, which distribution
        /// is between min and max
        /// </summary>
        /// <param name="x">X Coordinate for evaluation of perlin noise</param>
        /// <param name="y">Y Coordinate for evaluation of perlin noise</param>
        /// <param name="min">Minimum int value</param>
        /// <param name="max">Maximum int value</param>
        /// <param name="frequency">The frequency for the perlin noise sampling</param>
        /// <returns>The value between min and max evaluated form perlin noise</returns>
        public static float ClampedPerlinAt(float x, float y, int min, int max, float frequency)
        {
            // Be sure we never reach max
            var noiseValue = Mathf.Clamp(Mathf.PerlinNoise(x * frequency, y * frequency), 0, .999999f);
            var level = Mathf.Lerp(min, max, noiseValue);
            return level;
        }

        /// <summary>
        /// Creates a color from rgba values which range from 0 to 255 (Used for readability)
        /// </summary>
        /// <param name="r">Red channel</param>
        /// <param name="g">Green channel</param>
        /// <param name="b">Blue channel</param>
        /// <param name="a">Alpha channel</param>
        /// <returns>The newly created color</returns>
        public static Color Color255(int r, int g, int b, int a) => new(r / 255f, g / 255f, b / 255f, a / 255f);
        
        /// <summary>
        /// Creates a color from rgba values which range from 0 to 255 (Used for readability)
        /// </summary>
        /// <param name="r">Red channel</param>
        /// <param name="g">Green channel</param>
        /// <param name="b">Blue channel</param>
        /// <returns>The newly created color</returns>
        public static Color Color255(int r, int g, int b) => Color255(r, g, b,255);

        /// <summary>
        /// Draws a random result form the specified odds
        /// </summary>
        /// <param name="head">Object returned in case of Head</param>
        /// <param name="tail">Object returned in case of Tail</param>
        /// <param name="headOdds">Odds of drawing Head</param>
        /// <typeparam name="T">Type of the object to return</typeparam>
        /// <returns>The result of the random event</returns>
        public static T CoinFlip<T>(T head, T tail, float headOdds) =>
            Constants.URandom.NextDouble() < headOdds ? head : tail;

        /// <summary>
        /// Draws a random result from a uniform distribution (50/50)
        /// </summary>
        /// <param name="head">Object returned in case of Head</param>
        /// <param name="tail">Object returned in case of Tail</param>
        /// <typeparam name="T">Type of the object to return</typeparam>
        /// <returns>The result of the random event</returns>
        public static T CoinFlip<T>(T head, T tail) => CoinFlip(head, tail, 0.5f);

        /// <summary>
        /// Returns true with a change of one in <see cref="odds"/>
        /// </summary>
        /// <param name="odds">The inverse of the odds of returning true</param>
        /// <returns>True if random drawn true with one in <see cref="odds"/> chances, false otherwise</returns>
        public static bool OneIn(int odds) => Constants.URandom.Next(odds) == 0;
    }
}