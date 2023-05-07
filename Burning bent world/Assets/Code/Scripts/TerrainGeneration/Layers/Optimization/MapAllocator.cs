using System.Collections.Generic;
using TerrainGeneration;

namespace Code.Scripts.TerrainGeneration.Layers.Optimization
{
    public static class MapAllocator
    {
        /// <summary>
        /// The difference in ID between an array that is allocated and
        /// an array which can be reused for something else
        /// </summary>
        private const int ArrayLifespan = 2;

        /// <summary>
        /// The Ids of the array cannot go pass this size
        /// and will instead wrap around to 0 if this number tries to be
        /// attributed as an ID
        /// </summary>
        private const int IdWrapSize = ArrayLifespan + 10;
        
        private static int CurrentId = 0;

        private static readonly Dictionary<(int, int), List<AllocatedArray>> AllocatedArraysByDimension = new();
        
        
        /// <summary>
        /// Return a new 2 dimensional array of <see cref="CellInfo"/> which is either
        /// allocated (if one already existed with the same dimensions but was
        /// no longer being used)
        /// </summary>
        /// <param name="width">The width of the desired array</param>
        /// <param name="height">The height of the desired array</param>
        /// <returns>The new array (either allocated or reused from unused arrays)</returns>
        public static CellInfo[,] GetNew(int width, int height)
        {
            CellInfo[,] result;
            
            AllocatedArray array = null;
            var arraysExistForDimension = false;
            var arrayFound = false;
            
            if (AllocatedArraysByDimension.TryGetValue((width, height), out var cellArrays))
            {
                arraysExistForDimension = true;
                foreach (var allocatedArray in cellArrays)
                {
                    if (allocatedArray.Lifespan <= 0)
                    {
                        array = allocatedArray;
                        allocatedArray.Lifespan = ArrayLifespan+1;
                        arrayFound = true;
                        break;
                    }
                }
                // Decrease lifespan of all arrays
                cellArrays.ForEach(_ => _.Lifespan--);
            }
            
            // If no arrays were found for the given dimension, allocate a new list of arrays
            if (!arraysExistForDimension) { AllocatedArraysByDimension[(width, height)] = new List<AllocatedArray>(); }

            // If no suitable array is found, allocate a new one
            if (!arrayFound)
            {
                result = new CellInfo[width, height];
                array = new AllocatedArray { Array = result, Lifespan = ArrayLifespan};
                // And add it to the list of arrays
                AllocatedArraysByDimension[(width, height)].Add(array);

            // Otherwise extract the result from the array found
            } else { result = array.Array; }
            
            return result;
        }

        private class AllocatedArray
        {
            public CellInfo[,] Array;
            
            /// <summary>
            /// Index goes from <see cref="MapAllocator.ArrayLifespan"/> to
            /// negatives. If negative, it means that the array is no longer being used
            /// </summary>
            public int Lifespan;
        }
    }
}