using System;
using System.Collections.Generic;

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
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = Constants.URandom.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }  
        }
    }
}