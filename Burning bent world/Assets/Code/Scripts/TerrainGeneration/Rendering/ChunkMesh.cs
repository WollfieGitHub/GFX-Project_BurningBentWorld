using System;
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

            var meshData = new MeshData(chunk.Width, chunk.Height);

            // Simple algorithm :
            // - For sides : Depends on height difference with 4 neighbouring cells
            // - For top : 2 triangles at height level of cell
            for (var x = 0; x < chunk.Width; x++)
            {
                for (var y = 0; y < chunk.Height; y++)
                {
                    var height = chunk.GetHeightAt(x, y);
                    
                    // In clockwise wind rose : NW, NE, SE, SW
                    var vTop1 = meshData.AddVertex(x, y, height, VertexDir.Nw);
                    var vTop2 = meshData.AddVertex(x, y, height, VertexDir.Ne);
                    var vTop3 = meshData.AddVertex(x, y, height, VertexDir.Se);
                    var vTop4 = meshData.AddVertex(x, y, height, VertexDir.Sw);
                    // Add top surface such that it faces upward
                    meshData.AddTriangle(vTop1, vTop2, vTop3);
                    meshData.AddTriangle(vTop1, vTop3, vTop4);

                    int vBot1, vBot2, vBot3, vBot4;
                    
                    //======== ====== ==== ==
                    //      NORTH VERTICAL PANE
                    //======== ====== ==== ==

                    if (y == chunk.Height-1)
                    {
                        vBot1 = meshData.AddBoundaryVertex(x, y+1, VertexDir.Sw);
                        vBot2 = meshData.AddBoundaryVertex(x, y+1, VertexDir.Se);
                    
                        meshData.AddTriangle(vTop1, vTop2, vBot2);
                        meshData.AddTriangle(vTop1, vBot2, vBot1);
                    }

                    //======== ====== ==== ==
                    //      SOUTH VERTICAL PANE
                    //======== ====== ==== ==
                    
                    if (y != 0) {
                        vBot3 = meshData.GetVertex(x, y - 1, VertexDir.Ne).Index;
                        vBot4 = meshData.GetVertex(x, y - 1, VertexDir.Nw).Index;
                    }
                    else
                    {
                        vBot3 = meshData.AddBoundaryVertex(x, y - 1, VertexDir.Se);
                        vBot4 = meshData.AddBoundaryVertex(x, y - 1, VertexDir.Sw);
                    }
                    meshData.AddTriangle(vTop4, vTop3, vBot3);
                    meshData.AddTriangle(vTop4, vBot3, vBot4);
                    
                    
                    //======== ====== ==== ==
                    //      WEST VERTICAL PANE
                    //======== ====== ==== ==
                    
                    if (x != 0) {
                        vBot1 = meshData.GetVertex(x - 1, y, VertexDir.Ne).Index;
                        vBot4 = meshData.GetVertex(x - 1, y, VertexDir.Se).Index;
                    }
                    else
                    {
                        vBot1 = meshData.AddBoundaryVertex(x - 1, y, VertexDir.Ne);
                        vBot4 = meshData.AddBoundaryVertex(x - 1, y, VertexDir.Se);
                    }
                    meshData.AddTriangle(vTop1, vTop4, vBot4);
                    meshData.AddTriangle(vTop1, vBot4, vBot1);

                    //======== ====== ==== ==
                    //      EAST VERTICAL PANE
                    //======== ====== ==== ==

                    if (x == chunk.Width-1)
                    {
                        vBot2 = meshData.AddBoundaryVertex(x + 1, y, VertexDir.Nw);
                        vBot3 = meshData.AddBoundaryVertex(x + 1, y, VertexDir.Sw);
                    
                        meshData.AddTriangle(vTop3, vTop2, vBot2);
                        meshData.AddTriangle(vTop3, vBot2, vBot3);
                    }

                }
            }

            mesh.vertices = meshData.Vertices;
            mesh.triangles = meshData.Triangles;
            mesh.uv = meshData.UV;

            mesh.Optimize();
            mesh.RecalculateNormals();
            
            return mesh;
        }

        private class MeshData
        {
            private readonly int _width;
            private readonly int _height;

            private readonly List<int> _triangles;

            public int[] Triangles => _triangles.ToArray(); 

            private readonly Vertex[,,] _vertices;
            private readonly List<Vertex> _boundaryVertices = new ();

            public Vector3[] Vertices 
            {
                get { 
                    // Add boundary vertices
                    var result = new Vector3[_width * _height * Constants.NbSidesInSquare + _boundaryVertices.Count]; 
                    foreach(var v in _vertices) { result[v.Index] = v.Position; }
                    foreach (var v in _boundaryVertices) { result[v.Index] = v.Position; }

                    return result; 
                } 
            }
            
            public Vector2[] UV 
            {
                get { 
                    var result = new Vector2[_width * _height * Constants.NbSidesInSquare + _boundaryVertices.Count];
                    
                    foreach(var v in _vertices) { result[v.Index] = v.UV; }
                    foreach (var v in _boundaryVertices) { result[v.Index] = v.UV; }

                    return result; 
                } 
            }

            private int _counter;

            public MeshData(int width, int height)
            {
                _width = width;
                _height = height;
                
                _vertices = new Vertex[width, height, Constants.NbSidesInSquare];
                _triangles = new List<int>();
            }
            
            public int AddVertex(int x, int y, float height, VertexDir dir)
            {
                var vertex = new Vertex(_counter++, x + XOffset(dir), height, y+YOffset(dir));
                _vertices[x, y, (int)dir] = vertex;
                return vertex.Index;
            }

            private static float YOffset(VertexDir dir) => dir switch 
            {
                VertexDir.Se or VertexDir.Sw => -.5f,
                _ => +.5f
            };

            private static float XOffset(VertexDir dir) =>  dir switch
            {
                VertexDir.Nw or VertexDir.Sw => -.5f,
                _ => +.5f
            };

            public int AddBoundaryVertex(int x, int y, VertexDir dir)
            {
                var vertex = new Vertex(_counter++, x + XOffset(dir), Terrain.MinHeight, y+YOffset(dir));
                _boundaryVertices.Add(vertex);
                return vertex.Index;
            }
            
            public Vertex GetVertex(int x, int y, VertexDir dir) =>  _vertices[x, y, (int)dir];

            public void AddTriangle(int v1, int v2, int v3)
            {
                _triangles.Add(v1);
                _triangles.Add(v2);
                _triangles.Add(v3);
            }
        }

        private class Vertex
        {
            public readonly int Index;
            public readonly Vector3 Position;
            public readonly Vector2 UV;

            public Vertex(int index, float x, float height, float z)
            {
                var offset = -0.5f;
                Index = index;
                Position = new Vector3(x, height, z);
                UV = new Vector2((x - offset)/Chunk.Size, (z - offset)/Chunk.Size);
            }
        }

        /** All vertex directions */
        private static readonly VertexDir[] AllDirections = { VertexDir.Nw, VertexDir.Ne, VertexDir.Se, VertexDir.Sw };
        
        /** Return the next Vertex dir in clockwise wind rose order */
        private static VertexDir Next(VertexDir curr) => (VertexDir)(((int)curr + 1) % AllDirections.Length);
        
        private enum VertexDir { Nw, Ne, Se, Sw }
    }
}