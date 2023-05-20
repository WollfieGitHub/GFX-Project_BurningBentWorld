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
        private GeneratedTerrain _terrain;
        
//======== ====== ==== ==
//      LIFECYCLE
//======== ====== ==== ==
        
        private void Awake()
        {
            _terrain = GetComponent<GeneratedTerrain>();
        }

        private void Start() => LoadMaterials();

        private void LoadMaterials()
        {
            _materials = new Material[1/* TODO NbMaterials */];
            _materials[TerrainMaterialIdx] = _terrain.terrainMaterial;
            // TODO materials[GrassMaterialIdx] = _terrain.grassMaterial;
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
        /// <param name="chunkRenderer">The chunk renderer</param>
        public void RegisterChunkRenderer(ChunkRenderer chunkRenderer)
        {
            chunkRenderer.RenderMesh = _terrain.renderMesh;
            chunkRenderer.DisplayType = _terrain.displayType;
            chunkRenderer.SetMaterials(_materials);
            _chunkRenderers.Add(chunkRenderer);
        }
    }
}