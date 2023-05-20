using System;
using Code.Scripts.TerrainGeneration.Components;
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

//======== ====== ==== ==
//      STACKS
//======== ====== ==== ==

        private static readonly IslandLayer InitIslandLayer = new (0.2f, 1875);

        private static readonly GenerationStack BiomeStack = new("BiomeStack", new TransformLayer[]
        {
            new FuzzyZoom2Layer(2000),           // 4096 -> 2048
            
            new AddIslandLayer(1),       // Add first islands
            new Zoom2Layer(2001),      // 2048 -> 1024
            
            new AddIslandLayer(2),       // Add islands
            new AddIslandLayer(50),       // Add islands
            new AddIslandLayer(70),       // Add islands
            
            new ShrinkOceanLayer(2),     // Shrink the amount of ocean using erosion and land growing
            new AddTemperaturesLayer(4), // Temperatures are smooth because generated from PerlinNoise

            new AddIslandLayer(25),       // Add islands again
            
            new AddHumidityLayer(21),     // Add humidity, partly ruled by already present temperature
            
            new AddBiomesLayer(64),       // Add biomes, built from Humidity and Temperature

            new Zoom2Layer(47),           // 1024 -> 512
            new Zoom2Layer(28),           // 512 -> 256
            
            new AddIslandLayer(29),       // Add islands
        }, InitIslandLayer);

        private static readonly GenerationStack PreHillsStack = new("PreHills", new TransformLayer[]
        {
            new Zoom2Layer(38),       // 256 -> 128
            new Zoom2Layer(934),       // 128 -> 64
        }, BiomeStack);

        private static readonly GenerationStack RiverInitStack = new("RiverInitStack", new TransformLayer[]
        {
            new RiverInitLayer(3),   // Init river noise
            
            new Zoom2Layer(81),       // 256 -> 128
            new Zoom2Layer(4),       // 128 -> 64
        }, BiomeStack);
        
        private static readonly AddHillsLayer HillsLayer = new AddHillsLayer(781)
            .Mix(PreHillsStack, RiverInitStack);

        private static readonly GenerationStack DetailsStack = new("DetailsStack", new TransformLayer[]
        {
            new Zoom2Layer(458),       // 64 -> 32
            
            new AddIslandLayer(2457),   // Add some more islands :)
            
            new Zoom2Layer(81),       // 32 -> 16
            
            new ShoreLayer(976),       // Set boundary cells with ocean as Shore biome 
            
            new Zoom2Layer(336),       // 16 -> 8
            new Zoom2Layer(1346),       // 8 -> 4
            
            new SmoothLayer(1348),      // Final smooth
        }, HillsLayer);

        private static readonly GenerationStack RiverStack = new("RiverStack", new TransformLayer[]
        {
            new Zoom2Layer(891),   // 64 -> 32
            new Zoom2Layer(562),   // 32 -> 16
            new Zoom2Layer(21),   // 16 -> 8
            new Zoom2Layer(8),   // 8 -> 4
            
            new RiverLayer(185),
            new SmoothLayer(1345),  // Final smooth
        }, RiverInitStack);

        private static readonly RiverMixerLayer RiverMixerLayer = new RiverMixerLayer(62)
            .Mix(DetailsStack, RiverStack);
        
        private static readonly GenerationStack HeightStack = new("HeightStack", new TransformLayer[]
        {
            new AddHeightLayer(356),
            new SmoothLayer(64),
        }, RiverMixerLayer);
        
        private static readonly GenerationStack FinalStack = new("FinalStack", new TransformLayer[]
        {
            new VoronoiZoomLayer(892), // 4 -> 1 Final zoom, voronoi
        }, HeightStack);
        
//======== ====== ==== ==
//      METHODS
//======== ====== ==== ==
        
        private CellMap GenerateMaps() => FinalStack.Apply(Constants.Seed);

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
            return GenerateMaps()(xOffset, zOffset, width, height);
        }

    }

}