using System;

namespace UstaPlatform.Infrastructure
{
    public static class DistanceHelper
    {
        public static double CalculateDistance(string address1, string address2)
        {
            Random rand = new Random();
            double baseDistance = (address1.Length + address2.Length) * 0.5;

            return baseDistance + rand.NextDouble() * (15 - 5) + 5;
        }
    }
}