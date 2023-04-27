using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public Biome Biome { get; private set; }

        /** The area of the chunk */
        private float Area => Size * Size;

        /** Cells from this chunk */
        private readonly Cell[,] _cells;

//======== ====== ==== ==
//      CONSTRUCTOR
//======== ====== ==== ==

        public Chunk(Cell[,] cells, int moistureLevel)
        {
            // First save cells into the chunk
            _cells = cells;
            // Setup biome from elevation of cells and moisture level
            Biome = Biome.GetFrom(GetElevation(), moistureLevel);
        }
        
//======== ====== ==== ==
//      METHODS
//======== ====== ==== ==

        /**
         * <summary>The elevation of the chunk given from the average of cells height</summary>
         * <returns>The elevation from <see cref="Biome.MinElevation"/> to <see cref="Biome.MaxElevation"/></returns>
         */
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
            var relativeAverage = (chunkAverage + Terrain.MinHeight) / (Terrain.MaxHeight - Terrain.MinHeight);
            // Compute relative to biome elevation and cast as int
            return (int)(relativeAverage * (Biome.MaxElevation - Biome.MinElevation) + Biome.MinElevation);
        }

        /**
         * <summary>Finds the height (unity units) at the specified point</summary>
         * <param name="x">X coordinate in the chunk's referential (between 0 and <see cref="Size"/>)</param>
         * <param name="y">Y coordinate in the chunk's referential (between 0 and <see cref="Size"/>)</param>
         * <returns>The height of the terrain at the specified coordinates</returns>
         */
        public float GetHeightAt(float x, float y)
        {
            throw new NotImplementedException();
        }
        
//======== ====== ==== ==
//      OVERRIDES
//======== ====== ==== ==

        /**
         * <summary>Enumerates all cells in the order X then Y </summary>
         */
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