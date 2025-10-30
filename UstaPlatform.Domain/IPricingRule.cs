namespace UstaPlatform.Domain
{
    public interface IPricingRule
    {
        string RuleName { get; }
        decimal Apply(decimal currentPrice, WorkOrder workOrder);
    }
}
