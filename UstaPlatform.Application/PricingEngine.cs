using System;
using System.Collections.Generic;
using System.Linq;
using UstaPlatform.Domain;

namespace UstaPlatform.Application
{
    public class PricingEngine
    {
        private readonly List<IPricingRule> _rules;

        public PricingEngine(IEnumerable<IPricingRule> rules)
        {
            _rules = rules?.ToList() ?? new List<IPricingRule>();
        }

        public (decimal finalPrice, List<string> appliedRules) CalculatePrice(decimal basePrice, WorkOrder workOrder)
        {
            decimal finalPrice = basePrice;
            var appliedRules = new List<string>();

            Console.WriteLine($"  -> Baz Fiyat: {basePrice:C}");

            foreach (var rule in _rules)
            {
                decimal newPrice = rule.Apply(finalPrice, workOrder);

                if (newPrice != finalPrice)
                {
                    appliedRules.Add(rule.RuleName);
                    finalPrice = newPrice;
                }
            }

            return (finalPrice, appliedRules);
        }
    }
}