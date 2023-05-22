using Code.Scripts.TerrainGeneration.Components;

namespace Code.Scripts.TerrainGeneration.Generators.Layers.Smoothing
{
    public interface IVoxelSmoother
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="cells"></param>
        /// <returns></returns>
        float GetBiomeHeightAt(
            int x, int y, int width, int height, CellInfo[,] cells
        );
    }
}