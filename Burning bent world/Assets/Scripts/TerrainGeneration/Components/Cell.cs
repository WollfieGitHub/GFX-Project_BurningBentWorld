using UnityEngine;

namespace TerrainGeneration.Components
{
    /**
     * 2D Cell in the terrain, its underlay/overlay field represent anything we could
     * want to display under/on the cell's level
     */
    public class Cell
    {
        private MonoBehaviour _overlay;
        private MonoBehaviour _underlay;

        private Chunk _parentChunk;
    }
}