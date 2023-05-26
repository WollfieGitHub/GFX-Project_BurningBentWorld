using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Scripts.TerrainGeneration.Components;
using Code.Scripts.TerrainGeneration.Generators;
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
        private readonly ConcurrentDictionary<(int, int), Lazy<SuperChunk>> _loadedSuperChunks = new();
        /** Manages the chunks referencing the super chunk represented by the key */
        private readonly ConcurrentDictionary<(int, int), List<Chunk>> _referencingChunks = new();
        /** Manages the loaded chunks */
        private readonly ConcurrentDictionary<(int, int), Chunk> _loadedChunks = new();

        private void Start()
        {
            _generator = GetComponent<TerrainGenerator>();
            _chunkFactory = GetComponent<ChunkFactory>();
            
            SuperChunk.PurgeSavedSuperChunks();
            
            _chunkManager = new ChunkManager(
                (x, z) => Task.Run(() => LoadChunkAt(x, z)),
                (x, z) => Task.Run(() => UnloadChunkAt(x, z)),
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

        // Note : All the loading/unloading logic should be asynchronous to the player thread

        /// <summary>
        /// Loads a chunk at the specified chunk coordinates
        /// </summary>
        /// <param name="xChunk">The X coordinate in chunk space</param>
        /// <param name="zChunk">The z coordinate in chunk space</param>
        private async Task LoadChunkAt(int xChunk, int zChunk)
        {
            // Find the super chunk of the referenced chunk
            var (xSChunk, zSChunk) = SuperChunk.GetCoordinatesFrom(xChunk, zChunk);

            // Test if super chunk is already loaded
            var superChunk = _loadedSuperChunks.GetOrAdd((xSChunk, zSChunk), key =>
            {
                // As pointed out in : https://stackoverflow.com/questions/31637394/is-concurrentdictionary-getoradd-guaranteed-to-invoke-valuefactorymethod-only
                // Use lazy, otherwise the chunk gets created multiple times
                return new Lazy<SuperChunk>(() =>
                {
                    var (x, z) = key;
                    // If we managed to add, means it is not loaded
                    // then create/load from disk the super chunk
                    var createdSuperChunk = new SuperChunk(x, z, _generator);
                    return createdSuperChunk;
                });
            });

            var cells = await superChunk.Value.GetChunkCells(xChunk, zChunk);
            // Create a chunk and a reference in the list of loaded chunks
            var chunk = await _chunkFactory.CreateNew(xChunk, zChunk, cells);

            _loadedChunks[(xChunk, zChunk)] = chunk;

            // Add this chunk to the list of referencing chunks of the super chunk
            if (!_referencingChunks.TryGetValue((xSChunk, zSChunk), out var chunkList))
            {
                chunkList = new List<Chunk>();
                _referencingChunks[(xSChunk, zSChunk)] = chunkList;
            }
            chunkList.Add(chunk);
        }

        /// <summary>
        /// Unloads a chunk at the specified coordinates
        /// </summary>
        /// <param name="xChunk">The X coordinate in chunk space</param>
        /// <param name="zChunk">The z coordinate in chunk space</param>
        private async Task UnloadChunkAt(int xChunk, int zChunk)
        {
            // Find the chunk this coordinates are referencing and delete it
            if (!_loadedChunks.TryGetValue((xChunk, zChunk), out var chunk))
            {
                Debug.LogWarning("Trying to unload chunk at coordinate " +
                                 $"({xChunk}, {zChunk}) but the chunk was not found");
                return;
            }

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
            await _loadedSuperChunks[(xSChunk, zSChunk)].Value.Unload();
            _loadedSuperChunks.Remove((xSChunk, zSChunk), out _);

            _referencingChunks.Remove((xSChunk, zSChunk), out _);
        }

        /// <summary>
        /// Tries to find a loaded chunk at the specified coordinates.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <param name="chunk">The chunk found if any</param>
        /// <returns>True if a loaded chunk is found, false otherwise</returns>
        public bool TryGetChunkAt(int x, int z, out Chunk chunk)
        {
            return _loadedChunks.TryGetValue((x, z), out chunk);
        }
        
    }
}