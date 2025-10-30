using UstaPlatform.Domain;

namespace UstaPlatform.Pricing.Rules
{
    public class WeekendSurchargeRule : IPricingRule
    {
        public string RuleName => "Haftasonu Ek Ücreti Kuralı";

        public decimal Apply(decimal currentPrice, WorkOrder workOrder)
        {
            if (workOrder.Request.IsWeekend)
            {
                return currentPrice * 1.15m;
            }
            return currentPrice;
        }
    }
}
