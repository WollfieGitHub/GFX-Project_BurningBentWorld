using Code.Scripts.TerrainGeneration.Components;
using Code.Scripts.TerrainGeneration.Layers;
using Code.Scripts.TerrainGeneration.Layers.Optimization;

namespace Code.Scripts.TerrainGeneration.Generators.Layers.Mixers
{
    public class RiverMixerLayer : MixLayer
    {
        public override CellMap Apply()
        {
            return (x, z, width, height) =>
            {
                var resultCells = MapAllocator.GetNew(width, height);

                var baseCells = ParentMap(x, z, width, height);
                var riverCells = FilterMap(x, z, width, height);

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rZ = 0; rZ < height; rZ++)
                    {
                        var cell = baseCells[rX, rZ];
                        
                        // Only change if land
                        if (baseCells[rX, rZ].Land)
                        {
                            // And the river cell is river and not null
                            if (Biome.River.Equals(riverCells[rX, rZ].Biome))
                            {
                                // Then put a river
                                cell.Biome = cell.Temperature < Biome.WaterFreezingTemperature 
                                    ? Biome.FrozenRiver
                                    : Biome.River;
                            }
                        }

                        resultCells[rX, rZ] = cell;
                    }
                }

                return resultCells;
            };
        }

        public RiverMixerLayer(long baseSeed) : base(baseSeed) { }
    }
}