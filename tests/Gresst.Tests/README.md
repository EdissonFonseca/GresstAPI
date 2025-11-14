# Gresst.Tests

Proyecto de pruebas unitarias para la API de Gresst.

## Estructura (Clean Architecture - Estructura Espejo)

```
tests/Gresst.Tests/
├── Application/                    # Pruebas de la capa de aplicación
│   └── Services/                   # Pruebas de servicios
│       ├── FacilityServiceTests.cs
│       └── WasteClassServiceTests.cs
│
├── Infrastructure/                  # Pruebas de la capa de infraestructura
│   └── Common/                     # Pruebas de utilidades
│       └── GuidLongConverterTests.cs
│
└── README.md
```

**Nota**: La estructura refleja la arquitectura del proyecto principal para facilitar la navegación y el mantenimiento. Ver [ESTRUCTURA-PRUEBAS.md](./ESTRUCTURA-PRUEBAS.md) para más detalles.

## Tecnologías Utilizadas

- **xUnit**: Framework de pruebas unitarias
- **Moq**: Framework para crear mocks y stubs
- **FluentAssertions**: Biblioteca para aserciones más legibles

## Ejecutar Pruebas

### Desde la línea de comandos:
```bash
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj
```

### Con cobertura de código:
```bash
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Ejecutar pruebas específicas:
```bash
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj --filter "FullyQualifiedName~FacilityServiceTests"
```

## Escribir Nuevas Pruebas

### Estructura de una Prueba

```csharp
[Fact]
public async Task MethodName_WhenCondition_ExpectedBehavior()
{
    // Arrange - Configurar datos y mocks
    var mock = new Mock<IRepository<Entity>>();
    mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
        .ReturnsAsync(new Entity { Id = Guid.NewGuid() });

    // Act - Ejecutar el método a probar
    var result = await service.GetByIdAsync(Guid.NewGuid());

    // Assert - Verificar el resultado
    result.Should().NotBeNull();
    result.Id.Should().NotBe(Guid.Empty);
}
```

### Convenciones

1. **Nombres de pruebas**: `MethodName_WhenCondition_ExpectedBehavior`
2. **Organización**: Arrange-Act-Assert (AAA)
3. **Mocks**: Usar `Moq` para dependencias
4. **Aserciones**: Usar `FluentAssertions` para mejor legibilidad

## Cobertura de Pruebas

Las pruebas actuales cubren:

- ✅ **Servicios**: FacilityService, WasteClassService
- ✅ **Utilidades**: GuidLongConverter
- ⏳ **Mappers**: Pendiente de implementar
- ⏳ **Repositorios**: Pendiente de implementar
- ⏳ **Controladores**: Pendiente de implementar

## Próximos Pasos

1. Agregar pruebas para mappers (FacilityMapper, WasteClassMapper, etc.)
2. Agregar pruebas para repositorios
3. Agregar pruebas de integración
4. Configurar cobertura de código en CI/CD

