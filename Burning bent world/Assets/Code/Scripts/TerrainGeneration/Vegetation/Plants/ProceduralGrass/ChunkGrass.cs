using System.Diagnostics;
using Code.Scripts.TerrainGeneration.Vegetation.Plants;
using TerrainGeneration.Components;
using UnityEngine;

using static System.Runtime.InteropServices.Marshal;
using Terrain = TerrainGeneration.Components.Terrain;

namespace TerrainGeneration.Vegetation
{
        
    public class ChunkGrass : MonoBehaviour {
        private const int Resolution = 16;
        private const int MaxGrassCount = 16;
        private const float DisplacementStrength = Terrain.MaxHeight - Terrain.SeaLevel;
        private Material _grassMaterial;
        private Texture _grassMap;

        private ComputeShader _initializeGrassShader;
        private ComputeBuffer _grassDataBuffer, _grassIndicesBuffer;
        private Vector3[] _grassVertices;
        private int[] _grassIndices;
        private Bounds _bounds;
        private Mesh _grassMesh;

        public void Recalculate(Chunk chunk)
        {
            return;
            //var terrainGrass = GetComponentInParent<TerrainGrass>();

            //_grassMap = ComputeGrassHeightMap(chunk);

            //_initializeGrassShader = terrainGrass.ComputeShader;
            //_grassMaterial = terrainGrass.Material;
            
            //_grassDataBuffer = new ComputeBuffer(Resolution * Resolution, SizeOf(typeof(Vector3)));
            //_grassIndicesBuffer = new ComputeBuffer(Resolution * Resolution, SizeOf(typeof(int)));
            //_grassVertices = new Vector3[Resolution * Resolution];
            //_grassIndices = new int[Resolution * Resolution];
            //_bounds = new Bounds(Vector3.zero, new Vector3(-Resolution, DisplacementStrength * 2.0f, Resolution));

            //UpdateGrassMesh();
            //_grassDataBuffer.GetData(_grassVertices);
            //_grassIndicesBuffer.GetData(_grassIndices);

            //_grassMesh = new Mesh {name = "Grass"};
            //_grassMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            //_grassMesh.vertices = _grassVertices;
            //_grassMesh.SetIndices(_grassIndices, MeshTopology.Points, 0);

            //GetComponent<MeshRenderer>().sharedMaterial = _grassMaterial;
            //GetComponent<MeshFilter>().mesh = _grassMesh;
        }

        private Texture ComputeGrassHeightMap(Chunk chunk)
        {
            return Utils.Utils.CreateTexture(chunk.Width, chunk.Height, (x, z) =>
            {
                var cellInfo = chunk.GetCellAt(x, z).Info;

                var density = cellInfo.Land ? cellInfo.Biome.Vegetation.GrassCoefficient : 0f;
                var height = Mathf.InverseLerp(Terrain.SeaLevel, Terrain.MaxHeight, cellInfo.Height);

                return new Color(
                    height, density, 0f
                );
            });
        }

        void UpdateGrassMesh()
        {
            return;
            //_initializeGrassShader.SetInt("_Dimension", Resolution);
            //_initializeGrassShader.SetInt("_MaxGrassCount", MaxGrassCount);
            //_initializeGrassShader.SetBuffer(0, "_GrassDataBuffer", _grassDataBuffer);
            //_initializeGrassShader.SetBuffer(0, "_GrassIndicesBuffer", _grassIndicesBuffer);
            //_initializeGrassShader.SetTexture(0, "_GrassMap", _grassMap);
            //_initializeGrassShader.SetFloat("_DisplacementStrength", DisplacementStrength);
            //_initializeGrassShader.Dispatch(
            //    0, Mathf.CeilToInt(Resolution / 8.0f), 
            //    Mathf.CeilToInt(Resolution / 8.0f), 1
            //);
        }

        void OnDisable()
        {
            return;
            //_grassDataBuffer.Release();
            //_grassDataBuffer = null;
            //_grassIndicesBuffer.Release();
            //_grassIndicesBuffer = null;
        }
    }

}