using System;
namespace ObsidianMobile.Core.Utils
{
    public static class RandomGenerator
    {
        static readonly Random Rnd = new Random();

        public static int GenerateId()
        {
            return Rnd.Next(2,int.MaxValue);
        }

        public static int Generate(int minValue, int maxValue)
        {
            return Rnd.Next(minValue, maxValue);
        }
    }
}
