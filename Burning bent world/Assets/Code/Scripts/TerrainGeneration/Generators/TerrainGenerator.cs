using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Scripts.TerrainGeneration.Layers;
using TerrainGeneration.Components;
using TerrainGeneration.Layers;
using TerrainGeneration.Noises;
using Unity.VisualScripting;
using UnityEngine;
using static Utils.Utils;
using Terrain = TerrainGeneration.Components.Terrain;

namespace TerrainGeneration.Generators
{
    public class TerrainGenerator : MonoBehaviour
    {

        public event Action<ProgressStatus> OnProgressReported;

        private readonly Progress<ProgressStatus> _progress = new ();

        private void OnEnable() => _progress.ProgressChanged += OnProgress;
        private void OnDisable() => _progress.ProgressChanged -= OnProgress;

        private void OnProgress(object sender, ProgressStatus status) =>
            OnProgressReported?.Invoke(status);

//======== ====== ==== ==
//      STACKS
//======== ====== ==== ==
        
        private static readonly GenerationStack BiomeStack = new("BiomeStack", new ITransformLayer[]
        {
            new Zoom2Layer(),           // 4096 -> 2048
            
            new AddIslandLayer(),       // Add first islands
            new FuzzyZoom2Layer(),      // 2048 -> 1024
            
            new AddIslandLayer(),       // Add islands
            new AddIslandLayer(),       // Add islands
            new AddIslandLayer(),       // Add islands
            
            new ShrinkOceanLayer(),     // Shrink the amount of ocean using erosion and land growing
            new AddTemperaturesLayer(), // Temperatures are smooth because generated from PerlinNoise

            new AddIslandLayer(),       // Add islands again
            
            new AddHumidityLayer(),     // Add humidity, partly ruled by already present temperature
            
            new AddBiomesLayer(),       // Add biomes, built from Humidity and Temperature

            new Zoom2Layer(),           // 1024 -> 512
            new Zoom2Layer(),           // 512 -> 256
            
            new AddIslandLayer(),       // Add islands
        });


        private static readonly GenerationStack DetailsStack = new("DetailsStack", new ITransformLayer[]
        {
            new Zoom2Layer(),       // 256 -> 128
            new Zoom2Layer(),       // 128 -> 64
            new Zoom2Layer(),       // 64 -> 32
            
            new AddIslandLayer(),   // Add some more islands :)
            
            new Zoom2Layer(),       // 32 -> 16
            
            new ShoreLayer(),       // Set boundary cells with ocean as Shore biome 
            
            new Zoom2Layer(),       // 16 -> 8
            new Zoom2Layer(),       // 8 -> 4
            
            new SmoothLayer(),      // Final smooth
        });

        private static readonly GenerationStack RiverInitStack = new("RiverInitStack", new ITransformLayer[]
        {
            new RiverInitLayer(),   // Init river noise
            
            new Zoom2Layer(),       // 256 -> 128
            new Zoom2Layer(),       // 128 -> 64
        });

        private static readonly GenerationStack RiverStack = new("RiverStack", new ITransformLayer[]
        {
            new Zoom2Layer(),   // 64 -> 32
            new Zoom2Layer(),   // 32 -> 16
            new Zoom2Layer(),   // 16 -> 8
            new Zoom2Layer(),   // 8 -> 4
            
            new RiverLayer(),
            new SmoothLayer(),  // Final smooth
        });

        private static readonly GenerationStack FinalStack = new("FinalStack", new ITransformLayer[]
        {
            new VoronoiZoomLayer(), // Final zoom, voronoi
        });

        private static readonly GenerationStack HeightStack = new("HeightStack", new ITransformLayer[]
        {
            new AddHeightLayer(),
            new SmoothLayer(),
        });

//======== ====== ==== ==
//      MIXING
//======== ====== ==== ==

        private static readonly RiverMixerLayer RiverMixerLayer = new();
        
//======== ====== ==== ==
//      BASE LAYERS
//======== ====== ==== ==

        private static readonly CellMap BaseTerrainNoise = GenerationMaps.WhiteNoise(0.1f);

        private static readonly CellMap BaseHeightNoise 
            = GenerationMaps.FBmNoise(Terrain.MinHeight, Terrain.MaxHeight, 0.005f);
        
//======== ====== ==== ==
//      METHODS
//======== ====== ==== ==
        
        private CellMap GenerateMaps(IProgress<ProgressStatus> progress)
        {
            // Add islands, ocean, temperatures, precipitations and compute biome
            var biomesMap = BiomeStack.Apply(BaseTerrainNoise, progress);
            // Add details like shore, and smooth
            var detailsMap = DetailsStack.Apply(biomesMap, progress);

            // Init rivers 
            var riverInitMap = RiverInitStack.Apply(biomesMap, progress);
            // Complete rivers
            var completeRiverMap = RiverStack.Apply(riverInitMap, progress);
            // Mix river with main
            var mixedRiver = RiverMixerLayer.Mix(detailsMap, completeRiverMap);

            // Generate Height
            var heightMap = HeightStack.Apply(mixedRiver, progress);

            // Apply final transformations
            var finalMap = FinalStack.Apply(heightMap, progress);

            return finalMap;
        }

        public async Task<Terrain> GenerateNew(int width, int height)
        {
            var chunks = new Chunk[width / Chunk.Size, height / Chunk.Size];
            var map = await Task.Run(() => GenerateMaps(_progress));

            // var (mapWidth, mapHeight) = _biomeStack.DimensionModifier(width, height);

            var cellInfo = map(-width/2, -height/2, width, height);

            var nbCells = width * height / Chunk.Size;

            for (var xChunk = 0; xChunk < width / Chunk.Size; xChunk++)
            {
                for (var yChunk = 0; yChunk < height / Chunk.Size; yChunk++)
                {
                    // Chunks are lazily loaded to have decent performance
                    chunks[xChunk, yChunk] = new Chunk(
                        xChunk, yChunk, (x, y) => cellInfo[x, y]
                    );
                    // Report progress on build chunk instances
                    ((IProgress<ProgressStatus>)_progress).Report(new ProgressStatus
                    {
                        StackName = "ChunkBuilding",
                        Progress = (float)(yChunk * width + xChunk) / nbCells
                    });
                }
            }
            
            return new Terrain(chunks);
        }

        public struct ProgressStatus
        {
            public string StackName;
            public float Progress;
        }

    }

}