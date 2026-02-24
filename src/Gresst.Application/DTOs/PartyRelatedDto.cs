namespace Gresst.Application.DTOs;

public class PartyRelatedDto: PartyDTO
{
    public List<string>? Relations { get; set; } = new List<string>();
}