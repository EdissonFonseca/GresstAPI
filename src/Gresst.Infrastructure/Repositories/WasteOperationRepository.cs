using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Gresst.Infrastructure.Repositories;

public class WasteOperationRepository : IWasteOperationRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly RouteProcessDbContext _context;

    public WasteOperationRepository(RouteProcessDbContext context)
        => _context = context;

    public async Task AddAsync(WasteOperation operation, CancellationToken ct = default)
    {
        var dataJson = JsonSerializer.Serialize(operation.Data, operation.Data.GetType(), JsonOptions);
        var entity = new WasteOperationEntity
        {
            Id = operation.Id,
            Type = operation.Type.ToString(),
            ProcessId = operation.ProcessId,
            OccurredAt = operation.OccurredAt,
            DataJson = dataJson
        };
        await _context.WasteOperations.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }
}
