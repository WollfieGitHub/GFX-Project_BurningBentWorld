using System;
using System.IO;
using UnityEngine;
using Random = System.Random;

namespace Utils
{
    public static class Constants
    {
//======== ====== ==== ==
//      MODIFY SEED HERE
//======== ====== ==== ==

        private const bool UseRandomSeed = true;
        private const int CustomSeed = 1159795043;

        public static int Seed;
        
//======== ====== ==== ==
//      END OF SEED MODIFICATION
//======== ====== ==== ==
        
        public static readonly Random URandom;

        static Constants()
        {
            // Don't pay attention to this hehe
            // ReSharper disable once HeuristicUnreachableCode
#pragma warning disable CS0162
            Seed = UseRandomSeed ?  (int)DateTime.Now.Ticks : CustomSeed;
#pragma warning restore CS0162
            URandom = new(Seed);
            Debug.Log($"Starting with seed [{Seed}]");
        }

//======== ====== ==== ==
//      READABILITY
//======== ====== ==== ==

        public const int NbSidesInSquare = 4;

    }
}