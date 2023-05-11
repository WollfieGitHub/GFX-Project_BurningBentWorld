using System;
using System.Collections;
using System.Collections.Generic;
using TerrainGeneration.Generators;
using TerrainGeneration.Noises;
using UnityEngine;
using Utils;
using static Utils.Utils;
using Random = UnityEngine.Random;

namespace TerrainGeneration.Components
{
    public class Chunk : IEnumerable<Cell>
    {
//======== ====== ==== ==
//      PROPERTIES
//======== ====== ==== ==

        /** Same as in Minecraft */
        public const int Size = 16;
        
        public int Width => Size;
        public int Height => Size;

        /**
         * Biome of the chunk
         * A chunk has only one biome but TODO interpolation between neighbouring chunks
         * with different biomes is done to get a smooth appearance transition
         */
        private Biome _biome;
        /** <inheritdoc cref="_biome"/> */
        public Biome Biome {
            private set => _biome = value;
            get
            {
                if (!Initialized) { Initialize(); }
                return _biome;
            }
        }

        /** The area of the chunk */
        private static float Area => Size * Size;

        /** Cells from this chunk */
        private Cell[,] _cells;
        
        /** X Coordinate relative to parent terrain's origin in number of chunks */
        public readonly int ChunkX;
        /** Y Coordinate relative to parent terrain's origin in number of chunks */
        public readonly int ChunkZ;

        private readonly ChunkMap _mapGenerator;

        /** Whether the chunk has been initialized at least once or not */
        public bool Initialized { get; private set; } = false;
        /** Whether the chunk is actually loaded or not */
        public bool Loaded { get; private set; } = false;

//======== ====== ==== ==
//      CONSTRUCTOR
//======== ====== ==== ==

        public Chunk(int xChunk, int zChunk, ChunkMap mapGenerator)
        {
            ChunkX = xChunk;
            ChunkZ = zChunk;
            _mapGenerator = mapGenerator;
        }
        
//======== ====== ==== ==
//      METHODS
//======== ====== ==== ==

        /// <param name="x">The x coordinate in the chunk's referential</param>
        /// <param name="z">The y coordinate in the chunk's referential</param>
        /// <returns>The <see cref="Cell"/> at coordinates x,y</returns>
        public Cell GetCellAt(int x, int z)
        {
            // Initialize the whole chunk if it wasn't before
            if (!Initialized) { Initialize(); }
            return _cells[x, z];
        }

        /// <summary>
        /// Initialize the chunk
        /// </summary>
        private void Initialize()
        {
            var cellHeights = ChunkGenerator.GenerateHeight(ChunkX, ChunkZ, _mapGenerator);
            
            // First save cells into the chunk
            _cells = cellHeights;

            // Set the chunk as initialized once the cells are loaded
            Initialized = true;
            
            // Find corresponding biome
            Biome = default;
        }

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
    }
}