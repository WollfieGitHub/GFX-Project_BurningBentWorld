using Code.Scripts.TerrainGeneration.Components;

namespace Code.Scripts.TerrainGeneration.Generators.Layers.Smoothing
{
    public interface IVoxelSmoother
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="globX"></param>
        /// <param name="globZ"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="cells"></param>
        /// <returns></returns>
        float GetBiomeHeightAt(
            int globX, int globZ, int x, int z, int width, int height, CellInfo[,] cells
        );

        int NecessaryNeighboursCount { get; }
    }
}