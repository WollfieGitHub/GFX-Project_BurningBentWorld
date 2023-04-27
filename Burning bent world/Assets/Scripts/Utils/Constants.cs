using System;

namespace Utils
{
    public static class Constants
    {
        private const int Seed = 1278364512;
        
        public static readonly Random URandom = new (Seed);

    }
}