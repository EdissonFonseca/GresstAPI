# Arquitectura Gresst - Mapeo Domain ↔ Base de Datos

## 📊 Arquitectura de Capas con Mappers

```
┌─────────────────────────────────────────────────────────────┐
│                        API Layer                             │
│  Controllers (Inglés) - REST Endpoints                      │
│  FacilityController, WasteController, ManagementController  │
└────────────────┬────────────────────────────────────────────┘
                 │ DTOs (Inglés)
                 ↓
┌─────────────────────────────────────────────────────────────┐
│                   Application Layer                          │
│  Services (Inglés) - Business Logic                         │
│  FacilityService, WasteService, ManagementService           │
└────────────────┬────────────────────────────────────────────┘
                 │ Domain Entities (Inglés)
                 ↓
┌─────────────────────────────────────────────────────────────┐
│                     Domain Layer                             │
│  Entities: Facility, Waste, Management, Person              │
│  Enums: WasteStatus, ManagementType, OrderType              │
│  Interfaces: IRepository, IUnitOfWork                       │
└────────────────┬────────────────────────────────────────────┘
                 │
                 ↓
┌─────────────────────────────────────────────────────────────┐
│                 Infrastructure Layer                         │
│                                                              │
│  ┌──────────────────────────────────────────────────┐      │
│  │           MAPPERS (Bidirectional)                │      │
│  │  FacilityMapper: Facility ↔ Deposito            │      │
│  │  WasteMapper:    Waste ↔ Residuo                │      │
│  │  ManagementMapper: Management ↔ Gestion         │      │
│  │  PersonMapper:   Person ↔ Persona               │      │
│  └──────────────────┬───────────────────────────────┘      │
│                     │                                        │
│  ┌──────────────────↓───────────────────────────────┐      │
│  │        Repositories (Con Mappers)                │      │
│  │  - FacilityRepository                            │      │
│  │  - WasteRepository                               │      │
│  │  - ManagementRepository                          │      │
│  │  - PersonRepository                              │      │
│  └──────────────────┬───────────────────────────────┘      │
│                     │                                        │
│  ┌──────────────────↓───────────────────────────────┐      │
│  │    Database Entities (Español/Scaffolded)       │      │
│  │    Deposito, Residuo, Gestion, Persona          │      │
│  │    (124 entidades de la BD AWS)                 │      │
│  └──────────────────┬───────────────────────────────┘      │
│                     │                                        │
│  ┌──────────────────↓───────────────────────────────┐      │
│  │         InfrastructureDbContext                  │      │
│  │         (EF Core DbContext)                      │      │
│  └──────────────────┬───────────────────────────────┘      │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ↓
┌─────────────────────────────────────────────────────────────┐
│              SQL Server Database (AWS)                       │
│  ec2-18-224-46-73.us-east-2.compute.amazonaws.com          │
│  Database: QA.Gresst                                        │
│  Tablas: Deposito, Residuo, Gestion, Persona (Español)     │
└─────────────────────────────────────────────────────────────┘
```

## 🔄 Flujo de Datos Completo

### **Flujo de Lectura (GET)**

```
1. Client → GET /api/facility/{id}
         ↓
2. FacilityController.GetById(guid)
         ↓
3. FacilityService.GetByIdAsync(guid)
         ↓
4. FacilityRepository.GetByIdAsync(guid)
   - Convierte Guid → long
   - Consulta: _context.Depositos.FindAsync(idLong)
         ↓
5. FacilityMapper.ToDomain(deposito)
   - Deposito (BD/Español) → Facility (Domain/Inglés)
   - Nombre → Name
   - Acopio → CanCollect
   - Ubicacion (geography) → Latitude/Longitude
         ↓
6. FacilityService devuelve FacilityDto
         ↓
7. Controller → JSON Response (Inglés)
```

### **Flujo de Escritura (POST/PUT)**

```
1. Client → POST /api/facility + JSON (Inglés)
         ↓
2. FacilityController.Create(CreateFacilityDto)
         ↓
3. FacilityService.CreateAsync(dto)
   - Crea Facility (Domain entity)
         ↓
4. FacilityRepository.AddAsync(facility)
         ↓
5. FacilityMapper.ToDatabase(facility)
   - Facility (Domain/Inglés) → Deposito (BD/Español)
   - Name → Nombre
   - CanCollect → Acopio
   - Latitude/Longitude → Ubicacion (geography)
         ↓
6. DbContext.Depositos.AddAsync(deposito)
         ↓
7. SaveChangesAsync() → SQL Server
         ↓
8. Response: FacilityDto con ID generado
```

## 📋 Tabla de Mapeo: Domain ↔ Database

### **Facility ↔ Deposito**

| Domain (Inglés) | BD (Español) | Tipo Conversión |
|-----------------|--------------|-----------------|
| `Id` (Guid) | `IdDeposito` (long) | Guid ↔ long |
| `AccountId` (Guid) | `IdCuenta` (long) | Guid ↔ long |
| `Code` | `Referencia` | string |
| `Name` | `Nombre` | string |
| `Description` | `Notas` | string |
| `Address` | `Direccion` | string |
| `Latitude/Longitude` | `Ubicacion` | Geometry |
| `PersonId` (Guid) | `IdPersona` (string) | Guid ↔ string |
| `CanCollect` | `Acopio` | bool |
| `CanStore` | `Almacenamiento` | bool |
| `CanDispose` | `Disposicion` | bool |
| `CanTreat` | `Tratamiento` | bool |
| `CanReceive` | `Recepcion` | bool |
| `CanDeliver` | `Entrega` | bool |

### **Waste ↔ Residuo**

| Domain (Inglés) | BD (Español) | Tipo Conversión |
|-----------------|--------------|-----------------|
| `Id` (Guid) | `IdResiduo` (long) | Guid ↔ long |
| `Code` | `Referencia` | string |
| `Description` | `Descripcion` | string |
| `WasteTypeId` (Guid) | `IdMaterial` (long) | Guid ↔ long |
| `Status` (enum) | `IdEstado` (string 1 char) | Enum ↔ char |
| `GeneratedAt` | `FechaIngreso` | DateTime |
| `CurrentOwnerId` (Guid) | `IdPropietario` (string) | Guid ↔ string |

### **Management ↔ Gestion**

| Domain (Inglés) | BD (Español) | Tipo Conversión |
|-----------------|--------------|-----------------|
| `Id` (Guid) | `IdMovimiento` (long) | Guid ↔ long |
| `Type` (enum) | `IdServicio` (long) | Enum ↔ long |
| `ExecutedAt` | `Fecha` | DateTime |
| `WasteId` (Guid) | `IdResiduo` (long) | Guid ↔ long |
| `Quantity` | `Peso / Cantidad` | decimal |
| `ExecutedById` (Guid) | `IdResponsable` (string) | Guid ↔ string |
| `OriginFacilityId` | `IdDepositoOrigen` | Guid ↔ long |
| `DestinationFacilityId` | `IdDepositoDestino` | Guid ↔ long |
| `Notes` | `Observaciones` | string |

### **Person ↔ Persona**

| Domain (Inglés) | BD (Español) | Tipo Conversión |
|-----------------|--------------|-----------------|
| `Id` (Guid) | `IdPersona` (string) | Guid ↔ string |
| `AccountId` (Guid) | `IdCuenta` (long) | Guid ↔ long |
| `Name` | `Nombre` | string |
| `DocumentNumber` | `Identificacion` | string |
| `Email` | `Correo` | string |
| `Phone` | `Telefono` | string |
| `Address` | `Direccion` | string |

## 🔑 Conversiones de Tipos

### **Guid ↔ long**
```csharp
// Guid → long
long ConvertGuidToLong(Guid guid) 
{
    var guidString = guid.ToString().Replace("-", "");
    var numericPart = new string(guidString.Where(char.IsDigit).Take(18).ToArray());
    return long.Parse(numericPart);
}

// long → Guid
Guid ConvertLongToGuid(long id) 
{
    return new Guid(id.ToString().PadLeft(32, '0'));
}
```

### **Guid ↔ string (40 chars)**
```csharp
// Guid → string
string ConvertGuidToString(Guid guid) 
{
    return guid.ToString().Replace("-", "").Substring(0, 40);
}

// string → Guid
Guid ConvertStringToGuid(string id) 
{
    if (Guid.TryParse(id, out var guid))
        return guid;
    return new Guid(id.PadLeft(32, '0').Substring(0, 32));
}
```

### **Enum ↔ char/long**
```csharp
// WasteStatus enum → char
"G" = Generated
"T" = InTransit
"A" = Stored (Almacenado)
"D" = Disposed
"R" = Transformed/Reused

// ManagementType enum → IdServicio (long)
1 = Generate
2 = Collect
3 = Transport
4 = Receive
5 = Store
6 = Dispose
7 = Treat
8 = Transform
9 = Deliver
10 = Sell
11 = Classify
```

### **Geography/Geometry ↔ Lat/Long**
```csharp
// Geometry → Lat/Long
decimal? GetLatitude(Geometry geometry) => (geometry as Point)?.Y;
decimal? GetLongitude(Geometry geometry) => (geometry as Point)?.X;

// Lat/Long → Geometry
Geometry CreatePoint(decimal? lat, decimal? lon) 
{
    return new Point((double)lon, (double)lat) { SRID = 4326 };
}
```

## ✅ Ventajas de esta Arquitectura

1. ✅ **API en Inglés** - Estándar internacional
2. ✅ **BD en Español** - Sin modificar legacy database
3. ✅ **Clean Architecture** - Separación completa
4. ✅ **Mapeo Transparente** - El cliente no sabe que la BD está en español
5. ✅ **Multitenant** - Filtrado por `IdCuenta` / `AccountId`
6. ✅ **Type Safety** - Conversiones automáticas y seguras
7. ✅ **Mantenibilidad** - Cambios en BD no afectan Domain
8. ✅ **Testeable** - Domain sin dependencia de BD

## 🎯 Patrón para Crear Más Mappers

Para crear mappers de las otras 19 entidades, sigue este patrón:

1. **Crear Mapper** en `Infrastructure/Mappers/`
   - Heredar de `MapperBase<TDomain, TDatabase>`
   - Implementar `ToDomain()`, `ToDatabase()`, `UpdateDatabase()`
   - Agregar conversiones de tipos necesarias

2. **Crear Repository** en `Infrastructure/Repositories/`
   - Inyectar el `Mapper` y `DbContext`
   - Implementar `IRepository<TDomain>`
   - Usar el mapper en cada operación CRUD

3. **Registrar en DI** (`Program.cs`)
   ```csharp
   builder.Services.AddScoped<EntityMapper>();
   builder.Services.AddScoped<IRepository<Entity>, EntityRepository>();
   ```

4. **Usar en Services** (ya funcionan automáticamente)

## 📝 Entidades Completadas

- ✅ **Facility** ↔ **Deposito** (Plantas, depósitos, sitios)
- ✅ **Waste** ↔ **Residuo** (Residuos individuales)
- ✅ **Management** ↔ **Gestion** (Operaciones/Movimientos)
- ✅ **Person** ↔ **Persona** (Actores del sistema)

## 📝 Entidades Pendientes (18 más)

- ⏳ Request ↔ Solicitud
- ⏳ Order ↔ Orden
- ⏳ Certificate ↔ Certificado
- ⏳ License ↔ Licencium
- ⏳ Vehicle ↔ Vehiculo
- ⏳ WasteType ↔ Material/TipoResiduo
- ⏳ Classification ↔ Clasificacion
- ⏳ Location ↔ Localizacion
- ⏳ Balance ↔ Saldo
- ⏳ Adjustment ↔ Ajuste
- ⏳ WasteTransformation ↔ ResiduoTransformacion
- ⏳ Treatment ↔ Tratamiento
- ⏳ Material ↔ Material
- ⏳ Packaging ↔ Embalaje
- ⏳ Route ↔ Rutum
- ⏳ User ↔ Usuario
- ⏳ Rate ↔ Tarifa
- ⏳ RouteStop ↔ RutaDeposito

## 🚀 Ejemplo de Uso Completo

### **Crear Facility** (Planta de Tratamiento)

```bash
POST /api/facility
Headers: X-Account-Id: 12345678-1234-1234-1234-123456789012

{
  "code": "PLANTA-001",
  "name": "Planta de Tratamiento Norte",
  "description": "Planta principal",
  "facilityType": "TreatmentPlant",
  "address": "Calle 100 #50-20",
  "latitude": 4.701594,
  "longitude": -74.035126,
  "personId": "person-guid",
  "canCollect": true,
  "canStore": true,
  "canTreat": true,
  "maxCapacity": 10000,
  "capacityUnit": "kg"
}
```

**Lo que pasa:**
1. Controller recibe JSON en inglés
2. Service crea `Facility` (Domain)
3. Repository llama `FacilityMapper.ToDatabase()`
4. Se guarda en tabla `Deposito` con campos en español:
   - `Name` → `Nombre`
   - `CanTreat` → `Tratamiento = true`
   - `Lat/Long` → `Ubicacion` (geography)
5. SQL Server guarda el registro
6. Responde con `FacilityDto` en inglés

### **Obtener Facilities**

```bash
GET /api/facility
Headers: X-Account-Id: 12345678-1234-1234-1234-123456789012
```

**Lo que pasa:**
1. Repository consulta tabla `Deposito`
2. Filtrado automático: `WHERE IdCuenta = accountId AND Activo = 1`
3. Para cada `Deposito`, llama `FacilityMapper.ToDomain()`
4. Convierte:
   - `Nombre` → `Name`
   - `Tratamiento` → `CanTreat`
   - `Ubicacion` → `Latitude/Longitude`
5. Responde array de `FacilityDto` en inglés

## 🎨 Ventaja Clave: Transparencia

El cliente de la API **nunca sabe** que:
- La BD está en español
- Los IDs son diferentes (Guid vs long/string)
- Hay conversiones complejas de tipos
- Los nombres de campos son diferentes

Todo es **automático y transparente** gracias a los mappers! 🚀

