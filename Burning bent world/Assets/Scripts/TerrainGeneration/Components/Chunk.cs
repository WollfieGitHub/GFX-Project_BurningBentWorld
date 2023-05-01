using System;
using System.Collections;
using System.Collections.Generic;
using TerrainGeneration.Generators;
using TerrainGeneration.Noises;
using UnityEngine;
using Utils;
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
        public readonly int ChunkY;

        private readonly MapGenerator _mapGenerator;

        /** Whether the chunk has been initialized at least once or not */
        public bool Initialized { get; private set; } = false;
        /** Whether the chunk is actually loaded or not */
        public bool Loaded { get; private set; } = false;

//======== ====== ==== ==
//      CONSTRUCTOR
//======== ====== ==== ==

        public Chunk(int xChunk, int yChunk, MapGenerator mapGenerator)
        {
            ChunkX = xChunk;
            ChunkY = yChunk;
            _mapGenerator = mapGenerator;
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
            var cellHeights = ChunkGenerator.GenerateHeight(ChunkX, ChunkY, _mapGenerator);
            var waterDepths = ChunkGenerator.GenerateWater(ChunkX, ChunkY, _mapGenerator);
            
            // First save cells into the chunk
            _cells = new Cell[Width,Height];
            
            for (var x = 0; x < Width; x++) { for (var y = 0; y < Height; y++) {
                _cells[x, y] = new Cell(
                    cellHeights[x, y] - waterDepths[x, y]
                );
            } }
            
            // Set the chunk as initialized once the cells are loaded
            Initialized = true;
            
            // Setup biome from elevation of cells and moisture level
            var elevation = GetElevation();
            var moisture = _mapGenerator.MoistureLevel;
            // Find corresponding biome
            Biome = Biome.GetFrom(elevation, moisture);
        }

        /// <summary>The elevation of the chunk given from the average of cells height</summary>
        /// <returns>The elevation from <see cref="Biome.MinElevation"/> to <see cref="Biome.MaxElevation"/></returns>
        public int GetElevation()
        {
            var total = 0f;
            for (var x = 0; x < Width; x++) { for (var y = 0; y < Height; y++) {
                total += GetHeightAt(x, y);
            } }

            // Compute average for the chunk
            var chunkAverage = total / Area;
            // Compute average relative to terrain Min/Max
            var relativeAverage = Mathf.InverseLerp(Terrain.MinHeight, Terrain.MaxHeight, chunkAverage);
            // Compute relative to biome elevation and cast as int
            return Mathf.FloorToInt(relativeAverage * (Biome.MaxElevation - Biome.MinElevation) + Biome.MinElevation);
        }

        /// <summary>Finds the height (unity units) at the specified point</summary>
        /// <param name="x">X coordinate in the chunk's referential (between 0 and <see cref="Size"/>)</param>
        /// <param name="y">Y coordinate in the chunk's referential (between 0 and <see cref="Size"/>)</param>
        /// <returns>The height of the terrain at the specified coordinates</returns>
        public float GetHeightAt(int x, int y) => GetCellAt(x, y).Height;

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
                    yield return GetCellAt(x, y);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}