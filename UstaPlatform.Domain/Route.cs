using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UstaPlatform.Domain
{
    public class Route : IEnumerable<(int X, int Y)>
    {
        private readonly List<(int X, int Y)> _stops = new();
        public void Add(int x, int y)
        {
            _stops.Add((x, y));
        }
        public IEnumerator<(int X, int Y)> GetEnumerator() => _stops.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() =>
            $"Rota: {_stops.Count} durak içeriyor (Başlangıç: {_stops.FirstOrDefault()})";
    }
}