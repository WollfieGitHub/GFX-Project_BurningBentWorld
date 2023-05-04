using TerrainGeneration;
using Utils;

namespace Code.Scripts.TerrainGeneration.Layers
{
    public class RiverInitLayer : ITransformLayer
    {
        public CellMap Apply(CellMap inputMap)
        {
            return (x, y, width, height) =>
            {
                var parentCells = inputMap(x, y, width, height);

                var resultCells = new CellInfo[width, height];

                for (var rX = 0; rX < width; rX++)
                {
                    for (var rY = 0; rY < height; rY++)
                    {
                        var cell = new CellInfo();

                        if (parentCells[rX, rY].Land)
                        {
                            cell.RiverIndicator = Constants.URandom.Next(299999) + 2;
                        }
                        else
                        {
                            cell.RiverIndicator = 0;
                        }

                        resultCells[rX, rY] = cell;
                    }
                }

                return resultCells;
            };
        }
    }
}