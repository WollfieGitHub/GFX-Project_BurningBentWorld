using UnityEngine;

namespace TerrainGeneration
{
    public interface ITransformLayer 
    {
        GenerationMap<CellInfo> Apply(GenerationMap<CellInfo> inputMap);
    }
}