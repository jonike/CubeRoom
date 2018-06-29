using System.Collections;
using System.Collections.Generic;

namespace Sorumi.Util
{
    public static class Math
    {
        public static int mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        public static int mod(float x, int m)
        {
            return (int)(x % m + m) % m;
        }
    }
}