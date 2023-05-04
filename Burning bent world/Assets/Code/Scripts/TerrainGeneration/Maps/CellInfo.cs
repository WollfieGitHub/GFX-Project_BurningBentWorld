using TerrainGeneration.Components;

namespace TerrainGeneration
{
    public struct CellInfo
    {
        /** True if this cell represents a cell of land */
        public bool Land;
        /** True if this cell represents a cell of ocean */
        public bool Ocean => !Land;
        
        /** The average annual temperature in degrees celsius of this cell */
        public float Temperature { get; set; }
        /** The average annual precipitations in centimeters of this cell */
        public float Precipitation { get; set; }

        /** The biome this cell belongs to */
        public Biome Biome { get; set; }

        /** 0 Indicates no river */
        public int RiverIndicator { get; set; }
    }
}