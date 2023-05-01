using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using static Utils.Utils;

namespace TerrainGeneration
{
    public delegate T GenerationMap<out T>(float x, float y);

    public class WhiteNoise
    {
        private readonly float _fractionOfOnes;
        private readonly Dictionary<(int, int), int> _records;

        WhiteNoise(float fractionOfOnes)
        {
            _fractionOfOnes = fractionOfOnes;
            _records = new Dictionary<(int, int), int>();
        }

        /// <summary>
        /// Creates a white noise map with fraction of ones being the specified value
        /// </summary>
        /// <param name="fractionOfOnes">The fraction of ones compared to number of zeros in the
        /// distribution</param>
        /// <returns>The white noise map</returns>
        public static WhiteNoise CreateNew(float fractionOfOnes) => new (fractionOfOnes);

        public GenerationMap<int> Get()
        {
            return (x, y) =>
            {
                var coords = (Mathf.RoundToInt(x), Mathf.RoundToInt(y));

                if (_records.TryGetValue(coords, out var value)) { return value; }

                var newValue = CoinToss(1, 0, _fractionOfOnes);
                _records[coords] = newValue;
                return newValue;
            };
        }
    }
}