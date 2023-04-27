using System;
using System.Collections;

namespace TerrainGeneration.Components
{
    public class Chunk : IEnumerable
    {
        /** Same as in Minecraft */
        public const int Size = 16;

        /**
         * Biome of the chunk
         * A chunk has only one biome but TODO interpolation between neighbouring chunks
         * with different biomes is done to get a smooth appearance transition
         */
        private Biome _biome;

        /** Cells from this chunk */
        private readonly Cell[,] _cells = new Cell[Size, Size];

        /**
         * <summary>Enumerates all cells in the order X then Y </summary>
         */
        public IEnumerator GetEnumerator()
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
    }
}