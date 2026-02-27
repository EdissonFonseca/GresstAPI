public class WastePricing {
    public PricingType Type      { get; set; }
    public decimal     Price     { get; set; }
    public DateTime    ValidFrom { get; set; }
    public DateTime?   ValidTo   { get; set; }
}
