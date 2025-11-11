# Mappers - Traductores Domain ↔ Database

Este directorio contiene los mappers que traducen entre:

- **Domain Entities** (inglés, en `Gresst.Domain/Entities/`) - Modelo limpio de negocio
- **Database Entities** (español, en `Infrastructure/Data/Entities/`) - Modelo scaffoldeado de la BD

## Estructura

```
Mappers/
├── IMapper.cs              - Interfaz base para todos los mappers
├── MapperBase.cs           - Clase base con lógica común
├── WasteMapper.cs          - Waste ↔ Residuo
├── PersonMapper.cs         - Person ↔ Persona  
├── FacilityMapper.cs       - Facility ↔ Deposito
├── OrderMapper.cs          - Order ↔ Orden
└── ...                     - Un mapper por cada entidad principal
```

## Ejemplo de Uso

```csharp
// En el repositorio
var dbEntity = await _context.Residuos.FindAsync(id);
var domainEntity = _wasteMapper.ToDomain(dbEntity);

// Para guardar
var dbEntity = _wasteMapper.ToDatabase(domainEntity);
await _context.Residuos.AddAsync(dbEntity);
```

## Patrón de Implementación

Cada mapper debe:
1. Implementar `IMapper<TDomain, TDatabase>`
2. Heredar de `MapperBase<TDomain, TDatabase>`
3. Manejar valores nulos correctamente
4. Mapear todas las propiedades relevantes
5. No incluir navegación circular (usar IDs en lugar de objetos completos cuando sea necesario)

## Beneficios

- ✅ Domain limpio e independiente de la BD
- ✅ BD legacy puede tener nombres en español
- ✅ Flexibilidad para cambiar la BD sin afectar el dominio
- ✅ Facilita testing del dominio sin BD
- ✅ Cumple con Clean Architecture

