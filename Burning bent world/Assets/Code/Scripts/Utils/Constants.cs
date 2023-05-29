using System;
using Random = System.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utils
{
    public static class Constants
    {
     
//======== ====== ==== ==
//      MODIFY SEED HERE
//======== ====== ==== ==

        private const bool UseRandomSeed = true;
        private const int CustomSeed = 1159795043;

//======== ====== ==== ==
//      END OF SEED MODIFICATION
//======== ====== ==== ==

        public static int Seed;
        public static Random URandom;

        static Constants()
        {
            InitSeed();
#if UNITY_EDITOR
            InitCallback();
#endif
        }
        
#if UNITY_EDITOR
        private static void InitCallback()
        {
            EditorApplication.playModeStateChanged += mode =>
            {
                if (mode == PlayModeStateChange.EnteredPlayMode) { InitSeed(); }
            };
        }
#endif 

        private static void InitSeed()
        {
            // Don't pay attention to this hehe
            // ReSharper disable once HeuristicUnreachableCode
#pragma warning disable CS0162
            Seed = UseRandomSeed ? (int)DateTime.Now.Ticks : CustomSeed;
#pragma warning restore CS0162
            URandom = new(Seed);
        }

//======== ====== ==== ==
//      READABILITY
//======== ====== ==== ==

        public const int NbSidesInSquare = 4;

        public const int NbChunkNeighbours = 4;

    }
}