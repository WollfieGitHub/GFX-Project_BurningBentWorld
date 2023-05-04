using System.Collections;
using System.Collections.Generic;

namespace TerrainGeneration.Components
{
    public class Terrain : IEnumerable<Chunk>
    {
//======== ====== ==== ==
//      PROPERTIES
//======== ====== ==== ==

        /** Min Height the Terrain can have */
        public const int MaxHeight = 256;
        /** Max Height the Terrain can have */
        public const int MinHeight = -128;
        /** Height of the sea */
        public const int SeaLevel = 0;
        
        /** Min Height the Terrain can have */
        public const float MaxWaterDepth = 0;
        /** Max Height the Terrain can have */
        public const float MinWaterDepth = 0;

        /** All chunks in this terrain */
        private readonly Chunk[,] _chunks;

        public Terrain(Chunk[,] chunks) { _chunks = chunks; }

        /** Width (in number of cells) of the terrain */
        public int Width => _chunks.GetLength(0) * Chunk.Size;
        /** Height (in number of cells) of the terrain */
        public int Height => _chunks.GetLength(1) * Chunk.Size;

//======== ====== ==== ==
//      METHODS
//======== ====== ==== ==

        /**
         * <param name="x">The x coordinate in the terrain's referential</param>
         * <param name="y">The y coordinate in the terrain's referential</param>
         * <returns>The <see cref="Cell"/> at coordinates x,y</returns>
         */
        public Cell GetCellAt(int x, int y)
        {
            var xChunk = x / Chunk.Size;
            var yChunk = y / Chunk.Size;

            var x0 = x - xChunk * Chunk.Size;
            var y0 = y - yChunk * Chunk.Size;

            return _chunks[xChunk, yChunk].GetCellAt(x0, y0);
        }

        /// <param name="x">X index of the chunk or the cell the returned chunk owns</param>
        /// <param name="y">Y index of the chunk or the cell the returned chunk owns</param>
        /// <param name="cellCoordinates">Whether the x and y parameter indicates cell coordinates (if set to
        /// true) or chunk coordinates (if set to false)</param>
        /// <returns>The chunk with index (x, y) in the terrain</returns>
        public Chunk GetChunkAt(int x, int y, bool cellCoordinates = false)
        {
            if (!cellCoordinates) { return _chunks[x, y]; }
            
            var xChunk = x / Chunk.Size;
            var yChunk = y / Chunk.Size;

            return _chunks[xChunk, yChunk];
        }

//======== ====== ==== ==
//      OVERRIDES
//======== ====== ==== ==

        public IEnumerator<Chunk> GetEnumerator()
        {
            for (int x = 0; x < _chunks.GetLength(0); x++)
            {
                for (int y = 0; y < _chunks.GetLength(1); y++)
                {
                    yield return _chunks[x, y];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}