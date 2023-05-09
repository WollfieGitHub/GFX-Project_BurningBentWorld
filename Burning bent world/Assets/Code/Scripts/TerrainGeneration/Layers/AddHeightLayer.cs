using System.Collections.Generic;
using Code.Scripts.TerrainGeneration.Components;
using Code.Scripts.TerrainGeneration.Layers.Smoothing;
using TerrainGeneration;
using UnityEngine;
using Terrain = TerrainGeneration.Components.Terrain;

namespace Code.Scripts.TerrainGeneration.Layers
{
    public class AddHeightLayer : ITransformLayer
    {
        private static readonly EdgeFallOff Smoother = new EdgeFallOff();

        public CellMap Apply(CellMap inputMap)
        {
            return (x, y, width, height) =>
            {
                var cells = inputMap(x, y, width, height);

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rY = 0; rY < height; rY++)
                    {
                        var cell = cells[rX, rY];
                        
                        var cellHeight = Smoother.GetBiomeHeightAt(rX, rY, width, height, cells);

                        // TODO Problem occurs because the BiomeAttribute may have the same biome
                        // but different IsOcean value, which messes up everything

                        cell.Height = Mathf.RoundToInt(
                            cellHeight > 0
                                ? Mathf.Lerp(Terrain.SeaLevel, Terrain.MaxHeight, cellHeight)
                                : Mathf.Lerp(Terrain.SeaLevel, Terrain.MinHeight, -cellHeight)
                        );

                        cells[rX, rY] = cell;

                        // Debug.Log($"Cell {rX},{rY} / {width},{height}");
                    }
                }
                
                Smoother.Print();
                Debug.Log("Height Layer Stack processed ");

                return cells;
            };
        }

    }
}