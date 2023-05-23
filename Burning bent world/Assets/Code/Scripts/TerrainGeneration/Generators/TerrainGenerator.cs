using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Code.Scripts.TerrainGeneration.Components;
using Code.Scripts.TerrainGeneration.Generators.Layers;
using Code.Scripts.TerrainGeneration.Generators.Layers.Mixers;
using Code.Scripts.TerrainGeneration.Generators.Layers.Zooms;
using Code.Scripts.TerrainGeneration.Layers;
using TerrainGeneration;
using TerrainGeneration.Layers;
using UnityEngine;
using Utils;
using Terrain = TerrainGeneration.Components.Terrain;

namespace Code.Scripts.TerrainGeneration.Generators
{
    public class TerrainGenerator : MonoBehaviour
    {

        private CellMap _map;
        
//======== ====== ==== ==
//      STACKS
//======== ====== ==== ==

        public void InitializeLayers(int worldSeed) {
            IslandLayer initIslandLayer = new(0.2f, 1);


            GenerationStack biomeStack = new("BiomeStack", new TransformLayer[]
            {
                new FuzzyZoom2Layer(2000), // 4096 -> 2048

                new AddIslandLayer(1), // Add first islands
                new Zoom2Layer(2001), // 2048 -> 1024

                new AddIslandLayer(2), // Add islands
                new AddIslandLayer(50), // Add islands
                new AddIslandLayer(70), // Add islands

                new ShrinkOceanLayer(2), // Shrink the amount of ocean using erosion and land growing
                new AddTemperaturesLayer(4), // Temperatures are smooth because generated from PerlinNoise

                new AddIslandLayer(25), // Add islands again

                new AddHumidityLayer(21), // Add humidity, partly ruled by already present temperature

                new AddBiomesLayer(64), // Add biomes, built from Humidity and Temperature

                new Zoom2Layer(47), // 1024 -> 512
                new Zoom2Layer(28), // 512 -> 256

                new AddIslandLayer(29), // Add islands

            }, initIslandLayer);

            GenerationStack preHillsStack = new("PreHills", new TransformLayer[]
            {
                new Zoom2Layer(38), // 256 -> 128
                new Zoom2Layer(934), // 128 -> 64
            }, biomeStack);

            GenerationStack riverInitStack = new("RiverInitStack", new TransformLayer[]
            {
                new RiverInitLayer(3), // Init river noise

                new Zoom2Layer(81), // 256 -> 128
                new Zoom2Layer(4), // 128 -> 64
            }, biomeStack);

            AddHillsLayer hillsLayer = new(781);
            hillsLayer = hillsLayer.Mix(preHillsStack, riverInitStack);

            GenerationStack detailsStack = new("DetailsStack", new TransformLayer[]
            {
                new Zoom2Layer(458), // 64 -> 32

                new AddIslandLayer(2457), // Add some more islands :)

                new Zoom2Layer(81), // 32 -> 16

                new ShoreLayer(976), // Set boundary cells with ocean as Shore biome 

                new Zoom2Layer(336), // 16 -> 8
                new Zoom2Layer(1346), // 8 -> 4

                new SmoothLayer(1348), // Final smooth
            }, hillsLayer);

            GenerationStack riverStack = new("RiverStack", new TransformLayer[]
            {
                new Zoom2Layer(891), // 64 -> 32
                new Zoom2Layer(562), // 32 -> 16
                new Zoom2Layer(21), // 16 -> 8
                new Zoom2Layer(8), // 8 -> 4

                new RiverLayer(185),
                new SmoothLayer(1345), // Final smooth
            }, riverInitStack);

            RiverMixerLayer riverMixerLayer = new(62);
            riverMixerLayer = riverMixerLayer.Mix(detailsStack, riverStack);
                

            GenerationStack heightStack = new("HeightStack", new TransformLayer[]
            {
                new AddHeightLayer(356),
                new SmoothLayer(64),
            }, riverMixerLayer);

            GenerationStack finalStack = new("FinalStack", new TransformLayer[]
            {
                new VoronoiZoomLayer(892), // 4 -> 1 Final zoom, voronoi
            }, heightStack);
            
            GenerationStack debugStack = new ("Debug Stack", new TransformLayer[]
            {
                // new Zoom2Layer(3),  // 4096 -> 2048
                // new Zoom2Layer(3),  // 2048 -> 1024
                // new Zoom2Layer(3),  // 1024 -> 512
                // new Zoom2Layer(3),  // 512 -> 256
                new Zoom2Layer(3),  // 256 -> 128
                new Zoom2Layer(3),  // 128 -> 64
                new Zoom2Layer(3),  // 64 -> 32
                new Zoom2Layer(3),  // 32 -> 16
                new Zoom2Layer(3),  // 16 -> 8
            }, biomeStack);

            Debug.Log($"Using [{worldSeed}] as World Seed");
            finalStack.InitWorldSeed(worldSeed);
            _map = finalStack.Apply();
        }
        
        
//======== ====== ==== ==
//      METHODS
//======== ====== ==== ==

        private void Awake()
        {
            lock (this) { InitializeLayers(Constants.Seed); }
        }

        /// <summary>
        /// Generate a new <see cref="Terrain"/> of specified dimensions
        /// </summary>
        /// <param name="xOffset">Offset on the X coordinate for the map position</param>
        /// <param name="zOffset">Offset on the Z coordinate for the map position</param>
        /// <param name="width">The width of the terrain to generate</param>
        /// <param name="height">The height of the terrain to generate</param>
        /// <returns>The newly generated terrain</returns>
        public CellInfo[,] GenerateNew(int xOffset, int zOffset, int width, int height)
        {
            // Cannot make threads interleave on the generation stack
            lock (this) { return _map(xOffset, zOffset, width, height); }
        }

    }

}