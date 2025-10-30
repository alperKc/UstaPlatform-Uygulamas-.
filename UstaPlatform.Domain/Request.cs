using System;

namespace UstaPlatform.Domain
{
    public class Request
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public Citizen Requester { get; init; } = new Citizen();
        public bool IsUrgent { get; init; } = false;
        public bool IsWeekend { get; init; } = false;
        public string RequiredExpertise { get; init; } = string.Empty;
        public DateTime RequestTime { get; init; } = DateTime.Now;
    }
}