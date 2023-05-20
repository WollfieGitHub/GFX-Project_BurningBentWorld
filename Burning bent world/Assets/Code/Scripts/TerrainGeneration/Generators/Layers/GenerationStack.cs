using System;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.TerrainGeneration;
using JetBrains.Annotations;
using TerrainGeneration.Components;

namespace TerrainGeneration
{
    public class GenerationStack
    {
        protected readonly List<TransformLayer> Layers;
        public readonly string Name;

        public GenerationStack(string name, IEnumerable<TransformLayer> layers, GenerationStack parent)
            : this(name, layers, parent.GetLast()) { }
        
        public GenerationStack(string name, IEnumerable<TransformLayer> layers, TransformLayer parent)
        {
            Name = name;
            Layers = layers.ToList();
            // Set parent of all layers
            Layers[0].SetParent(parent);
            for (var i = 1; i < Layers.Count; i++) { Layers[i].SetParent(Layers[i-1]); }
        }

        public TransformLayer GetLast() => Layers.Last();

        public CellMap Apply(int worldSeed)
        {
            var lastLayer = GetLast();
            lastLayer.InitWorldGenSeed(worldSeed);
            return lastLayer.Apply();
        }
    }
}