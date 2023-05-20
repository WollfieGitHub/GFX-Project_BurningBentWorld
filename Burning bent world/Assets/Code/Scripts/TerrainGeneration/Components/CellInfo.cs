namespace Code.Scripts.TerrainGeneration.Components
{
    public struct CellInfo
    {
//======== ====== ==== ==
//      HEIGHT
//======== ====== ==== ==
        
        /** The height in number of cells of this cell */
        public float Height { get; set; }
        
//======== ====== ==== ==
//      BIOME
//======== ====== ==== ==

        /** True if this cell represents a cell of land */
        public bool Land
        {
            get => !Ocean;
            set => Ocean = !value;
        }

        /** True if this cell represents a cell of ocean */
        public bool Ocean
        {
            get => BiomeAttribute.IsOcean;
            set => BiomeAttribute.IsOcean = value;
        }

        public BiomeAttribute BiomeAttribute;

        
        /** The average annual temperature in degrees celsius of this cell */
        public float Temperature;
        /** The average annual precipitations in centimeters of this cell */
        public float Precipitation;
        
        /** Wrapper for <see cref="BiomeAttribute"/>'s Biome */
        public Biome Biome
        {
            get => BiomeAttribute.Biome;
            set => BiomeAttribute.Biome = value;
        }

        /** 0 Indicates no river */
        public int RiverIndicator;

        public float Dist;

        /** True if this cell has the same terrain attributes than the other cell */
        public bool HasSameParametersAs(CellInfo other) => BiomeAttribute.Equals(other.BiomeAttribute);
    }
}