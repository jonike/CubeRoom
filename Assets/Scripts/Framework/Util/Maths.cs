using System.Collections;
using System.Collections.Generic;

namespace Sorumi.Util
{
    public static class Maths
    {
        public static int mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        public static float mod(float x, int m)
        {
            return (x % m + m) % m;
        }


        public static bool inRange(float x, float a, float b)
        {
            return x >= a && x <= b;
        }

        public static bool outRange(float x, float a, float b)
        {
            return x < a || x > b;
        }
    }
}