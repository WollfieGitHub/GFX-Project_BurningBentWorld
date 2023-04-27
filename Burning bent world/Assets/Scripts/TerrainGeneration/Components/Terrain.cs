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

        /** All chunks in this terrain */
        private Chunk[,] _chunks;

        public Terrain(Chunk[,] chunks)
        {
            _chunks = chunks;
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