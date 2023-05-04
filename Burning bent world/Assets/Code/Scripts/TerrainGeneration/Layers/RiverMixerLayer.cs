using TerrainGeneration;
using TerrainGeneration.Components;

namespace Code.Scripts.TerrainGeneration.Layers
{
    public class RiverMixerLayer : IMixLayer
    {
        public CellMap Apply(CellMap baseMap, CellMap riverMap)
        {
            return (x, y, width, height) =>
            {
                var resultCells = new CellInfo[width,height];

                var baseCells = baseMap(x, y, width, height);
                var riverCells = riverMap(x, y, width, height);

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rY = 0; rY < height; rY++)
                    {
                        var cell = baseCells[rX, rY];
                        
                        // Only change if land
                        if (baseCells[rX, rY].Land)
                        {
                            // And the river cell is river and not null
                            if (Biome.River.Equals(riverCells[rX, rY].Biome))
                            {
                                // Then put a river
                                cell.Biome = cell.Temperature < Biome.WaterFreezingTemperature 
                                    ? Biome.FrozenRiver
                                    : Biome.River;
                            }
                        }

                        resultCells[rX, rY] = cell;
                    }
                }

                return resultCells;
            };
        }
    }
}