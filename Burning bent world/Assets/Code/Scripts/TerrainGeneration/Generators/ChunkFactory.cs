using System;
using Code.Scripts.TerrainGeneration.Components;
using TerrainGeneration.Components;
using TerrainGeneration.Rendering;
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
        
        private void Awake()
        {
            _transform = transform;
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
        public Chunk CreateNew(
            int xOffset, int zOffset, Cell[,] cells
        )
        {
            // Create the new chunk game object
            var chunkObj = new GameObject($"Chunk#{xOffset}#{zOffset}");

            // Setup transform
            SetupChunkTransform(xOffset, zOffset, chunkObj);

            // Setup chunk behaviours
            var chunk = chunkObj.AddComponent<Chunk>();
            chunk.Init(xOffset, zOffset, cells);

            // Setup collider
            var chunkCollider = chunkObj.AddComponent<ChunkCollider>();
            
            var rb = chunkObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            
            chunkObj.AddComponent<MeshRenderer>();
            chunkObj.AddComponent<MeshFilter>();
            
            // Setup renderer
            var chunkRenderer = chunkObj.AddComponent<ChunkRenderer>();
            
            // Update the chunk's references
            chunk.UpdateRefs(chunkRenderer, chunkCollider);

            OnChunkCreated?.Invoke(chunk);
            
            return chunk;
        }

        /** Setup the object's transform */
        private void SetupChunkTransform(int xOffset, int zOffset, GameObject chunkObj)
        {
            var chunkTransform = chunkObj.transform;
            chunkTransform.SetParent(_transform);

            chunkTransform.localPosition = new Vector3(
                xOffset * Chunk.Size, 0, zOffset * Chunk.Size
            );
        }

//======== ====== ==== ==
//      CHUNK DECONSTRUCTION
//======== ====== ==== ==

        /// <summary>
        /// Destroys the specified chunk
        /// </summary>
        /// <param name="chunk">The chunk to destroy</param>
        // TODO Check if there is something more to do here
        public void DestroyChunk(Chunk chunk)
        {
            OnChunkDestroyed?.Invoke(chunk);
            Destroy(chunk.gameObject);
        }
    }
}