using UnityEngine;

namespace TerrainGeneration.Noises
{
    /// <summary>
    /// Smooth the transition between two property steps
    /// </summary>
    public class PropertySmoother
    {
        private GenerationMap<float> _propertyMap;
        
        public PropertySmoother(GenerationMap<float> propertyMap)
        {
            _propertyMap = propertyMap;
        }

        /// <summary>
        /// Gives the interpolation factor between one discrete property steps and the next :
        /// Each property steps is an integer and the <see cref="_propertyMap"/> maps
        /// position to continuous values of this property
        /// </summary>
        /// <param name="x">X Coordinate</param>
        /// <param name="y">Y Coordinate</param>
        /// <remarks>Very useful : https://gamedev.stackexchange.com/questions/194368/2d-side-scroller-game-smooth-noise-transition-between-biomes</remarks>
        /// <returns>The interpolation factor (0 to 1) of the property value</returns>
        public float Apply(float x, float y)
        {
            var propValue = _propertyMap(x, y);

            // Closest distance to an integer is 
            var relativeBiomeValue = Mathf.Abs(propValue - Mathf.RoundToInt(propValue));
            
            // Interpolation factor between two biomes
            return PropertyTransitionFunction(relativeBiomeValue);
        }

        /// <summary>
        /// From the article cited in <see cref="Apply"/>, smoothing function between two biomes
        /// </summary>
        /// <param name="distToBiome">Distance in the current biome : 0 = center of biome, 1 = boundary</param>
        /// <returns>The interpolation factor between 0 and 1 of the current and next biome</returns>
        private float PropertyTransitionFunction(float distToBiome)
        {
            var sin = Mathf.Sin(distToBiome * Mathf.PI);
            return sin * sin;
        }
    }
}