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

        private static readonly GenerationStack PreHillsStack = new("PreHills", new ITransformLayer[]
        {
            new Zoom2Layer(),       // 256 -> 128
            new Zoom2Layer(),       // 128 -> 64
        });


        private static readonly GenerationStack DetailsStack = new("DetailsStack", new ITransformLayer[]
        {
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

        private static readonly AddHillsLayer HillsLayer = new();
        
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

            var preHillsMap = PreHillsStack.Apply(biomesMap);

            // Init rivers 
            var riverInitMap = RiverInitStack.Apply(biomesMap, progress);

            // Add hills, using initial river tracing
            var hillsMap = HillsLayer.Mix(preHillsMap, riverInitMap);
            // Add details like shore, and smooth
            var detailsMap = DetailsStack.Apply(hillsMap, progress);
            
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

        /// <summary>
        /// Generate a new <see cref="Terrain"/> of specified dimensions
        /// </summary>
        /// <param name="xOffset">Offset on the X coordinate for the map position</param>
        /// <param name="zOffset">Offset on the Z coordinate for the map position</param>
        /// <param name="width">The width of the terrain to generate</param>
        /// <param name="height">The height of the terrain to generate</param>
        /// <returns>The newly generated terrain</returns>
        public Terrain GenerateNew(int xOffset, int zOffset, int width, int height)
        {
            var chunks = new Chunk[width / Chunk.Size, height / Chunk.Size];
            var map = GenerateMaps(_progress);

            // var (mapWidth, mapHeight) = _biomeStack.DimensionModifier(width, height);

            var cellInfo = map(xOffset, zOffset, width, height);

            var nbCells = width * height / Chunk.Size;

            for (var xChunk = 0; xChunk < width / Chunk.Size; xChunk++)
            {
                for (var zChunk = 0; zChunk < height / Chunk.Size; zChunk++)
                {
                    // Chunks are lazily loaded to have decent performance
                    chunks[xChunk, zChunk] = new Chunk(
                        xChunk, zChunk, (x, z) => cellInfo[x, z]
                    );
                    // Report progress on build chunk instances
                    ((IProgress<ProgressStatus>)_progress).Report(new ProgressStatus
                    {
                        StackName = "ChunkBuilding",
                        Progress = (float)(zChunk * width + xChunk) / nbCells
                    });
                }
            }
            // Report completion of terrain generation
            ((IProgress<ProgressStatus>)_progress).Report(new ProgressStatus
            {
                StackName = "Complete",
                Progress = 1f
            });
            
            return new Terrain(chunks);
        }

        public struct ProgressStatus
        {
            public string StackName;
            public float Progress;
        }

    }

}