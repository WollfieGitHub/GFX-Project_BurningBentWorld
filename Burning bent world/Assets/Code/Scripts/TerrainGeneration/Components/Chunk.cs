using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Scripts.TerrainGeneration.Loaders;
using Code.Scripts.TerrainGeneration.Rendering;
using Code.Scripts.TerrainGeneration.Vegetation.Plants.ProceduralGrass;
using Code.Scripts.Utils;
using TerrainGeneration.Rendering;
using UnityEngine;
using UnityEngine.Rendering.UI;
using Utils;
using static Utils.Utils;

namespace Code.Scripts.TerrainGeneration.Components
{
    public class Chunk : MonoBehaviour, IEnumerable<Cell>
    {
//======== ====== ==== ==
//      EVENTS
//======== ====== ==== ==

        public event Action<Direction> NeighbourStateChanged;

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

            public bool GetIfLoaded(out Chunk chunk)
            {
                if (Loaded)
                {
                    chunk = Chunk;
                    return true;
                }
                chunk = null;
                return false;
            }
        }

        private DirIndexArray<NeighbouringChunk> _neighbouringChunks = new();
        /// <summary>
        /// Convention :
        /// 0 -> North
        /// 1 -> East
        /// 2 -> South
        /// 3 -> West
        /// </summary>
        public DirIndexArray<NeighbouringChunk> NeighbouringChunks
        {
            get
            {
                // Cache but refetch if not loaded
                for (var i = 0; i < 4; i++)
                {
                    var dir = (Direction)i;

                    if (!_neighbouringChunks[dir].Loaded)
                    {
                        _neighbouringChunks[dir] = dir switch
                        {
                            Direction.North => _terrainManager.TryGetChunkAt(ChunkX, ChunkZ+1, out var north) 
                                ? NeighbouringChunk.Of(north)
                                : NeighbouringChunk.None,
                            Direction.East => _terrainManager.TryGetChunkAt(ChunkX+1, ChunkZ, out var east) 
                                ? NeighbouringChunk.Of(east)
                                : NeighbouringChunk.None,
                            Direction.South => _terrainManager.TryGetChunkAt(ChunkX, ChunkZ-1, out var south) 
                                ? NeighbouringChunk.Of(south)
                                : NeighbouringChunk.None,
                            Direction.West => _terrainManager.TryGetChunkAt(ChunkX-1, ChunkZ, out var west) 
                                ? NeighbouringChunk.Of(west)
                                : NeighbouringChunk.None,
                            _ => throw new InvalidOperationException()
                        };
                    }
                }

                return _neighbouringChunks;
            }
        }

        /// <summary>
        /// The neighbours which already notified this chunk they existed
        /// </summary>
        private readonly DirIndexArray<bool> _noticedNeighbour = new();

        private TerrainManager _terrainManager;

//======== ====== ==== ==
//      INITIALIZATION
//======== ====== ==== ==

        public void Init(int xChunk, int zChunk, Efficient2DArray<Cell> cells)
        {
            ChunkX = xChunk;
            ChunkZ = zChunk;
            
            // Initialize cells
            _cells = cells;
        }

        public ChunkRenderer ChunkRenderer { private set; get; }
        public ChunkGrass ChunkGrass       { private set; get; }

        /// <summary>
        /// Updates the reference to the chunk's objects
        /// to make sure they are always available
        /// </summary>
        /// <param name="chunkRenderer">Chunk Renderer</param>
        /// <param name="chunkCollider">Chunk Collider</param>
        /// <param name="chunkGrass">Chunk Grass</param>
        public void UpdateRefs(
            ChunkRenderer chunkRenderer,
            ChunkGrass chunkGrass
        )
        {
            ChunkRenderer = chunkRenderer;
            ChunkGrass = chunkGrass;
        }

//======== ====== ==== ==
//      LIFECYCLE
//======== ====== ==== ==

        private void Awake()
        {
            _terrainManager = GetComponentInParent<TerrainManager>();
        }

        private void Start()
        {
            NotifyNeighbour(Direction.South);
            NotifyNeighbour(Direction.West);
            
            NotifySelf(Direction.North);
            NotifySelf(Direction.East);
        }

        private void NotifyNeighbour(Direction direction)
        {
            if (!NeighbouringChunks[direction].GetIfLoaded(out var neighbour)) { return; }
                
            neighbour.OnNeighbourLoaded(direction.Opposite());
        }

        private void NotifySelf(Direction direction)
        {
            // If the neighbour is already loaded, then we can take its texture
            if (!NeighbouringChunks[direction].Loaded) { return; }
            OnNeighbourLoaded(direction);
        }

//======== ====== ==== ==
//      METHODS
//======== ====== ==== ==

        private void OnNeighbourLoaded(Direction direction)
        {
            // If already notified, return
            if (_noticedNeighbour[direction]) return;
            
            NeighbourStateChanged?.Invoke(direction);
            _noticedNeighbour[direction] = true;
        }

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