using System;
using Code.Scripts.TerrainGeneration.Components;
using Code.Scripts.TerrainGeneration.Generators;
using Code.Scripts.TerrainGeneration.Loaders;
using Code.Scripts.TerrainGeneration.Rendering;
using TerrainGeneration.Rendering;
using UnityEngine;

namespace Code.Scripts.TerrainGeneration
{
// //======================================================================================\\
// ||                                                                                      ||
// ||                                        DELEGATES                                     ||
// ||                                                                                      ||
// \\======================================================================================//

    public delegate CellInfo[,] CellMap(int x, int z, int width, int height);

    public delegate CellInfo ChunkMap(int x, int z);
    
// //======================================================================================\\
// ||                                                                                      ||
// ||                                     GENERATED TERRAIN                                ||
// ||                                                                                      ||
// \\======================================================================================//
    
    [RequireComponent(typeof(TerrainGenerator))]
    [RequireComponent(typeof(TerrainManager))]
    [RequireComponent(typeof(TerrainRenderer))]
    [RequireComponent(typeof(ChunkFactory))]
    public class GeneratedTerrain : MonoBehaviour
    {

// //======================================================================================\\
// ||                                                                                      ||
// ||                                        PROPERTIES                                    ||
// ||                                                                                      ||
// \\======================================================================================//
        
//======== ====== ==== ==
//      SERIALIZED PROPERTIES
//======== ====== ==== ==

        [Header("Base")]
        [SerializeField] private Transform playerTransform;

        [Header("Performance")] 
        [SerializeField] [Range(1, 32)] private int chunkDistance = 4;

        [Header("Appearance")]
        [SerializeField] public bool renderMesh = true;
        [SerializeField] public ChunkTexture.DisplayType displayType;
        [SerializeField] public Material terrainMaterial;
        [SerializeField] public Material grassMaterial;

        private bool _needRefresh;
        
        public TerrainRenderer Renderer { get; private set; }

        private void OnValidate()
        {
            _needRefresh = true;
        }

//======== ====== ==== ==
//      TERRAIN OBJECTS
//======== ====== ==== ==

        private TerrainManager _manager;
        private TerrainRenderer _renderer;
        private TerrainGenerator _generator;

// //======================================================================================\\
// ||                                                                                      ||
// ||                                       LIFECYCLE                                      ||
// ||                                                                                      ||
// \\======================================================================================//

        private void Awake()
        {
            
            _manager = GetComponent<TerrainManager>();
            Renderer = GetComponent<TerrainRenderer>();
            _generator = GetComponent<TerrainGenerator>();
            
            Debug.Log("Starting terrain generation...");

            _manager.PlayerPosition = playerTransform.position;
            _manager.ChunkDistance = chunkDistance;
        }
        
// //======================================================================================\\
// ||                                                                                      ||
// ||                                       UTIL METHODS                                   ||
// ||                                                                                      ||
// \\======================================================================================//

        private void Update()
        {
            _manager.PlayerPosition = playerTransform.position;

            if (_needRefresh)
            {
                _manager.ChunkDistance = chunkDistance;
            }
        }
        
    }
}