using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Scripts.TerrainGeneration;
using Code.Scripts.TerrainGeneration.Components;
using Code.Scripts.TerrainGeneration.Generators;
using Code.Scripts.TerrainGeneration.Loaders;
using Code.Scripts.TerrainGeneration.Rendering;
using TerrainGeneration.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace TerrainGeneration.Rendering
{
    [RequireComponent(typeof(TerrainGenerator))]
    public class TerrainRenderer : MonoBehaviour
    {
        
//======== ====== ==== ==
//      PROPERTIES
//======== ====== ==== ==
        
        public const int NbMaterials = 2;
        public const int TerrainMaterialIdx = 0;
        public const int GrassMaterialIdx = 1;
        
        private Material[] _materials;
        
        public bool needMeshRefresh;
        private void OnValidate() => needMeshRefresh = true;
        
        private readonly List<ChunkRenderer> _chunkRenderers = new();

        private ChunkFactory _chunkFactory;
        private GeneratedTerrain _terrain;
        
//======== ====== ==== ==
//      LIFECYCLE
//======== ====== ==== ==
        
        private void Awake()
        {
            _chunkFactory = GetComponent<ChunkFactory>();
            _terrain = GetComponent<GeneratedTerrain>();
        }

        private void OnEnable()
        {
            _chunkFactory.OnChunkCreated += RegisterChunkRenderer;
            _chunkFactory.OnChunkDestroyed += UnregisterChunkRenderer;
        }
        
        private void OnDisable()
        {
            _chunkFactory.OnChunkCreated -= RegisterChunkRenderer;
            _chunkFactory.OnChunkDestroyed -= UnregisterChunkRenderer;
        }

        private void Start() => LoadMaterials();

        private void LoadMaterials()
        {
            var materials = new List<Material> { _terrain.terrainMaterial };

            if (_terrain.renderGrass) { materials.Add(_terrain.grassMaterial); }

            _materials = materials.ToArray();
        }
        
        private void Update()
        {
            if (needMeshRefresh)
            {
                needMeshRefresh = false;
                StartCoroutine(nameof(RecalculateMesh));
            }
        }
        
//======== ====== ==== ==
//      CONTROLS
//======== ====== ==== ==
        
        /// <summary>
        /// Force recalculation of mesh and texture of all chunks in the terrain
        /// </summary>
        private void RecalculateMesh()
        {
            foreach (var chunkRenderer in _chunkRenderers)
            {
                chunkRenderer.RenderMesh = _terrain.renderMesh;
                chunkRenderer.DisplayType = _terrain.displayType;
            }
        }

        /// <summary>
        /// Register the chunk renderer for the terrain
        /// </summary>
        /// <param name="chunk">The chunk</param>
        public void RegisterChunkRenderer(Chunk chunk)
        {
            var chunkRenderer = chunk.ChunkRenderer;
            
            chunkRenderer.RenderMesh = _terrain.renderMesh;
            chunkRenderer.DisplayType = _terrain.displayType;
            chunkRenderer.SetMaterials(_materials);

            var chunkGrass = chunk.ChunkGrass;

            if (_terrain.renderGrass)
            {
                chunkGrass.SetMaterial(_terrain.grassMaterial);
                
            } else { chunkGrass.enabled = false; }
            
            _chunkRenderers.Add(chunkRenderer);
        }

        /// <summary>
        /// Unregisters the chunk renderer for the terrain
        /// </summary>
        /// <param name="chunk">The chunk</param>
        public void UnregisterChunkRenderer(Chunk chunk)
        {
            var chunkRenderer = chunk.ChunkRenderer;

            _chunkRenderers.Remove(chunkRenderer);
        }
    }
}