# Estructura de Pruebas - Clean Architecture

## Principio: Estructura Espejo (Mirror Structure)

En Clean Architecture, **la mejor práctica es reflejar la estructura del proyecto principal** en las pruebas. Esto facilita:

1. **Encontrar pruebas rápidamente**: Si buscas pruebas de `FacilityService`, sabes que están en `Application/Services/`
2. **Mantener coherencia**: La estructura de pruebas sigue la misma lógica que el código de producción
3. **Escalabilidad**: Fácil agregar nuevas pruebas siguiendo el mismo patrón
4. **Navegación intuitiva**: Los desarrolladores encuentran las pruebas de forma natural

## Estructura Actual

```
tests/Gresst.Tests/
├── Application/                    # Pruebas de la capa de aplicación
│   └── Services/                   # Pruebas de servicios de aplicación
│       ├── FacilityServiceTests.cs
│       └── WasteClassServiceTests.cs
│
├── Infrastructure/                  # Pruebas de la capa de infraestructura
│   ├── Common/                     # Pruebas de utilidades comunes
│   │   └── GuidLongConverterTests.cs
│   ├── Mappers/                    # Pruebas de mappers (pendiente)
│   │   └── (ej: FacilityMapperTests.cs)
│   └── Repositories/               # Pruebas de repositorios (pendiente)
│       └── (ej: FacilityRepositoryTests.cs)
│
├── Domain/                          # Pruebas de la capa de dominio (si es necesario)
│   └── Entities/                   # Pruebas de entidades (si tienen lógica)
│       └── (ej: FacilityTests.cs)
│
└── API/                             # Pruebas de la capa de presentación (opcional)
    └── Controllers/                  # Pruebas de controladores (si se hacen)
        └── (ej: FacilityControllerTests.cs)
```

## Correspondencia con el Proyecto Principal

```
src/                                tests/
├── Gresst.Application/             ├── Application/
│   └── Services/                   │   └── Services/
│       └── FacilityService.cs     │       └── FacilityServiceTests.cs
│
├── Gresst.Infrastructure/          ├── Infrastructure/
│   ├── Common/                     │   ├── Common/
│   │   └── GuidLongConverter.cs   │   │   └── GuidLongConverterTests.cs
│   ├── Mappers/                    │   ├── Mappers/
│   │   └── FacilityMapper.cs      │   │   └── FacilityMapperTests.cs
│   └── Repositories/               │   └── Repositories/
│       └── FacilityRepository.cs   │       └── FacilityRepositoryTests.cs
│
└── Gresst.Domain/                   └── Domain/
    └── Entities/                    │   └── Entities/
        └── Facility.cs             │       └── FacilityTests.cs (si tiene lógica)
```

## Ventajas de esta Estructura

### ✅ **Facilidad de Navegación**
- Si trabajas en `src/Gresst.Application/Services/FacilityService.cs`
- Sabes que las pruebas están en `tests/Gresst.Tests/Application/Services/FacilityServiceTests.cs`

### ✅ **Mantenibilidad**
- Cuando refactorizas código, es fácil encontrar y actualizar las pruebas correspondientes
- La estructura espejo hace obvio dónde deben ir las nuevas pruebas

### ✅ **Escalabilidad**
- Fácil agregar nuevas pruebas siguiendo el mismo patrón
- No necesitas pensar dónde ponerlas, solo sigue la estructura

### ✅ **Separación de Responsabilidades**
- Las pruebas de cada capa están claramente separadas
- Facilita entender qué capa estás probando

## Convenciones de Nomenclatura

### Archivos de Prueba
- **Patrón**: `{Clase}Tests.cs`
- **Ejemplo**: `FacilityService.cs` → `FacilityServiceTests.cs`

### Namespaces
- **Patrón**: `Gresst.Tests.{Capa}.{Subcarpeta}`
- **Ejemplo**: 
  - `Gresst.Application.Services.FacilityService` 
  - → `Gresst.Tests.Application.Services.FacilityServiceTests`

### Clases de Prueba
- **Patrón**: `{Clase}Tests`
- **Ejemplo**: `FacilityService` → `FacilityServiceTests`

## Tipos de Pruebas por Capa

### Application Layer
- **Servicios**: Lógica de negocio, validaciones, transformaciones
- **DTOs**: Validaciones de datos (si tienen lógica)

### Infrastructure Layer
- **Mappers**: Conversiones entre dominio y BD
- **Repositories**: Acceso a datos (usualmente con mocks o base de datos en memoria)
- **Common**: Utilidades y helpers

### Domain Layer
- **Entidades**: Solo si tienen lógica de negocio compleja
- **Value Objects**: Validaciones y comportamientos
- **Domain Services**: Lógica de dominio

### API Layer (Opcional)
- **Controllers**: Pruebas de integración o unitarias con mocks
- **Middleware**: Comportamiento de middleware
- **Filters**: Filtros personalizados

## Ejemplo de Estructura Completa

```
tests/Gresst.Tests/
├── Application/
│   ├── Services/
│   │   ├── FacilityServiceTests.cs
│   │   ├── WasteClassServiceTests.cs
│   │   ├── MaterialServiceTests.cs
│   │   └── VehicleServiceTests.cs
│   └── DTOs/                        # Si los DTOs tienen lógica
│       └── FacilityDtoTests.cs
│
├── Infrastructure/
│   ├── Common/
│   │   └── GuidLongConverterTests.cs
│   ├── Mappers/
│   │   ├── FacilityMapperTests.cs
│   │   └── WasteClassMapperTests.cs
│   └── Repositories/
│       ├── FacilityRepositoryTests.cs
│       └── WasteClassRepositoryTests.cs
│
└── Domain/
    └── Entities/
        └── FacilityTests.cs        # Solo si tiene lógica compleja
```

## Alternativas (No Recomendadas)

### ❌ Estructura por Tipo de Prueba
```
tests/
├── Unit/
├── Integration/
└── E2E/
```
**Problema**: Difícil encontrar pruebas de un componente específico

### ❌ Estructura Plana
```
tests/
├── FacilityServiceTests.cs
├── WasteClassServiceTests.cs
└── GuidLongConverterTests.cs
```
**Problema**: No escala bien, difícil de navegar

### ❌ Estructura por Feature
```
tests/
├── Facility/
│   ├── FacilityServiceTests.cs
│   └── FacilityRepositoryTests.cs
└── WasteClass/
    ├── WasteClassServiceTests.cs
    └── WasteClassRepositoryTests.cs
```
**Problema**: Duplica componentes que pertenecen a diferentes capas

## Conclusión

**La estructura espejo es la mejor práctica** porque:
- ✅ Refleja la arquitectura del proyecto
- ✅ Facilita encontrar y mantener pruebas
- ✅ Escala bien con proyectos grandes
- ✅ Es intuitiva para nuevos desarrolladores
- ✅ Sigue principios de Clean Architecture

