using Code.Scripts.TerrainGeneration.Components;
using TerrainGeneration;
using Utils;

namespace Code.Scripts.TerrainGeneration.Generators.Layers
{
    public class RiverInitLayer : TransformLayer
    {
        public override CellMap Apply()
        {
            return (x, z, width, height) =>
            {
                var cells = ParentMap(x, z, width, height);
                
                for (var rX = 0; rX < width; rX++)
                {
                    for (var rZ = 0; rZ < height; rZ++)
                    {
                        InitChunkSeed(x + rX, z + rZ);
                        
                        var cell = new CellInfo();

                        if (cells[rX, rZ].Land)
                        {
                            cell.RiverIndicator = NextInt(299999) + 2;
                        }
                        else
                        {
                            cell.RiverIndicator = 0;
                        }

                        cells[rX, rZ] = cell;
                    }
                }

                return cells;
            };
        }

        public RiverInitLayer(long baseSeed) : base(baseSeed) { }
    }
}