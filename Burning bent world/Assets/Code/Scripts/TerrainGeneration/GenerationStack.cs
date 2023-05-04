using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TerrainGeneration.Components;

namespace TerrainGeneration
{
    public class GenerationStack
    {
        protected readonly List<ITransformLayer> Layers;

        public GenerationStack(IEnumerable<ITransformLayer> layers)
        {
            Layers = layers.ToList();
        }

        public CellMap Apply(CellMap initialMap)
        {
            var map = initialMap;
            
            foreach (var layer in Layers)
            {
                map = layer.Apply(map);
            }

            return map;
        }
    }
}