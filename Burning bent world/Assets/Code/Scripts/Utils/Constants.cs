using System;
using System.IO;
using UnityEditor;
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

//======== ====== ==== ==
//      END OF SEED MODIFICATION
//======== ====== ==== ==

        public static int Seed;
        public static Random URandom;

        static Constants()
        {
            InitSeed();
            EditorApplication.playModeStateChanged += mode =>
            {
                if (mode == PlayModeStateChange.EnteredPlayMode) { InitSeed(); }
            };
        }

        private static void InitSeed()
        {
            // Don't pay attention to this hehe
            // ReSharper disable once HeuristicUnreachableCode
            Debug.Log(DateTime.Now);
#pragma warning disable CS0162
            Seed = UseRandomSeed ? (int)DateTime.Now.Ticks : CustomSeed;
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