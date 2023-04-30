using System;
using System.Collections;
using System.Collections.Generic;
using TerrainGeneration.Generators;
using UnityEngine;
using Utils;

namespace TerrainGeneration.Components
{
    public class Chunk : IEnumerable<Cell>
    {
//======== ====== ==== ==
//      PROPERTIES
//======== ====== ==== ==

        /** Same as in Minecraft */
        public const int Size = 16;

        /**
         * Biome of the chunk
         * A chunk has only one biome but TODO interpolation between neighbouring chunks
         * with different biomes is done to get a smooth appearance transition
         */
        public Biome Biome { private set; get; }

        /** The area of the chunk */
        private static float Area => Size * Size;

        /** Cells from this chunk */
        private Cell[,] _cells;
        
        /** X Coordinate relative to parent terrain's origin */
        public readonly int PosX;
        /** Y Coordinate relative to parent terrain's origin */
        public readonly int PosY;

        /** Whether the chunk has been initialized at least once or not */
        public bool Initialized { get; private set; } = false;
        /** Whether the chunk is actually loaded or not */
        public bool Loaded { get; private set; } = false;

//======== ====== ==== ==
//      CONSTRUCTOR
//======== ====== ==== ==

        public Chunk(int xChunk, int yChunk)
        {
            PosX = xChunk;
            PosY = yChunk;
        }
        
//======== ====== ==== ==
//      METHODS
//======== ====== ==== ==

        /// <param name="x">The x coordinate in the chunk's referential</param>
        /// <param name="y">The y coordinate in the chunk's referential</param>
        /// <returns>The <see cref="Cell"/> at coordinates x,y</returns>
        public Cell GetCellAt(int x, int y)
        {
            // Initialize the whole chunk if it wasn't before
            if (!Initialized) { Initialize(); }
            return _cells[x, y];
        }

        /// <summary>
        /// Initialize the chunk
        /// </summary>
        private void Initialize()
        {
            var (cellHeights, moistureLevel) = ChunkGenerator.Generate(PosX, PosY);
            
            // First save cells into the chunk
            _cells = new Cell[Size,Size];
            
            for (var x = 0; x < Size; x++) { for (var y = 0; y < Size; y++) {
                _cells[x, y] = new Cell(cellHeights[x, y]);
            } }
            // Setup biome from elevation of cells and moisture level
            var elevation = GetElevation();
            Biome = Biome.GetFrom(elevation, moistureLevel);

            // Set the chunk as initialized
            Initialized = true;
        }

        /// <summary>The elevation of the chunk given from the average of cells height</summary>
        /// <returns>The elevation from <see cref="Biome.MinElevation"/> to <see cref="Biome.MaxElevation"/></returns>
        public int GetElevation()
        {
            var total = 0f;
            foreach (var cell in this)
            {
                total += GetHeightAt(cell.Position.x, cell.Position.y);
            }

            // Compute average for the chunk
            var chunkAverage = total / Area;
            // Compute average relative to terrain Min/Max
            var relativeAverage = (chunkAverage - Terrain.MinHeight) / (Terrain.MaxHeight - Terrain.MinHeight);
            // Compute relative to biome elevation and cast as int
            return Mathf.FloorToInt(relativeAverage * (Biome.MaxElevation - Biome.MinElevation) + Biome.MinElevation);
        }

        /// <summary>Finds the height (unity units) at the specified point</summary>
        /// <param name="x">X coordinate in the chunk's referential (between 0 and <see cref="Size"/>)</param>
        /// <param name="y">Y coordinate in the chunk's referential (between 0 and <see cref="Size"/>)</param>
        /// <returns>The height of the terrain at the specified coordinates</returns>
        public float GetHeightAt(float x, float y)
        {
            return _cells[Mathf.RoundToInt(x), Mathf.RoundToInt(y)].Height;
        }
        
//======== ====== ==== ==
//      OVERRIDES
//======== ====== ==== ==

        /// <summary>Enumerates all cells in the order X then Y </summary>
        public IEnumerator<Cell> GetEnumerator()
        {
            for (var x = 0; x < Size; x++)
            {
                for (var y = 0; y < Size; y++)
                {
                    // Yield each cells
                    yield return _cells[x, y];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}