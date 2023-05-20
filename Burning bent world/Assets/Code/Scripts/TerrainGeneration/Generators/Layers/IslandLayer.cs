using Code.Scripts.TerrainGeneration;
using Code.Scripts.TerrainGeneration.Components;
using TerrainGeneration;

namespace Code.Scripts.TerrainGeneration.Layers
{
    public class IslandLayer : TransformLayer
    {
        private readonly float _fractionOfLand;
        
        public IslandLayer(float fractionOfLand, long baseSeed) : base(baseSeed)
        {
            _fractionOfLand = fractionOfLand;
        }

        /// <summary>
        /// Creates a white noise map with fraction of ones being the specified value
        /// </summary>
        /// <returns>The white noise map</returns>
        public override CellMap Apply()
        {
            return (x, z, width, height) =>
            {
                var cells = new CellInfo[width, height];

                for (var px = 0; px < width; px++)
                {
                    for (var pz = 0; pz < height; pz++)
                    {
                        InitChunkSeed(x + px, z + pz);
                        cells[px, pz] = new CellInfo
                        {
                            Land = CoinFlip(true, false, _fractionOfLand)
                        };
                    }
                }
                
                return cells;
            };
        }
    }
}