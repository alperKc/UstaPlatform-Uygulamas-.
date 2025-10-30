using System;

namespace UstaPlatform.Domain
{
    public class WorkOrder
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public DateTime CreationTime { get; init; } = DateTime.Now;

        public Request Request { get; init; } = new Request();

        public Master AssignedMaster { get; set; } = new Master();
        public decimal BasePrice { get; init; } = 0m;
        public decimal FinalPrice { get; set; } = 0m;

        public (int X, int Y) RouteStopLocation { get; set; }

        public override string ToString() =>
            $"İş Emri ID: {Id.ToString().Substring(0, 8)} | Usta: {AssignedMaster.Name} | Fiyat: {FinalPrice:C}";
    }
}