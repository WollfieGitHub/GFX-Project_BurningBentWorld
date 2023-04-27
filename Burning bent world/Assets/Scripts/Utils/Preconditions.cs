using System;

namespace Utils
{
    public class Preconditions
    {
        public static void CheckArgument(bool shouldBeTrue)
        {
            if (!shouldBeTrue) { throw new ArgumentException(); }
        }
    }
}