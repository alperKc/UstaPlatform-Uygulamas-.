namespace UstaPlatform.Domain
{
    public enum LoyaltyTier
    {
        None,
        Silver,
        Gold,
        Platinum
    }

    public class Citizen
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Name { get; init; } = string.Empty;
        public string Address { get; init; } = string.Empty;

        public LoyaltyTier Tier { get; init; } = LoyaltyTier.None;
    }
}