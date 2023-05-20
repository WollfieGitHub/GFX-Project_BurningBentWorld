using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Code.Scripts.TerrainGeneration.Components;
using Code.Scripts.TerrainGeneration.Generators;
using TerrainGeneration.Components;
using UnityEngine;

namespace Code.Scripts.TerrainGeneration.Loaders
{
    public class TerrainManager : MonoBehaviour
    {
        private bool _started;
        
        private Vector3 _playerPosition;
        public Vector3 PlayerPosition
        {
            get => _playerPosition;
            set
            {
                _playerPosition = new Vector3(
                    Mathf.RoundToInt(value.x),
                    Mathf.RoundToInt(value.y),
                    Mathf.RoundToInt(value.z)
                );

                if (!_started) { return; }
                
                OnPositionUpdated();
            }
        }

        private int _chunkDistance;
        public int ChunkDistance
        {
            get => _chunkDistance;
            set
            {
                _chunkDistance = value;
                
                if (!_started) { return; }
                
                _chunkManager.RenderingDistance = value;
            }
        }

        /** Manages creation of the terrain's cells */
        private TerrainGenerator _generator;
        /** Manages creation and destruction of chunks */
        private ChunkFactory _chunkFactory;
        /** Manages loaded chunks */
        private ChunkManager _chunkManager;
        
        /** Manages loaded super chunks with their center as key */
        private readonly ConcurrentDictionary<(int, int), SuperChunk> _loadedSuperChunks = new();
        /** Manages the chunks referencing the super chunk represented by the key */
        private readonly ConcurrentDictionary<(int, int), List<Chunk>> _referencingChunks = new();
        /** Manages the loaded chunks */
        private readonly ConcurrentDictionary<(int, int), Chunk> _loadedChunks = new();

        private void Start()
        {
            _generator = GetComponent<TerrainGenerator>();
            _chunkFactory = GetComponent<ChunkFactory>();
            
            _chunkManager = new ChunkManager(
                LoadChunkAt, UnloadChunkAt,
                _chunkDistance, _playerPosition / Chunk.Size
            );

            _started = true;
        }

        private void OnPositionUpdated()
        {
            // On each chunk unloading, test if there is still 
            // a chunk referencing the super chunk, unload super chunk if not
            
            // On each chunk load, first load super chunk it is in if not already done
            _chunkManager.Position = PlayerPosition / Chunk.Size;
        }
        
// //======================================================================================\\
// ||                                                                                      ||
// ||                                       CHUNK LOADING                                  ||
// ||                                                                                      ||
// \\======================================================================================//

        /// <summary>
        /// Loads a chunk at the specified chunk coordinates
        /// </summary>
        /// <param name="xChunk">The X coordinate in chunk space</param>
        /// <param name="zChunk">The z coordinate in chunk space</param>
        private async void LoadChunkAt(int xChunk, int zChunk)
        {
            // Find the super chunk of the referenced chunk
            var (xSChunk, zSChunk) = SuperChunk.GetCoordinatesFrom(xChunk, zChunk);
            
            Debug.Log($"Loading super chunk with coordinates ({xSChunk}, {zSChunk})");

            // Loads the super chunk this chunk is in, if it is not already loaded
            if (!_loadedSuperChunks.TryGetValue((xSChunk, zSChunk), out var superChunk))
            {
                superChunk = new SuperChunk();
                _loadedSuperChunks[(xSChunk, zSChunk)] = superChunk;
                await superChunk.LoadAt(xSChunk, zSChunk, _generator);
            }

            var cells = await superChunk.GetChunkCells(xChunk, zChunk);
            // Create a chunk and a reference in the list of loaded chunks
            var chunk = _chunkFactory.CreateNew(xChunk, zChunk, cells);
            
            _loadedChunks[(xChunk, zChunk)] = chunk;

            // Add this chunk to the list of referencing chunks of the super chunk
            if (!_referencingChunks.TryGetValue((xSChunk, zSChunk), out var chunkList))
            {
                chunkList = new List<Chunk>();
                _referencingChunks[(xSChunk, zSChunk)] = chunkList;
            }
            chunkList.Add(chunk);
            
            // TODO Rest
        }

        /// <summary>
        /// Unloads a chunk at the specified coordinates
        /// </summary>
        /// <param name="xChunk">The X coordinate in chunk space</param>
        /// <param name="zChunk">The z coordinate in chunk space</param>
        private void UnloadChunkAt(int xChunk, int zChunk)
        {
            // Find the chunk this coordinates are referencing and delete it
            var chunk = _loadedChunks[(xChunk, zChunk)];
            _loadedChunks.Remove((xChunk, zChunk), out _);
            
            // Find the super chunk of this chunk
            var (xSChunk, zSChunk) = SuperChunk.GetCoordinatesFrom(xChunk, zChunk);
            
            // Remove the chunk from the chunk list of this super chunk
            var chunkList = _referencingChunks[(xSChunk, zSChunk)];
            chunkList.Remove(chunk);
            
            // Destroy the chunk from the game, we can safely
            // do this because the SuperChunk instance has a reference to the chunk's
            // cell and will be the one in charge of saving its data
            _chunkFactory.DestroyChunk(chunk);

            // If the list is empty, remove all references and unload super chunk
            if (chunkList.Count != 0) { return; }
            
            // Super Chunk has no references anymore
            _loadedSuperChunks[(xSChunk, zSChunk)].Unload();
            _loadedSuperChunks.Remove((xSChunk, zSChunk), out _);

            _referencingChunks.Remove((xSChunk, zSChunk), out _);

        }
    }
}