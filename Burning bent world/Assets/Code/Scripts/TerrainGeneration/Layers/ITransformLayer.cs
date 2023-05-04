using Code.Scripts.TerrainGeneration.Layers;
using UnityEngine;

namespace TerrainGeneration
{
    /// <summary>
    /// A layer which modifies a map and return as output the modified map
    /// </summary>
    public interface ITransformLayer
    {
        CellMap Apply(CellMap inputMap);
    }
}