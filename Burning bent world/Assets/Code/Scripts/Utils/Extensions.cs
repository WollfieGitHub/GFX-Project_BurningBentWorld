using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Extensions
    {
        /**
         * <summary>Shuffles a list randomly in place</summary>
         * <param name="list">Extended class, the list to shuffle</param>
         * <a href="https://stackoverflow.com/a/1262619">Source</a>
         */
        public static void Shuffle<T>(this IList<T> list)  
        {  
            var n = list.Count;  
            while (n > 1) {  
                n--;  
                var k = Constants.URandom.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }  
        }

        /// <summary>
        /// Darkens the color by the specified factor
        /// </summary>
        /// <param name="c">The color to darken</param>
        /// <param name="factor">The factor by which to darken the color</param>
        /// <returns>The darkened color</returns>
        public static Color Darken(this Color c, float factor) => new Color(c.r * factor, c.g * factor, c.b * factor, c.a);
        
        /// <summary>
        /// Lightens the color by the specified factor
        /// </summary>
        /// <param name="c">The color to lighten</param>
        /// <param name="factor">The factor by which to lighten the color</param>
        /// <returns>The lightened color</returns>
        public static Color Lighten(this Color c, float factor) => Darken(c, 1+factor);

        /// <summary>
        /// Performs a Linear mix in the RGBA space between this color
        /// and the <see cref="endColor"/> by the specified <see cref="factor"/>
        /// </summary>
        /// <param name="c">This color, starting color in the interpolation</param>
        /// <param name="endColor">The end color, ending color in the interpolation</param>
        /// <param name="factor">The interpolation factor between the two colors</param>
        /// <returns>The interpolated color</returns>
        public static Color Mix(this Color c, Color endColor, float factor)
        {
            return new Color(
                c.r * factor + endColor.r * (1 - factor),
                c.g * factor + endColor.g * (1 - factor),
                c.b * factor + endColor.b * (1 - factor),
                c.a * factor + endColor.a * (1 - factor)
            );
        }

        /// <summary>
        /// Finds the opposite direction
        /// </summary>
        /// <param name="direction">The initial direction</param>
        /// <returns>The opposite direction</returns>
        /// <exception cref="InvalidOperationException">If not a valid direction</exception>
        public static Utils.Direction Opposite(this Utils.Direction direction) => direction switch
        {
            Utils.Direction.North => Utils.Direction.South,
            Utils.Direction.South => Utils.Direction.North,
            Utils.Direction.East => Utils.Direction.West,
            Utils.Direction.West => Utils.Direction.East,
            _ => throw new InvalidOperationException()
        };


    }
}