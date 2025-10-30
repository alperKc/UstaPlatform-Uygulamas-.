using System;

namespace UstaPlatform.Domain
{
    public class Master
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Name { get; init; } = string.Empty;
        public string Address { get; init; } = "Merkez Depo";
        public string Expertise { get; init; } = string.Empty;
        public int Rating { get; set; } = 5;
        public int CurrentLoad { get; set; } = 0;
    }
}