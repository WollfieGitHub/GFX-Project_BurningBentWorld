using System.Collections.Generic;
using TerrainGeneration.Components;
using UnityEngine;
using Utils;
using Terrain = TerrainGeneration.Components.Terrain;

namespace TerrainGeneration.Rendering
{
    public class ChunkMesh
    {
        /// <summary>
        /// Generates a mesh for a chunk
        /// </summary>
        /// <param name="chunk">The chunk to generate the 3D mesh from</param>
        /// <returns>The generated mesh</returns>
        public static Mesh From(Chunk chunk)
        {
            var mesh = new Mesh();

            var vertices = new Vector3[Chunk.Size, Chunk.Size, Constants.NbSidesInSquare];
            var triangles = new List<int>();

            // Simple algorithm :
            // - For sides : Depends on height difference with 4 neighbouring cells
            // - For top : 2 triangles at height level of cell
            for (var x = 0; x < Chunk.Size; x++)
            {
                for (var y = 0; y < Chunk.Size; y++)
                {
                    // In clockwise wind rose : NW, NE, SE, SW
                    vertices[x, y, 0] = new Vector3(x-0.5f, chunk.GetHeightAt(x, y), y+0.5f);
                    vertices[x, y, 1] = new Vector3(x+0.5f, chunk.GetHeightAt(x, y), y+0.5f);
                    vertices[x, y, 2] = new Vector3(x+0.5f, chunk.GetHeightAt(x, y), y-0.5f);
                    vertices[x, y, 3] = new Vector3(x-0.5f, chunk.GetHeightAt(x, y), y-0.5f);
                    
                    triangles.Add();
                }
            }
            
            
        } 
    }
}