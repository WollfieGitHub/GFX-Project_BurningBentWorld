using System;
using System.Collections.Generic;
using Code.Scripts.TerrainGeneration.Components;
using Code.Scripts.TerrainGeneration.Rendering;
using Code.Scripts.TerrainGeneration.Vegetation.Plants.ProceduralGrass;
using TerrainGeneration.Components;
using TerrainGeneration.Vegetation;
using Unity.VisualScripting;
using UnityEngine;

namespace TerrainGeneration.Rendering
{
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(ChunkCollider))]
    public class ChunkRenderer : MonoBehaviour
    {
        
        private Texture2D _texture;
        private Renderer _rend;
        private MeshFilter _meshFilter;
        private ChunkCollider _chunkCollider;
        
        private Mesh _mesh;

        private Chunk _chunk;

        private bool _renderMesh;
        public bool RenderMesh
        {
            set
            {
                _renderMesh = value;
                StartCoroutine(nameof(CalcNewMesh));
            }
        }

        private ChunkTexture.DisplayType _displayType;
        public ChunkTexture.DisplayType DisplayType
        {
            set
            {
                _displayType = value;
                StartCoroutine(nameof(CalcNewTexture));
            }
        }

        private void Awake()
        {
            _chunk = GetComponent<Chunk>();
            
            // Collect rendering components
            _rend = GetComponent<Renderer>();
            _meshFilter = GetComponent<MeshFilter>();
            _chunkCollider = GetComponent<ChunkCollider>();
        }
        
        private void Start()
        {
            // Set up the texture.
            CalcNewTexture();
            CalcNewMesh();
        }

        private void CalcNewMesh()
        {
            if (_mesh != null && !_mesh.IsDestroyed()) { Destroy(_mesh); }

            _mesh = _renderMesh ? GenerateChunkMesh(_chunk) : GeneratePlanarMesh();
            
            _meshFilter.sharedMesh = _mesh;
            
            _chunkCollider.Recalculate();
        }

        private Mesh GeneratePlanarMesh()
        {
            Mesh mesh = new Mesh();
            var vertices = new [] {
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(Chunk.Size, 0.0f, 0.0f),
                new Vector3(0.0f, 0.0f, Chunk.Size),
                new Vector3(Chunk.Size, 0.0f, Chunk.Size),
            };
            var triangles = new[]
            {
                0, 3, 1,
                0, 2, 3
            };
            var uvs = new[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.Optimize();
            mesh.RecalculateNormals();

            return mesh;
        }

        private Mesh GenerateChunkMesh(Chunk chunk) => ChunkMesh.From(chunk);

        private void CalcNewTexture()
        {
            if (_texture != null && !_texture.IsDestroyed()) { Destroy(_texture); }
            
            _texture = ChunkTexture.From(_chunk, _displayType);
            _rend.material.mainTexture = _texture;
        }

        public void SetMaterials(IEnumerable<Material> materials) => 
            _rend.SetMaterials(new List<Material>(materials));
    }
}