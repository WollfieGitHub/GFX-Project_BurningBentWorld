using System;
using System.Collections.Generic;
using Code.Scripts.TerrainGeneration.Components;
using Unity.VisualScripting;
using UnityEngine;

namespace Code.Scripts.TerrainGeneration.Rendering
{
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(ChunkCollider))]
    public class ChunkRenderer : MonoBehaviour
    {
        
//======== ====== ==== ==
//      PROPERTIES
//======== ====== ==== ==
        
        private Texture2D _texture;
        private Renderer _rend;
        private MeshFilter _meshFilter;
        private ChunkCollider _chunkCollider;
        
        private Mesh _mesh;

        private Chunk _chunk;

        /// <summary>
        /// Whether the mesh should be rendered using the cell's heights in the chunk
        /// or as a planar mesh if set to false
        /// </summary>
        private bool _renderMesh;
        ///<summary><inheritdoc cref="_displayType"/></summary>
        public bool RenderMesh
        {
            set
            {
                _renderMesh = value;
                StartCoroutine(nameof(CalcNewMesh));
            }
        }

        /// <summary>
        /// The <see cref="ChunkTexture.DisplayType"/> that
        /// the renderer should use to display the chunk
        /// </summary>
        private ChunkTexture.DisplayType _displayType;
        ///<summary><inheritdoc cref="_displayType"/></summary>
        public ChunkTexture.DisplayType DisplayType
        {
            set
            {
                _displayType = value;
                StartCoroutine(nameof(CalcNewTexture));
            }
        }
        
//======== ====== ==== ==
//      LIFECYCLE
//======== ====== ==== ==

        private void Awake()
        {
            _chunk = GetComponent<Chunk>();
            
            // Collect rendering components
            _rend = GetComponent<Renderer>();
            _meshFilter = GetComponent<MeshFilter>();
            _chunkCollider = GetComponent<ChunkCollider>();
        }

        private void OnEnable() => _chunk.NeighbourStateChanged += OnNeighbourChanged;
        private void OnDisable() => _chunk.NeighbourStateChanged -= OnNeighbourChanged;

        /// <summary>
        /// A neighbour's state has changed, we must recompute the
        /// mesh so that the boundaries adapt to the neighbouring chunk's bound appearance
        /// </summary>
        private void OnNeighbourChanged() => 
            StartCoroutine(nameof(CalcNewMesh));

        private void Start()
        {
            // Set up the texture.
            CalcNewTexture();
            CalcNewMesh();
        }
        
//======== ====== ==== ==
//      CONTROLS
//======== ====== ==== ==

        private void CalcNewMesh()
        {
            if (_mesh != null && !_mesh.IsDestroyed()) { Destroy(_mesh); }

            _mesh = _renderMesh ? GenerateChunkMesh(_chunk) : GeneratePlanarMesh();
            
            _meshFilter.sharedMesh = _mesh;
            
            _chunkCollider.Recalculate();
        }
        /// <summary>
        /// Recomputes the new texture for the mesh given the <see cref="_displayType"/>
        /// </summary>
        private void CalcNewTexture()
        {
            if (_texture != null && !_texture.IsDestroyed()) { Destroy(_texture); }
            
            _texture = ChunkTexture.From(_chunk, _displayType);
            _rend.material.mainTexture = _texture;
        }

        /// <summary>
        /// Set the materials of the renderer
        /// </summary>
        /// <param name="materials">The materials that the renderer should use</param>
        public void SetMaterials(IEnumerable<Material> materials) => 
            _rend.SetMaterials(new List<Material>(materials));
        
//======== ====== ==== ==
//      HELPERS
//======== ====== ==== ==
        
        /// <summary>
        /// Generate a planar mesh which size is the same
        /// as the chunk's
        /// </summary>
        /// <returns>The newly created plane mesh</returns>
        private static Mesh GeneratePlanarMesh()
        {
            var mesh = new Mesh();
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

        /// <summary>
        /// Generates a mesh for the chunk using se <see cref="ChunkMesh"/> class
        /// </summary>
        /// <param name="chunk">The chunk for which we generate the mesh</param>
        /// <returns>The newly created chunk mesh</returns>
        private static Mesh GenerateChunkMesh(Chunk chunk) => ChunkMesh.From(chunk);

    }
}