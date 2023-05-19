using TerrainGeneration.Components;
using TerrainGeneration.Vegetation;
using Unity.VisualScripting;
using UnityEngine;

namespace TerrainGeneration.Rendering
{
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(ChunkCollider))]
    [RequireComponent(typeof(ChunkGrass))]
    public class ChunkRenderer : MonoBehaviour
    {
        private TerrainRenderer _terrainRenderer;

        private Transform _transform;

        private Texture2D _texture;
        private Renderer _rend;
        private MeshFilter _meshFilter;
        private ChunkGrass _chunkGrass;
        private ChunkCollider _chunkCollider;

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
                GenerateTexture();
            }
        }
    
        // Must wait for parent to setup in Awake
        public void Init(Chunk chunk)
        {
            _chunk = chunk;
            // Cache the transform
            _transform = transform;
            
            // Collect rendering components
            _rend = GetComponent<Renderer>();
            _meshFilter = GetComponent<MeshFilter>();
            _chunkCollider = GetComponent<ChunkCollider>();
            _chunkGrass = GetComponent<ChunkGrass>();

            _terrainRenderer = transform.parent.GetComponent<TerrainRenderer>();
            
            // Set up the texture.
            GenerateTexture();
            CalcNewMesh();
        }

        private void CalcNewMesh()
        {
            if (_mesh != null && !_mesh.IsDestroyed()) { Destroy(_mesh); }

            _mesh = _renderMesh ? GenerateChunkMesh(_chunk) : GeneratePlanarMesh(_chunk);
            
            _meshFilter.sharedMesh = _mesh;
            
            _chunkCollider.Recalculate();
            _chunkGrass.Recalculate(_chunk);
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

        /// <summary>
        /// Generates a texture from the chunk's information and applies it.
        /// </summary>
        public void GenerateTexture()
        {
            if (_texture != null && !_texture.IsDestroyed()) { Destroy(_texture); }
            
            _texture = ChunkTexture.From(_chunk, _displayType);
            _rend.material.mainTexture = _texture;
        }
    }
}