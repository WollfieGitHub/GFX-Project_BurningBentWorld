using Code.Scripts.TerrainGeneration.Components;
using TerrainGeneration;

namespace Code.Scripts.TerrainGeneration.Generators.Layers
{
    public class AddBiomesLayer : TransformLayer
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
                        
                        var cell = cells[rX, rZ];

                        var biome = Biome.From(
                            cell.Temperature,
                            cell.Precipitation
                        );

                        cells[rX, rZ].Biome = biome;
                    }
                }

                return cells;
            };
        }

        public AddBiomesLayer(long baseSeed) : base(baseSeed) { }
    }
}