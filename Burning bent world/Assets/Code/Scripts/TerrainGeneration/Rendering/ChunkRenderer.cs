﻿using TerrainGeneration.Components;
using Unity.VisualScripting;
using UnityEngine;

namespace TerrainGeneration.Rendering
{
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class ChunkRenderer : MonoBehaviour
    {
        private TerrainRenderer _terrainRenderer;

        private Transform _transform;

        private Texture2D _texture;
        private Renderer _rend;
        private MeshFilter _meshFilter;

        private Mesh _mesh;

        private Chunk _chunk;

        private bool _renderMesh;
        public bool RenderMesh
        {
            get => _renderMesh;
            set
            {
                _renderMesh = value;
                if (_chunk == null) { return; }
                CalcNewMesh();
            }
        }

        private ChunkTexture.DisplayType _displayType;

        public ChunkTexture.DisplayType DisplayType
        {
            get => _displayType;
            set
            {
                _displayType = value;
                if (_chunk == null) { return; }
                CalcNewTexture();
            }
        }
    
        // Must wait for parent to setup in Awake
        private void Start()
        {
            // Cache the transform
            _transform = transform;
            
            _rend = GetComponent<Renderer>();
            _meshFilter = GetComponent<MeshFilter>();

            _terrainRenderer = transform.parent.GetComponent<TerrainRenderer>();

            var position = _transform.position;
            
            _chunk = _terrainRenderer.Terrain.GetChunkAt(
                (int)position.x,
                (int)position.z,
                true
            );

            // Set up the texture.
            CalcNewTexture();
            CalcNewMesh();
        }

        private void CalcNewMesh()
        {
            if (_mesh != null && !_mesh.IsDestroyed()) { Destroy(_mesh); }

            _mesh = _renderMesh ? GenerateChunkMesh(_chunk) : GeneratePlanarMesh(_chunk);
            
            _meshFilter.sharedMesh = _mesh;
        }

        private Mesh GeneratePlanarMesh(Chunk chunk)
        {
            Mesh mesh = new Mesh();
            var vertices = new [] {
                new Vector3(        0.0f, 0.0f,          0.0f),
                new Vector3(chunk.Width, 0.0f,          0.0f),
                new Vector3(        0.0f, 0.0f, chunk.Height),
                new Vector3(chunk.Width, 0.0f, chunk.Height),
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
    }
}