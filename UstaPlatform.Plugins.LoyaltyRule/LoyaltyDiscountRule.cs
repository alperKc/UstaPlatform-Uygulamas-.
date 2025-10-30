using UstaPlatform.Domain;

namespace UstaPlatform.Plugins.LoyaltyRule
{
    public class LoyaltyDiscountRule : IPricingRule
    {
        public string RuleName => "Kademeli Sadakat İndirimi Kuralı";

        public decimal Apply(decimal currentPrice, WorkOrder workOrder)
        {
            switch (workOrder.Request.Requester.Tier)
            {
                case LoyaltyTier.Gold:
                    return currentPrice * 0.90m;

                case LoyaltyTier.Platinum:
                    return currentPrice * 0.80m;

                case LoyaltyTier.Silver:
                    return currentPrice * 0.95m;

                case LoyaltyTier.None:
                default:
                    return currentPrice;
            }
        }
    }
}