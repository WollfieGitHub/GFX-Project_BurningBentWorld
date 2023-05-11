using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TerrainGeneration.Components;
using TerrainGeneration.Generators;

namespace TerrainGeneration
{
    public class GenerationStack
    {
        protected readonly List<ITransformLayer> Layers;
        public readonly string Name;

        public GenerationStack(string name, IEnumerable<ITransformLayer> layers)
        {
            Name = name;
            Layers = layers.ToList();
        }

        public CellMap Apply(CellMap initialMap, IProgress<TerrainGenerator.ProgressStatus> progressTracker = null)
        {
            // Prevent costly comparison on each iteration
            var progressTrackerNotNull = progressTracker != null;
            
            var map = initialMap;

            for (int i = 0; i < Layers.Count; i++)
            {
                var layer = Layers[i];
                map = layer.Apply(map);
                // Update progress for stack
                if (progressTrackerNotNull) { progressTracker.Report(new TerrainGenerator.ProgressStatus
                {
                    Progress = (float)(i+1) / Layers.Count,
                    StackName = Name
                }); }
            }

            return map;
        }
    }
}