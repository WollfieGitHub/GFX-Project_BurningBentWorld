using System.Collections.Generic;
using TerrainGeneration;
using TerrainGeneration.Components;
using TerrainGeneration.Noises;
using UnityEngine;
using Terrain = TerrainGeneration.Components.Terrain;

namespace Code.Scripts.TerrainGeneration.Layers
{
    public class AddHeightLayer : ITransformLayer
    {
        private static readonly Dictionary<Biome, FractalBrownianMotion> BiomeToHeightMap = new();

        private static readonly FractalBrownianMotion BaseHeightMap = new();

        private static float TransitionFunction(float x)
        {
            var sin = Mathf.Sin(x/2.0f * Mathf.PI);
            return sin * sin;
        }

        public CellMap Apply(CellMap inputMap)
        {
            return (x, y, width, height) =>
            {
                var resultCells = new CellInfo[width, height];

                var parentCells = inputMap(x, y, width, height);

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rY = 0; rY < height; rY++)
                    {
                        var cell = parentCells[rX, rY];

                        var biomeHeight = cell.Biome.FBm.Apply(rX, rY);
                        var baseHeight = BaseHeightMap.Apply(rX, rY);

                        var mixedRelHeight = Mathf.Lerp(
                            Terrain.SeaLevel, biomeHeight, TransitionFunction(cell.BiomeIntensityFactor)
                        );

                        cell.Height = Mathf.Lerp(Terrain.SeaLevel, Terrain.MaxHeight, mixedRelHeight);

                        resultCells[rX, rY] = cell;
                    }
                }
                
                return resultCells;
            };
        }
    }
}