using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Code.Scripts.TerrainGeneration.Layers;
using Code.Scripts.TerrainGeneration;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

namespace TerrainGeneration
{
    /// <summary>
    /// A layer which modifies a map and return as output the modified map
    /// </summary>
    public abstract class TransformLayer
    {
        private readonly long _baseSeed;
        private long _worldGenSeed;
        private long _chunkSeed;

        protected Random _worldGenRandom;

        protected TransformLayer Parent;

        protected CellMap ParentMap => Parent.Apply();

        public void SetParent(TransformLayer transformLayer) => Parent = transformLayer;
        
        protected TransformLayer(long baseSeed)
        {
            _baseSeed = baseSeed;
            _baseSeed *= _baseSeed * 6364136223846793005L + 1442695040888963407L;
            _baseSeed += baseSeed;
            _baseSeed *= _baseSeed * 6364136223846793005L + 1442695040888963407L;
            _baseSeed += baseSeed;
            _baseSeed *= _baseSeed * 6364136223846793005L + 1442695040888963407L;
            _baseSeed += baseSeed;

            _worldGenSeed = 0;
            _worldGenRandom = new Random((int)_worldGenSeed);
            _chunkSeed = 0;
        }

        public virtual void InitWorldGenSeed(long seed)
        {
            _worldGenSeed = seed;

            Parent?.InitWorldGenSeed(seed);

            _worldGenSeed *= _worldGenSeed * 6364136223846793005L + 1442695040888963407L;
            _worldGenSeed += _baseSeed;
            _worldGenSeed *= _worldGenSeed * 6364136223846793005L + 1442695040888963407L;
            _worldGenSeed += _baseSeed;
            _worldGenSeed *= _worldGenSeed * 6364136223846793005L + 1442695040888963407L;
            _worldGenSeed += _baseSeed;
            
            _worldGenRandom = new Random((int)_worldGenSeed);
        }

        private bool _chunkSeedInitialized;
        
        protected void InitChunkSeed(long x, long z)
        {
            if (_worldGenSeed == 0)
            {
                Task.Run(() => Debug.Log("PPRRIINNTTT !"));
            }

            _chunkSeedInitialized = true;
            
            _chunkSeed = _worldGenSeed;
            _chunkSeed *= _chunkSeed * 6364136223846793005L + 1442695040888963407L;
            _chunkSeed += x;
            _chunkSeed *= _chunkSeed * 6364136223846793005L + 1442695040888963407L;
            _chunkSeed += z;
            _chunkSeed *= _chunkSeed * 6364136223846793005L + 1442695040888963407L;
            _chunkSeed += x;
            _chunkSeed *= _chunkSeed * 6364136223846793005L + 1442695040888963407L;
            _chunkSeed += z;
        }

        public abstract CellMap Apply();

        protected int NextInt(int max)
        {
            if (!_chunkSeedInitialized)
            {
                throw new InvalidOperationException("Chunk Seed must be initialized"); 
            }
            
            var i = (int)((_chunkSeed >> 24) % max);

            if (i < 0) { i += max; }

            _chunkSeed *= _chunkSeed * 6364136223846793005L + 1442695040888963407L;
            _chunkSeed += _worldGenSeed;
            return i;
        }
        
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
            OneIn(Mathf.RoundToInt(1 / headOdds)) ? head : tail;

        protected virtual T SelectRandom<T>(T[] collection) =>
            collection[NextInt(collection.Length)];

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