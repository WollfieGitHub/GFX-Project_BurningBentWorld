using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Code.Scripts.TerrainGeneration.Components;
using Code.Scripts.TerrainGeneration.Rendering;
using Code.Scripts.TerrainGeneration.Vegetation.Plants.ProceduralGrass;
using Code.Scripts.Utils;
using UnityEngine;

namespace Code.Scripts.TerrainGeneration.Generators
{
    /// <summary>
    /// Its job is to build and destroy chunks, it is sitting on the same
    /// game object as the terrain and can therefore access its characteristics
    /// </summary>
    public class ChunkFactory : MonoBehaviour
    {
        public event Action<Chunk> OnChunkCreated;
        public event Action<Chunk> OnChunkDestroyed;
        
        private Transform _transform;
        private GeneratedTerrain _terrain;

        private readonly ConcurrentQueue<Task> _chunkCreationQueue = new();

        private bool _busy;

        private const int MaxTaskPerFrame = 32;

        private void Awake()
        {
            _transform = transform;
            _terrain = GetComponent<GeneratedTerrain>();
        }
        
        // DONE Idea : Queue all creation tasks and run them in update loop 
        // Return in CreateNew a Task<Chunk> and resolve the task once it has been
        // processed by main loop

        private void Update()
        {
            if (_chunkCreationQueue.IsEmpty || _busy) return;

            EmptyTaskQueue();
        }

        private void EmptyTaskQueue()
        {
            var counter = 0;
            while (!_chunkCreationQueue.IsEmpty && counter < MaxTaskPerFrame)
            {
                if (_chunkCreationQueue.TryDequeue(out var chunkCreationTask))
                {
                    chunkCreationTask.RunSynchronously();
                }
                counter++;
            }
        }

//======== ====== ==== ==
//      CHUNK CONSTRUCTION
//======== ====== ==== ==

        /// <summary>
        /// Creates a new <see cref="Chunk"/> with the specified position offset
        /// in <see cref="Chunk"/>'s coordinates
        /// </summary>
        /// <param name="xOffset">The X offset in <see cref="Chunk"/>'s coordinates</param>
        /// <param name="zOffset">The X offset in <see cref="Chunk"/>'s coordinates</param>
        /// <param name="cells">The cells of the chunk, array of size
        /// <see cref="Chunk.Size"/> x <see cref="Chunk.Size"/></param>
        /// <returns>The newly created <see cref="Chunk"/></returns>
        public async Task<Chunk> CreateNew(
            int xOffset, int zOffset, Efficient2DArray<Cell> cells
        )
        {
            var chunkCreationTask = new Task<Chunk>(() =>
            {
                // DONE : Must be done on player thread, add component may be too
                // Create the new chunk game object
                var chunkObj = new GameObject($"Chunk#{xOffset}#{zOffset}");

                // Setup transform
                var chunkTransform = SetupChunkTransform(xOffset, zOffset, chunkObj);

                // Setup chunk behaviours
                var chunk = chunkObj.AddComponent<Chunk>();
                chunk.Init(xOffset, zOffset, cells);

                // Setup collider
                chunkObj.AddComponent<MeshCollider>();
                // Setup water
                Instantiate(
                    _terrain.waterPrefab, 
                    chunkTransform.position - new Vector3(0, 1, 0),
                    Quaternion.identity,
                    chunkTransform    
                );
                // Setup grass

                var rb = chunkObj.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;
                
                chunkObj.AddComponent<MeshRenderer>();
                chunkObj.AddComponent<MeshFilter>();

                // Setup renderer
                var chunkRenderer = chunkObj.AddComponent<ChunkRenderer>();
                // Setup grass
                var chunkGrass = chunkObj.AddComponent<ChunkGrass>();

                // Update the chunk's references
                chunk.UpdateRefs(chunkRenderer, chunkGrass);

                OnChunkCreated?.Invoke(chunk);

                return chunk;
            });
            
            _chunkCreationQueue.Enqueue(chunkCreationTask);
            
            return await chunkCreationTask;
        }

        /** Setup the object's transform */
        private Transform SetupChunkTransform(int xOffset, int zOffset, GameObject chunkObj)
        {
            var chunkTransform = chunkObj.transform;
            chunkTransform.SetParent(_transform);

            chunkTransform.localPosition = new Vector3(
                xOffset * Chunk.Size, 0, zOffset * Chunk.Size
            );
            return chunkTransform;
        }

//======== ====== ==== ==
//      CHUNK DECONSTRUCTION
//======== ====== ==== ==

        /// <summary>
        /// Destroys the specified chunk
        /// </summary>
        /// <param name="chunk">The chunk to destroy</param>
        public void DestroyChunk(Chunk chunk)
        {
            var chunkDestructionTask = new Task(() =>
            {
                OnChunkDestroyed?.Invoke(chunk);
                Destroy(chunk.gameObject);
            });
            
            _chunkCreationQueue.Enqueue(chunkDestructionTask);
        }
    }
}