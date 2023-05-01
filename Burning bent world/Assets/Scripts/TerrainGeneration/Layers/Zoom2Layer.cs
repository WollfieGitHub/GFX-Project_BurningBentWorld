using System.Xml;
using TerrainGeneration.Noises;
using UnityEngine;

namespace TerrainGeneration.Layers
{
    public class Zoom2Layer : ITransformLayer
    {
        private const float ScaleFactor = 2f;
        
        public GenerationMap<CellInfo> Apply(GenerationMap<CellInfo> inputMap) =>
            (x, y) => inputMap(x / ScaleFactor, y / ScaleFactor);
    }
}