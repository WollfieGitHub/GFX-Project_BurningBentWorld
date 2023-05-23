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

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rZ = 0; rZ < height; rZ++)
                    {
                        InitChunkSeed(x + rX, z + rZ);

                        cells[rX, rZ] = new CellInfo {
                            Land = CoinFlip(true, false, _fractionOfLand)
                        };
                    }
                }

                return cells;
            };
        }
    }
}