public class CertificateEvent : DomainEvent
{
    public CertificateStatus FromStatus { get; set; }
    public CertificateStatus ToStatus { get; set; }
    public string? CertificateNumber { get; set; }
    public string? DocumentUrl { get; set; }
}