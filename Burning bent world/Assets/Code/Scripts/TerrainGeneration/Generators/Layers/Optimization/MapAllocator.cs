using System.Collections.Concurrent;
using System.Collections.Generic;
using Code.Scripts.TerrainGeneration.Components;
using Code.Scripts.Utils;

namespace Code.Scripts.TerrainGeneration.Generators.Layers.Optimization
{
    public static class MapAllocator
    {
        /// <summary>
        /// The difference in ID between an array that is allocated and
        /// an array which can be reused for something else
        /// </summary>
        private const int ArrayLifespan = 2;

        private static readonly ConcurrentDictionary<(int, int), List<AllocatedArray>> AllocatedArraysByDimension = new();
        
        
        /// <summary>
        /// Return a new 2 dimensional array of <see cref="CellInfo"/> which is either
        /// allocated (if one already existed with the same dimensions but was
        /// no longer being used)
        /// </summary>
        /// <param name="width">The width of the desired array</param>
        /// <param name="height">The height of the desired array</param>
        /// <returns>The new array (either allocated or reused from unused arrays)</returns>
        public static Efficient2DArray<CellInfo> GetNew(int width, int height)
        {
            return new Efficient2DArray<CellInfo>(width,height);

            Efficient2DArray<CellInfo> result;
            
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
                result = new Efficient2DArray<CellInfo>(width, height);
                array = new AllocatedArray { Array = result, Lifespan = ArrayLifespan};
                // And add it to the list of arrays
                AllocatedArraysByDimension[(width, height)].Add(array);

            // Otherwise extract the result from the array found
            } else { result = array.Array; }
            
        }

        private class AllocatedArray
        {
            public Efficient2DArray<CellInfo> Array;
            
            /// <summary>
            /// Index goes from <see cref="MapAllocator.ArrayLifespan"/> to
            /// negatives. If negative, it means that the array is no longer being used
            /// </summary>
            public int Lifespan;
        }
    }
}