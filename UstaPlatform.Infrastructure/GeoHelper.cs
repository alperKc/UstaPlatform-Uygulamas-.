using System;

namespace UstaPlatform.Infrastructure
{
    public static class GeoHelper
    {
        public static (int X, int Y) GetCoordinates(string address)
        {
            Console.WriteLine($"[GEO] '{address}' için koordinatlar hesaplandı.");
            return (address.Length * 10, address.Length * 5);
        }
    }
}