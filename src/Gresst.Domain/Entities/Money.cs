public class Money
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "COP"; // default Colombia

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException("Cannot add amounts in different currencies");
        return new Money { Amount = Amount + other.Amount, Currency = Currency };
    }

    public Money Multiply(decimal factor) =>
        new Money { Amount = Amount * factor, Currency = Currency };
}