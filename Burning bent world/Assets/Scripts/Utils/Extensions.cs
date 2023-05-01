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
    }
}