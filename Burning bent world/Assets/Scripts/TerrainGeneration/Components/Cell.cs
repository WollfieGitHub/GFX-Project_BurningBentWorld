using UnityEngine;

namespace TerrainGeneration.Components
{
    /**
     * 2D Cell in the terrain, its underlay/overlay field represent anything we could
     * want to display under/on the cell's level
     *
     * TODO List:
     * <ul style="padding: 10px">
     *  <li>Rivers which go from sea to inside the land</li>
     *  <li></li>
     *  <li></li>
     * </ul>
     */
    public class Cell
    {
        public readonly float Height;

        public Cell(float height)
        {
            Height = height;
        }

        private MonoBehaviour _overlay;
        private MonoBehaviour _underlay;
        
        public Vector2 Position { get; }

        private Chunk _parentChunk;
    }
}