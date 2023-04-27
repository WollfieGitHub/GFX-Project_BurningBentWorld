using System;
using System.Collections.Generic;
using System.Linq;
using TerrainGeneration.Components;
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

        // The number of cycles of the basic noise pattern that are repeated
        // over the width and height of the texture.
        public float scale = 1.0F;

        private Texture2D noiseTex;
        private Color[] pix;
        private Renderer rend;

        private Terrain Terrain;

        void Start()
        {
            rend = GetComponent<Renderer>();


            // Set up the texture and a Color array to hold pixels during processing.
            noiseTex = new Texture2D(pixWidth, pixHeight);
            pix = new Color[noiseTex.width * noiseTex.height];

            rend.material.mainTexture = noiseTex;
            
            Terrain = TerrainGenerator.GenerateNew(Mathf.RoundToInt(scale), Mathf.RoundToInt(scale));
            print(Terrain);
            
            CalcNoise();
            CalMesh();
        }

        private void CalMesh()
        {
            
        }

        void CalcNoise()
        {
            // For each pixel in the texture...

            if (ReferenceEquals(null, Terrain))
            {
                return;
            }

            var waterLevel = (Terrain.SeaLevel -(float)Terrain.MinHeight) / (Terrain.MaxHeight - Terrain.MinHeight);
            
            float y = 0.0F;
            while (y < noiseTex.height)
            {
                float x = 0.0F;
                while (x < noiseTex.width)
                {
                    float xCoord = xOrg + x / noiseTex.width * scale;
                    float yCoord = yOrg + y / noiseTex.height * scale;
                    Cell sample = Terrain.GetCellAt(
                        Mathf.RoundToInt(xCoord), 
                        Mathf.RoundToInt(yCoord)
                    );
                    var height = (sample.Height - Terrain.MinHeight) / (Terrain.MaxHeight - Terrain.MinHeight);
                    
                    
                    pix[(int)y * noiseTex.width + (int)x] = new Color(
                    height < waterLevel ? 0.7f : height, 
                    height < waterLevel ? 0.7f : height, 
                    height < waterLevel ? 1f : height
                    );
                    x++;
                }
                y++;
            }

            // Copy the pixel data to the texture and load it into the GPU.
            noiseTex.SetPixels(pix);
            noiseTex.Apply();
        }
    }
}