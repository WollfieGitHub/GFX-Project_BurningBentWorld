using Code.Scripts.TerrainGeneration.Layers;
using Code.Scripts.TerrainGeneration;
using UnityEngine;

namespace TerrainGeneration
{
    /// <summary>
    /// A layer which modifies a map and return as output the modified map
    /// </summary>
    public abstract class TransformLayer
    {
        private static long Convolute(long x, long a) =>
            x * (x * 6364136223846793005L + 1442695040888963407L) + a;
        
        private readonly long _baseSeed;
        private long _worldGenSeed;
        private long _chunkSeed;

        protected TransformLayer Parent;

        protected CellMap ParentMap => Parent.Apply();

        public void SetParent(TransformLayer transformLayer) => Parent = transformLayer;
        
        protected TransformLayer(long baseSeed)
        {
            _baseSeed = baseSeed;
            _baseSeed = Convolute(_baseSeed, _baseSeed);
            _baseSeed = Convolute(_baseSeed, _baseSeed);
            _baseSeed = Convolute(_baseSeed, _baseSeed);

            _worldGenSeed = 0;
            _chunkSeed = 0;
        }

        public virtual void InitWorldGenSeed(long seed)
        {
            _worldGenSeed = seed;

            Parent?.InitWorldGenSeed(seed);

            _worldGenSeed = Convolute(_worldGenSeed, _baseSeed);
            _worldGenSeed = Convolute(_worldGenSeed, _baseSeed);
            _worldGenSeed = Convolute(_worldGenSeed, _baseSeed);
        }

        protected void InitChunkSeed(long x, long z)
        {
            _chunkSeed = _worldGenSeed;
            _chunkSeed = Convolute(_chunkSeed, x);
            _chunkSeed = Convolute(_chunkSeed, z);
            _chunkSeed = Convolute(_chunkSeed, x);
            _chunkSeed = Convolute(_chunkSeed, z);
        }

        public abstract CellMap Apply();

        protected int NextInt(int max)
        {
            var i = _chunkSeed >> 24 % max;

            if (i < 0) { i += max; }

            _chunkSeed = Convolute(_chunkSeed, _worldGenSeed);
            return (int)i;
        }

        private const int RandomResolution = 1000;

        /// <summary>
        /// Returns true with a change of one in <see cref="odds"/>
        /// </summary>
        /// <param name="odds">The inverse of the odds of returning true</param>
        /// <returns>True if random drawn true with one in <see cref="odds"/> chances, false otherwise</returns>
        protected bool OneIn(int odds) => NextInt(odds) == 0;

        /// <summary>
        /// Draws a random result form the specified odds
        /// </summary>
        /// <param name="head">Object returned in case of Head</param>
        /// <param name="tail">Object returned in case of Tail</param>
        /// <param name="headOdds">Odds of drawing Head</param>
        /// <typeparam name="T">Type of the object to return</typeparam>
        /// <returns>The result of the random event</returns>
        protected T CoinFlip<T>(T head, T tail, float headOdds) => 
            NextInt(RandomResolution) <= headOdds * RandomResolution ? head : tail;

        /// <summary>
        /// Draws a random result from a uniform distribution (50/50)
        /// </summary>
        /// <param name="head">Object returned in case of Head</param>
        /// <param name="tail">Object returned in case of Tail</param>
        /// <typeparam name="T">Type of the object to return</typeparam>
        /// <returns>The result of the random event</returns>
        protected T CoinFlip<T>(T head, T tail) => CoinFlip(head, tail, 0.5f);


    }
}