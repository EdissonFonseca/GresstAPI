using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Services;

/// <summary>
/// Registers waste generated at a company's facility: creates Residuo + Saldo in one transaction.
/// See docs/waste-generation.md.
/// </summary>
public class WasteGenerationService : IWasteGenerationService
{
    private readonly InfrastructureDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    private const string EstadoResiduoGenerated = "G";
    private const string EstadoSaldoActive = "A";

    public WasteGenerationService(
        InfrastructureDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<RegisteredWasteResultDto> RegisterGeneratedWasteAsync(
        RegisterGeneratedWasteDto dto,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var userId = GetCurrentUserIdAsLong();

        var residuo = new Residuo
        {
            IdMaterial = dto.MaterialId,
            IdPropietario = dto.GeneratorPersonId,
            IdEstado = EstadoResiduoGenerated,
            FechaIngreso = now,
            Descripcion = dto.Description,
            Referencia = dto.Reference,
            FechaCreacion = now,
            IdUsuarioCreacion = userId
        };

        _context.Residuos.Add(residuo);
        await _context.SaveChangesAsync(cancellationToken);

        var saldo = new Saldo
        {
            IdResiduo = residuo.IdResiduo,
            IdDeposito = dto.DepotId,
            IdTratamiento = dto.TreatmentId,
            Cantidad = dto.Quantity,
            Peso = dto.Weight,
            Volumen = dto.Volume,
            IdEstado = EstadoSaldoActive,
            FechaCreacion = now,
            IdUsuarioCreacion = userId
        };

        _context.Saldos.Add(saldo);
        await _context.SaveChangesAsync(cancellationToken);

        return new RegisteredWasteResultDto
        {
            IdResiduo = residuo.IdResiduo,
            IdDeposito = saldo.IdDeposito,
            Cantidad = saldo.Cantidad,
            Peso = saldo.Peso,
            Volumen = saldo.Volumen,
            FechaIngreso = residuo.FechaIngreso ?? now
        };
    }

    private long GetCurrentUserIdAsLong()
    {
        var userId = _currentUserService.GetCurrentUserId();
        return IdConversion.ToLongFromString(userId);
    }
}
