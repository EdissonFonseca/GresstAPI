# Guía para Ejecutar Pruebas Unitarias

## Ejecutar Todas las Pruebas

### Desde la raíz del proyecto:
```bash
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj
```

### Desde el directorio de pruebas:
```bash
cd tests/Gresst.Tests
dotnet test
```

### Con salida detallada:
```bash
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj --verbosity normal
```

### Con salida muy detallada (ver cada prueba):
```bash
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj --verbosity detailed
```

## Ejecutar Pruebas Específicas

### Por nombre de clase:
```bash
# Solo pruebas de FacilityService
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj --filter "FullyQualifiedName~FacilityServiceTests"

# Solo pruebas de WasteClassService
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj --filter "FullyQualifiedName~WasteClassServiceTests"

# Solo pruebas de IdConversion
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj --filter "FullyQualifiedName~IdConversionTests"
```

### Por nombre de método específico:
```bash
# Una prueba específica
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj --filter "FullyQualifiedName~GetByIdAsync_WhenFacilityExists_ReturnsFacilityDto"

# Múltiples pruebas con patrón
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj --filter "FullyQualifiedName~GetAllAsync"
```

### Por categoría (si usas [Trait]):
```bash
# Si marcas pruebas con [Trait("Category", "Integration")]
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj --filter "Category=Integration"
```

## Ejecutar con Cobertura de Código

### Usando coverlet (requiere paquete coverlet.collector):
```bash
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=opencover \
  /p:CoverletOutput=./coverage.xml
```

### Ver cobertura en formato HTML:
```bash
# Instalar reportgenerator si no lo tienes
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generar reporte
reportgenerator \
  -reports:./coverage.xml \
  -targetdir:./coverage-report \
  -reporttypes:Html
```

## Ejecutar Pruebas en Paralelo

### Por defecto, las pruebas se ejecutan en paralelo. Para deshabilitar:
```bash
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj --no-build -- --parallel none
```

## Ejecutar Pruebas y Ver Resultados en Formato JSON

```bash
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj --logger "json;LogFilePath=test-results.json"
```

## Ejecutar Pruebas y Ver Solo Resumen

```bash
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj --verbosity minimal
```

## Ejemplos Prácticos

### Ver todas las pruebas que pasan:
```bash
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj --verbosity normal | grep "Passed"
```

### Ver solo las pruebas que fallan:
```bash
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj --verbosity normal | grep "Failed"
```

### Ejecutar pruebas y continuar aunque algunas fallen:
```bash
dotnet test tests/Gresst.Tests/Gresst.Tests.csproj --no-build -- --continue
```

## Ejecutar desde Visual Studio Code

1. Abre el panel de Testing (Ctrl+Shift+P → "Test: Focus on Test View")
2. Verás todas las pruebas listadas
3. Click en el ícono de play ▶️ para ejecutar todas
4. Click en el ícono de play junto a una prueba específica para ejecutarla sola

## Ejecutar desde Visual Studio

1. Abre el Test Explorer (Test → Test Explorer)
2. Click en "Run All" para ejecutar todas
3. Click derecho en una prueba específica → "Run Selected Tests"

## Ejecutar desde Rider

1. Abre la ventana de Unit Tests
2. Click en el botón de play para ejecutar todas
3. Click en una prueba específica para ejecutarla sola

