using System;
using System.Collections.Generic;
using TerrainGeneration.Components;
using Unity.Burst.Intrinsics;
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

        /// <summary>
        /// Finds whether the element with coordinates (x, y) is within the bounds of a
        /// 2 dimensional array with specified with and height
        /// </summary>
        /// <param name="x">The x coordinate of the element</param>
        /// <param name="y">The y coordinate of the element</param>
        /// <param name="width">The width of the array</param>
        /// <param name="height">The height of the array</param>
        /// <returns>True if the element is within the bounds of the array, false otherwise</returns>
        public static bool IsInBounds(int x, int y, int width, int height) =>
            0 <= x && x < width && 0 <= y && y < height;
        
        /// <summary>
        /// Gives an Enumerable of (x, y) coordinate tuple which iterates on a grid in a spiral fashion :
        /// From the center (0, 0), it returns the elements of the grid in a spiral
        /// until either the radius is reached
        /// </summary>
        /// <param name="radius">The radius of the spiral from the center</param>
        /// <returns>The (x, y) tuples of the grid in a spiral order</returns>
        /// <remarks><a href="https://stackoverflow.com/questions/398299/looping-in-a-spiral">Source</a></remarks>
        public static IEnumerable<(int, int)> Spiral(int radius) {
            int x = 0, y = 0, dx = 0, dy = -1;
            var maxI = (radius*radius*4);

            for (var i = 0; i < maxI; i++)
            {
                // Check if the x,y coordinates are still within the spiral
                if (-radius <= x && x <= radius && -radius <= y && y <= radius)
                {
                    yield return (x, y);
                }
                if( x == y || (x < 0 && x == -y) || (x > 0 && x == 1-y))
                {
                    // Swap dx and dy values
                    (dx, dy) = (-dy, dx);
                }
                x += dx;
                y += dy;
            }
        }

        /// <summary>
        /// Creates a texture easily with specified width and height
        /// </summary>
        /// <param name="width">Width of the texture</param>
        /// <param name="height">Height of the texture</param>
        /// <param name="getPixel">Return the pixel color at x,y</param>
        /// <returns>The newly created texture</returns>
        public static Texture2D CreateTexture(int width, int height, Func<int, int, Color> getPixel)
        {
            // Create the color map
            var pixels = new Color[width * height];
            
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    pixels[y * width + x] = getPixel(x, y);
                }
            }
            
            var texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }

        /// <summary>
        /// Computes the mathematical modulus : a (mod b)
        /// </summary>
        /// <param name="a">The value</param>
        /// <param name="b">The modulus' base</param>
        /// <returns>The result of a mod b</returns>
        public static int MathModulus(int a, int b) => (Math.Abs(a * b) + a) % b;
    }
}