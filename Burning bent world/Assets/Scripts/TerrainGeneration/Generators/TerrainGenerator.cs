using System;
using System.Collections;
using System.Collections.Generic;
using TerrainGeneration.Components;
using TerrainGeneration.Layers;
using TerrainGeneration.Noises;
using UnityEngine;
using static Utils.Utils;
using Terrain = TerrainGeneration.Components.Terrain;

namespace TerrainGeneration.Generators
{
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField][Range(-1, 10)] private int stopIndex = 0;

        private void OnValidate()
        {
            _biomeStack.StoppingLayerIdx = stopIndex;
        }

        private readonly DebugGenerationStack _biomeStack = new(new ITransformLayer[]
        {
            new Zoom2Layer(),           // 4096 -> 2048
            
            new AddIslandLayer(),
            new Zoom2Layer(),           // 2048 -> 1024
            
            new AddIslandLayer(),       //  
            new AddIslandLayer(),       // 
            new AddIslandLayer(),       // 
            
            new Zoom2Layer(),
            new Zoom2Layer(),
            new Zoom2Layer(),
            new Zoom2Layer(),
            new Zoom2Layer(),
            new Zoom2Layer(),
            new Zoom2Layer(),
            
            new ShrinkOceanLayer(),     //
            new AddTemperaturesLayer(), //
            
            new AddBiomesLayer(),       // Add biomes
            
            new Zoom2Layer(),           // 1024 -> 512
            new Zoom2Layer(),           // 512 -> 256
            
            new AddIslandLayer(),       // 
        });

        public GenerationMap<CellInfo> GenerateMaps()
        {
            var baseNoise = WhiteNoise.CreateNew(0.1f);
            var baseMap = baseNoise.Get();
            
            CellInfo InitialMap(float x, float y)
            {
                var value = baseMap(x, y);
                
                return new CellInfo { Land = value == 1 };
            }
            
            var biomesMap = _biomeStack.Apply(InitialMap);

            return biomesMap;
        }

        public Terrain GenerateNew(int width, int height)
        {
            var chunks = new Chunk[width, height];
            var map = GenerateMaps();
            
            for (var xChunk = 0; xChunk < width; xChunk++)
            {
                for (var yChunk = 0; yChunk < height; yChunk++)
                {
                    // Chunks are lazily loaded to have decent performance
                    chunks[xChunk, yChunk] = new Chunk(
                        xChunk, yChunk, map
                    );
                }
            }

            return new Terrain(chunks);
        }

    }

}