using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using static Utils.Utils;

namespace TerrainGeneration.Layers
{
    public class AddIslandLayer : ITransformLayer
    {
        public GenerationMap<CellInfo> Apply(GenerationMap<CellInfo> inputMap)
        {
            return (x, y) =>
            {
                // Find center cell
                var c = inputMap(x, y);

                // Find neighbours
                var n = inputMap(x, y + 1).Land;
                var e = inputMap(x + 1, y).Land;
                var s = inputMap(x, y - 1).Land;
                var w = inputMap(x - 1, y).Land;

                var neighbours = new[] { n, e, s, w };

                // If center is land
                if (c.Land)
                {
                    // Chances of being converted to land varies from 0.1 to 0.9 with number of land neighbours
                    c.Land = CoinToss(
                        true, false,
                        Mathf.Lerp(0.1f, 0.9f, neighbours.Count(_ => _) / 4.0f)
                    );
                }
                // If center is ocean
                else
                {
                    // Chances of being converted to ocean varies from 0.1 to 0.9 with number of ocean neighbours
                    c.Land = CoinToss(
                        false, true,
                        Mathf.Lerp(0.1f, 0.9f, neighbours.Count(isLand => !isLand) / 4.0f)
                    );
                }

                return c;
            };
        }
    }
}