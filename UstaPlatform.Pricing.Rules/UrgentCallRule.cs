using UstaPlatform.Domain;

namespace UstaPlatform.Pricing.Rules
{
    public class UrgentCallRule : IPricingRule
    {
        public string RuleName => "Acil Çağrı Ücreti Kuralı";

        public decimal Apply(decimal currentPrice, WorkOrder workOrder)
        {
            if (workOrder.Request.IsUrgent)
            {
                return currentPrice * 1.30m;
            }
            return currentPrice;
        }
    }
}