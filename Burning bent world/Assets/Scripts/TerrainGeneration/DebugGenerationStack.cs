using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace TerrainGeneration
{
    public class DebugGenerationStack : GenerationStack
    {
        public int StoppingLayerIdx = -1;
        
        public DebugGenerationStack(IEnumerable<ITransformLayer> layers) : base(layers) { }

        public override GenerationMap<CellInfo> Apply(GenerationMap<CellInfo> initialMap)
        {
            int stoppingIndex = StoppingLayerIdx;
            // Special value to say to keep going until the end
            if (StoppingLayerIdx == -1) { stoppingIndex = Layers.Count; }
            
            var map = initialMap;
            
            foreach (var layer in Layers)
            {
                map = layer.Apply(map);
                stoppingIndex--;

                // When we reach the desired layer, stop
                if (stoppingIndex == 0) { break; }
            }

            return map;
        }
    }
}