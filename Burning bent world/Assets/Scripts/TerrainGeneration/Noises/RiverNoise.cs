namespace TerrainGeneration.Noises
{
    public class RiverNoise
    {
        public const float Threshold = 0.1f;
        
        /// <summary>
        /// Generates a height value between 0 and 1 which is a height offset into the ground compared to
        /// original terrain level. A height offset greater than <see cref="Threshold"/> means there is water
        /// for a depth of <code>value - Threshold</code> into the ground.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>Height offset</returns>
        public float Apply(float x, float y)
        {
            // TODO Implement
            return 0.0f;
        }
    }
}