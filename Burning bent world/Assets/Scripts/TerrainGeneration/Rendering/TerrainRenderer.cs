using System;
using System.Collections.Generic;
using System.Linq;
using TerrainGeneration.Components;
using TerrainGeneration.Generators;
using UnityEngine;
using Terrain = TerrainGeneration.Components.Terrain;

namespace TerrainGeneration
{
    [RequireComponent(typeof(Renderer))]
    public class TerrainRenderer : MonoBehaviour
    {
        // Width and height of the texture in pixels.
        public int pixWidth;
        public int pixHeight;

        // The origin of the sampled area in the plane.
        public float xOrg;
        public float yOrg;

        private Texture2D noiseTex;
        private Renderer rend;

        private Terrain Terrain;

        void Start()
        {
            rend = GetComponent<Renderer>();

            Terrain = TerrainGenerator.GenerateNew(
                pixWidth / Chunk.Size,
                pixHeight / Chunk.Size
            );

            // Set up the texture.
            noiseTex = CalcNoise(Terrain);
            rend.material.mainTexture = noiseTex;
            
            CalMesh();
        }

        private void CalMesh()
        {
            
        }

        Texture2D CalcNoise(Terrain terrain)
        {
            // For each pixel in the texture...
            if (ReferenceEquals(null, Terrain)) { return default; }

            var width = terrain.Width;
            var height = terrain.Height;
            
            var heightMap = new float[width, height];
            
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    Cell sample = Terrain.GetCellAt(
                        Mathf.RoundToInt(x), 
                        Mathf.RoundToInt(y)
                    );
                    
                    var cellHeight = (sample.Height - Terrain.MinHeight) / (Terrain.MaxHeight - Terrain.MinHeight);
                    
                    heightMap[x, y] = cellHeight;
                }
            }

            return TerrainTexture.FromHeightMap(heightMap);
        }
    }
}