using TerrainGeneration;

namespace Code.Scripts.TerrainGeneration.Layers
{
    /// <summary>
    /// A layer which takes two map as input and return as output a new map using
    /// the inputs' information
    /// </summary>
    public interface IMixLayer
    {
        CellMap Apply(CellMap input1, CellMap input2);
    }
}