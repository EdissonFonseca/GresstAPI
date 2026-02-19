public enum MeasurementUnit
{
    Kg, Ton,        // Weight
    L, M3,          // Volume
    Units           // Count
}
public class Measurement
{
    public decimal Amount { get; set; }
    public MeasurementUnit Unit { get; set; }

    public bool IsWeight => Unit is MeasurementUnit.Kg or MeasurementUnit.Ton;
    public bool IsVolume => Unit is MeasurementUnit.L or MeasurementUnit.M3;
    public bool IsCount => Unit == MeasurementUnit.Units;
    
    public decimal ToKg() => Unit switch
    {
        MeasurementUnit.Kg  => Amount,
        MeasurementUnit.Ton => Amount * 1000,
        _ => throw new DomainException("Cannot convert to Kg")
    };
}
