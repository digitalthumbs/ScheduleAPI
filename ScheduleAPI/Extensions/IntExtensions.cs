using System;

namespace ScheduleAPI.Extensions
{
    public static class IntExtensions
    {
        public static int RotateLeft(this int value, byte totalBits, byte numOfBits)
        {
            var rotateBy = totalBits - numOfBits;
            var mask = (int)Math.Pow(2, totalBits) - 1;
            return ((value << numOfBits) | (value >> rotateBy)) & mask;
        }

        public static int RotateRight(this int value, byte totalBits, byte numOfBits)
        {
            var rotateBy = totalBits - numOfBits;
            var mask = (int)Math.Pow(2, totalBits) - 1;
            return ((value >> numOfBits) | (value << rotateBy)) & mask;
        }

    }
}
