using System;
using System.Collections;
using System.Collections.Generic;
using Code.Scripts.TerrainGeneration.Rendering;
using Code.Scripts.Utils;
using TerrainGeneration.Rendering;
using UnityEngine;
using Utils;
using static Utils.Utils;

namespace Code.Scripts.TerrainGeneration.Components
{
    public class Chunk : MonoBehaviour, IEnumerable<Cell>
    {
//======== ====== ==== ==
//      EVENTS
//======== ====== ==== ==

        public event Action NeighbourStateChanged;

//======== ====== ==== ==
//      PROPERTIES
//======== ====== ==== ==

        /** Same as in Minecraft */
        public const int Size = 16;
        
        /** Cells from this chunk */
        private Efficient2DArray<Cell> _cells;
        
        /** X Coordinate relative to parent terrain's origin in number of chunks */
        [NonSerialized] public int ChunkX;
        /** Y Coordinate relative to parent terrain's origin in number of chunks */
        [NonSerialized] public int ChunkZ;

        public struct NeighbouringChunk
        {
            public Chunk Chunk;
            public bool Loaded;

            public static NeighbouringChunk Of(Chunk chunk)
            {
                return new NeighbouringChunk { Chunk = chunk, Loaded = true };
            }

            public static NeighbouringChunk None => new() { Loaded = false };
        }

        /// <summary>
        /// Convention :
        /// 0 -> North
        /// 1 -> East
        /// 2 -> South
        /// 3 -> West
        /// </summary>
        public NeighbouringChunk[] NeighbouringChunks = new NeighbouringChunk[Constants.NbChunkNeighbours];

        private readonly ChunkMap _mapGenerator;

//======== ====== ==== ==
//      CONSTRUCTOR
//======== ====== ==== ==

        public void Init(int xChunk, int zChunk, Efficient2DArray<Cell> cells)
        {
            ChunkX = xChunk;
            ChunkZ = zChunk;
            
            // Initialize cells
            _cells = cells;
        }

        public ChunkRenderer ChunkRenderer { private set; get; }
        public ChunkCollider ChunkCollider { private set; get; }

        /// <summary>
        /// Updates the reference to the chunk's objects
        /// to make sure they are always available
        /// </summary>
        /// <param name="chunkRenderer">Chunk Renderer</param>
        /// <param name="chunkCollider">Chunk Collider</param>
        public void UpdateRefs(
            ChunkRenderer chunkRenderer,
            ChunkCollider chunkCollider
        )
        {
            ChunkRenderer = chunkRenderer;
            ChunkCollider = chunkCollider;
        }

//======== ====== ==== ==
//      METHODS
//======== ====== ==== ==

        /// <param name="x">The x coordinate in the chunk's referential</param>
        /// <param name="z">The y coordinate in the chunk's referential</param>
        /// <returns>The <see cref="Cell"/> at coordinates x,y</returns>
        public Cell GetCellAt(int x, int z) => _cells[x, z];

        /// <summary>Finds the height (unity units) at the specified point</summary>
        /// <param name="x">X coordinate in the chunk's referential (between 0 and <see cref="Size"/>)</param>
        /// <param name="z">Z coordinate in the chunk's referential (between 0 and <see cref="Size"/>)</param>
        /// <returns>The height of the terrain at the specified coordinates</returns>
        public float GetHeightAt(int x, int z) => GetCellAt(x, z).Height;

        /// <summary>Finds the height (unity units) at the specified point or returns fallback if
        /// the cell is not within the bounds of the chunk</summary>
        /// <param name="x">X coordinate in the chunk's referential</param>
        /// <param name="z">Z coordinate in the chunk's referential</param>
        /// <param name="fallback">Fallback height in case the x, z coordinates are not in the grid's bounds</param>
        /// <returns>The height of the terrain at the specified coordinates</returns>
        public float GetHeightAtOrDefault(int x, int z, float fallback) =>
            IsInBounds(x, z, Size, Size) ? GetCellAt(x, z).Height : fallback;
        
        /// <summary>
        /// A neighbouring chunk has just been loaded
        /// </summary>
        /// <param name="chunk">The loaded chunk</param>
        /// <param name="loaded">True if the chunk is getting loaded, false if it is
        ///  getting unloaded</param>
        public void OnNeighbourLoadingStateChanged(Chunk chunk, bool loaded)
        {
            var index = (int)IndexFromCoordinates(
                ChunkX - chunk.ChunkX,
                ChunkZ - chunk.ChunkZ
            );

            NeighbouringChunks[index] = loaded ? NeighbouringChunk.Of(chunk) : NeighbouringChunk.None; 
            // Notify modules that a neighbour's state has changed
            NeighbourStateChanged?.Invoke();
        }

        private static Direction IndexFromCoordinates(int x, int z)
        {
            return z switch
            {
                > 0 => Direction.North,
                < 0 => Direction.South,
                _ => x switch
                {
                    > 0 => Direction.East,
                    < 0 => Direction.West,
                    _ => throw new ArgumentException("The chunk should not be identical")
                }
            };
        }

//======== ====== ==== ==
//      OVERRIDES
//======== ====== ==== ==

        /// <summary>Enumerates all cells in the order X then Y </summary>
        public IEnumerator<Cell> GetEnumerator()
        {
            for (var x = 0; x < Size; x++)
            {
                for (var z = 0; z < Size; z++)
                {
                    // Yield each cells
                    yield return GetCellAt(x, z);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        
        
//======== ====== ==== ==
//      HELPERS
//======== ====== ==== ==

        /// <summary>
        /// Compute the coordinates of the chunk in which the cell
        /// with the specified coordinates resides
        /// </summary>
        /// <param name="cellX">X Coordinate of the cell</param>
        /// <param name="cellZ">Z Coordinate of the cell</param>
        /// <returns>The (x,z) coordinates of the chunk</returns>
        public static (int, int) GetChunkCoordinatesOf(
            int cellX, int cellZ
        ) =>  (cellX / Size, cellZ / Size);
    }
}