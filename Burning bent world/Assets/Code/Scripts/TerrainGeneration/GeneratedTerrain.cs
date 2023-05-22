﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private ChunkFactory _chunkFactory;

        private readonly ConcurrentDictionary<(int, int), Chunk> _chunks = new();

        /// <summary>
        /// Finds the chunk at coordinates (xChunk, zChunk) or null if
        /// the chunk is not currently loaded
        /// </summary>
        /// <param name="xChunk">The X coordinate in the chunk's referential</param>
        /// <param name="zChunk">The Z coordinate in the chunk's referential</param>
        /// <returns>The chunk found or null if none is loaded</returns>
        public Chunk GetChunkAt(int xChunk, int zChunk) => 
            _chunks.GetValueOrDefault((xChunk, zChunk));

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

            _chunkFactory = GetComponent<ChunkFactory>();
        }

        private void OnEnable()
        {
            _chunkFactory.OnChunkCreated += RegisterChunk;
            _chunkFactory.OnChunkDestroyed += UnregisterChunk;
        }
        
        private void OnDisable()
        {
            _chunkFactory.OnChunkCreated -= RegisterChunk;
            _chunkFactory.OnChunkDestroyed -= UnregisterChunk;
        }

// //======================================================================================\\
// ||                                                                                      ||
// ||                                         LIFECYCLE                                    ||
// ||                                                                                      ||
// \\======================================================================================//

        private void Update()
        {
            _manager.PlayerPosition = playerTransform.position;

            if (_needRefresh) { _manager.ChunkDistance = chunkDistance; }
        }

        private void RegisterChunk(Chunk chunk) => _chunks[(chunk.ChunkX, chunk.ChunkZ)] = chunk;
        private void UnregisterChunk(Chunk chunk) => _chunks.Remove((chunk.ChunkX, chunk.ChunkZ), out _);

    }
}