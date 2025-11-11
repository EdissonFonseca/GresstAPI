# Ejemplos de CRUDs - Gresst API

## 🏢 FACILITY (Deposito) - Ejemplo Completo

### ✅ **CREATE - Crear Facility**

```bash
curl -X POST http://localhost:5000/api/facility \
  -H "Content-Type: application/json" \
  -H "X-Account-Id: 00000000-0000-0000-0001-000000000001" \
  -d '{
    "code": "PLANTA-NORTE-001",
    "name": "Planta de Tratamiento Norte",
    "description": "Planta principal para tratamiento de residuos industriales",
    "facilityType": "TreatmentPlant",
    "address": "Autopista Norte Km 12, Bogotá",
    "latitude": 4.701594,
    "longitude": -74.035126,
    "personId": "00000000-0000-0000-0002-000000000001",
    "canCollect": true,
    "canStore": true,
    "canDispose": false,
    "canTreat": true,
    "canReceive": true,
    "canDeliver": true,
    "maxCapacity": 50000,
    "capacityUnit": "kg"
  }'
```

**Lo que se guarda en la BD (tabla Deposito):**
```sql
INSERT INTO Deposito (
    Nombre,              -- "Planta de Tratamiento Norte"
    Notas,               -- "Planta principal..."
    Direccion,           -- "Autopista Norte..."
    Ubicacion,           -- GEOGRAPHY point
    Acopio,              -- true (CanCollect)
    Almacenamiento,      -- true (CanStore)
    Tratamiento,         -- true (CanTreat)
    Recepcion,           -- true (CanReceive)
    Entrega,             -- true (CanDeliver)
    Disposicion,         -- false (CanDispose)
    Peso,                -- 50000 (MaxCapacity)
    IdCuenta,            -- (AccountId convertido a long)
    Activo               -- true
)
```

---

### 📖 **READ - Obtener Facilities**

#### **a) Obtener todas las facilities**

```bash
curl -X GET http://localhost:5000/api/facility \
  -H "X-Account-Id: 00000000-0000-0000-0001-000000000001"
```

**Respuesta:**
```json
[
  {
    "id": "00000000-0000-0000-0000-000000000123",
    "code": "PLANTA-NORTE-001",
    "name": "Planta de Tratamiento Norte",
    "description": "Planta principal...",
    "facilityType": "TreatmentPlant",
    "address": "Autopista Norte Km 12, Bogotá",
    "latitude": 4.701594,
    "longitude": -74.035126,
    "personId": "00000000-0000-0000-0002-000000000001",
    "canCollect": true,
    "canStore": true,
    "canDispose": false,
    "canTreat": true,
    "canReceive": true,
    "canDeliver": true,
    "maxCapacity": 50000.0,
    "capacityUnit": "kg",
    "currentCapacity": 0,
    "isActive": true
  }
]
```

#### **b) Obtener por ID**

```bash
curl -X GET http://localhost:5000/api/facility/00000000-0000-0000-0000-000000000123 \
  -H "X-Account-Id: 00000000-0000-0000-0001-000000000001"
```

#### **c) Obtener por persona (dueño)**

```bash
curl -X GET http://localhost:5000/api/facility/person/00000000-0000-0000-0002-000000000001 \
  -H "X-Account-Id: 00000000-0000-0000-0001-000000000001"
```

#### **d) Obtener por tipo**

```bash
# Tipos: TreatmentPlant, DisposalSite, StorageFacility, TransferStation
curl -X GET http://localhost:5000/api/facility/type/TreatmentPlant \
  -H "X-Account-Id: 00000000-0000-0000-0001-000000000001"
```

---

### ✏️ **UPDATE - Actualizar Facility**

```bash
curl -X PUT http://localhost:5000/api/facility/00000000-0000-0000-0000-000000000123 \
  -H "Content-Type: application/json" \
  -H "X-Account-Id: 00000000-0000-0000-0001-000000000001" \
  -d '{
    "id": "00000000-0000-0000-0000-000000000123",
    "name": "Planta de Tratamiento Norte - Ampliada",
    "description": "Planta ampliada con nueva capacidad",
    "canDispose": true,
    "maxCapacity": 75000,
    "currentCapacity": 15000
  }'
```

**Lo que se actualiza en BD:**
```sql
UPDATE Deposito SET
    Nombre = 'Planta de Tratamiento Norte - Ampliada',
    Notas = 'Planta ampliada...',
    Disposicion = 1,                    -- Ahora puede disponer
    Peso = 75000,                       -- Nueva capacidad máxima
    Cantidad = 15000,                   -- Capacidad actual
    FechaUltimaModificacion = GETDATE()
WHERE IdDeposito = 123
```

---

### 🗑️ **DELETE - Eliminar Facility (Soft Delete)**

```bash
curl -X DELETE http://localhost:5000/api/facility/00000000-0000-0000-0000-000000000123 \
  -H "X-Account-Id: 00000000-0000-0000-0001-000000000001"
```

**Lo que pasa en BD:**
```sql
UPDATE Deposito SET
    Activo = 0,
    FechaUltimaModificacion = GETDATE()
WHERE IdDeposito = 123
-- NO se elimina el registro, solo se marca como inactivo
```

---

## 🗑️ WASTE (Residuo) - Operaciones

### CREATE Waste

```bash
curl -X POST http://localhost:5000/api/waste \
  -H "Content-Type: application/json" \
  -H "X-Account-Id: 00000000-0000-0000-0001-000000000001" \
  -d '{
    "code": "WASTE-2025-001",
    "description": "Residuos plásticos industriales",
    "wasteTypeId": "00000000-0000-0000-0000-000000000456",
    "quantity": 500.5,
    "unit": 1,
    "generatorId": "00000000-0000-0000-0002-000000000001",
    "isHazardous": false,
    "batchNumber": "BATCH-2025-01-10"
  }'
```

**Mapeo a BD (tabla Residuo):**
- `Code` → `Referencia`
- `Description` → `Descripcion`
- `WasteTypeId` → `IdMaterial`
- `Status: Generated` → `IdEstado: "G"`
- `GeneratorId` → `IdPropietario`
- `GeneratedAt` → `FechaIngreso`

### GET Waste

```bash
# Todos los residuos
GET /api/waste

# Por generador
GET /api/waste/generator/{generatorId}

# Por estado
GET /api/waste/status/Generated
GET /api/waste/status/InTransit
GET /api/waste/status/Disposed

# Banco de residuos
GET /api/waste/bank
```

---

## 🔄 MANAGEMENT (Gestion) - Las 11 Operaciones

### **1. GENERAR Residuo**

```bash
POST /api/management/generate
{
  "code": "WASTE-001",
  "description": "Plásticos industriales",
  "wasteTypeId": "guid-material",
  "quantity": 500,
  "unit": 1,
  "generatorId": "guid-persona",
  "isHazardous": false
}
```

**Se guarda en BD:**
- Tabla `Residuo`: Nuevo residuo con `IdEstado = "G"`
- Tabla `Gestion`: Movimiento con `IdServicio = 1` (Generate)

### **2. RECOGER Residuo**

```bash
POST /api/management/collect
{
  "wasteId": "guid-residuo",
  "quantity": 500,
  "collectorId": "guid-recolector",
  "vehicleId": "guid-vehiculo",
  "originLocationId": "guid-ubicacion"
}
```

**Se guarda:**
- Tabla `Gestion`: `IdServicio = 2` (Collect)
- Tabla `Residuo`: `IdEstado = "T"` (InTransit)

### **3. TRANSPORTAR Residuo**

```bash
POST /api/management/transport
{
  "wasteId": "guid-residuo",
  "quantity": 500,
  "transporterId": "guid-transportista",
  "vehicleId": "guid-vehiculo",
  "originFacilityId": "guid-deposito-origen",
  "destinationFacilityId": "guid-deposito-destino"
}
```

**Mapeo a BD (tabla Gestion):**
```sql
INSERT INTO Gestion (
    IdResiduo,           -- wasteId convertido a long
    IdServicio,          -- 3 (Transport)
    IdResponsable,       -- transporterId convertido a string
    IdVehiculo,          -- vehicleId convertido a string
    IdDepositoOrigen,    -- originFacilityId convertido a long
    IdDepositoDestino,   -- destinationFacilityId convertido a long
    Peso,                -- quantity
    Fecha,               -- DateTime.UtcNow
    Procesado            -- false
)
```

### **Historial Completo de un Residuo**

```bash
GET /api/management/waste/{wasteId}/history
```

**Respuesta:**
```json
[
  {
    "id": "guid1",
    "code": "MGT-1",
    "type": "Generate",
    "executedAt": "2025-01-10T10:00:00Z",
    "quantity": 500,
    "executedByName": "Empresa Generadora"
  },
  {
    "id": "guid2",
    "type": "Collect",
    "executedAt": "2025-01-10T14:00:00Z",
    "quantity": 500,
    "executedByName": "Transportadora ABC"
  },
  {
    "id": "guid3",
    "type": "Transport",
    "executedAt": "2025-01-10T15:30:00Z",
    "quantity": 500,
    "executedByName": "Transportadora ABC"
  },
  {
    "id": "guid4",
    "type": "Receive",
    "executedAt": "2025-01-10T17:00:00Z",
    "quantity": 500,
    "executedByName": "Planta de Tratamiento"
  }
]
```

---

## 👤 PERSON (Persona) - Operaciones

### CREATE Person

```bash
POST /api/person
{
  "name": "Transportes XYZ S.A.S.",
  "documentNumber": "900123456",
  "email": "info@transportesxyz.com",
  "phone": "+57 300 1234567",
  "address": "Carrera 50 #80-30, Bogotá",
  "isCollector": true,
  "isTransporter": true,
  "isReceiver": true
}
```

### GET Persons

```bash
GET /api/person
GET /api/person/{id}
GET /api/person/document/{documentNumber}
```

---

## 🔄 Flujo Completo: Transportar un Residuo

### **Paso 1: Crear Persona (Transportista)**

```bash
POST /api/person
{
  "name": "Transportes ABC",
  "isTransporter": true,
  "isCollector": true
}
```
→ Respuesta: `personId: "xxx"`

### **Paso 2: Crear Facility de Origen**

```bash
POST /api/facility
{
  "code": "FAC-ORIGEN",
  "name": "Depósito Origen",
  "canDeliver": true
}
```
→ Respuesta: `originFacilityId: "yyy"`

### **Paso 3: Crear Facility de Destino**

```bash
POST /api/facility
{
  "code": "FAC-DESTINO",
  "name": "Planta de Tratamiento",
  "canReceive": true,
  "canTreat": true
}
```
→ Respuesta: `destinationFacilityId: "zzz"`

### **Paso 4: Generar Residuo**

```bash
POST /api/management/generate
{
  "code": "WASTE-001",
  "quantity": 500,
  "generatorId": "{personId}",
  ...
}
```
→ Respuesta: `wasteId: "www"`

### **Paso 5: Transportar**

```bash
POST /api/management/transport
{
  "wasteId": "{wasteId}",
  "quantity": 500,
  "transporterId": "{personId}",
  "vehicleId": "{vehicleId}",
  "originFacilityId": "{originFacilityId}",
  "destinationFacilityId": "{destinationFacilityId}"
}
```

**Resultado en BD:**

**Tabla Gestion (nuevo registro):**
```
IdMovimiento: 123
IdResiduo: (wasteId convertido)
IdServicio: 3 (Transport)
IdResponsable: (transporterId)
IdDepositoOrigen: (originFacilityId)
IdDepositoDestino: (destinationFacilityId)
Peso: 500
Fecha: 2025-01-10 15:30:00
```

**Tabla Residuo (actualizado):**
```
IdEstado: "T" (InTransit)
```

### **Paso 6: Ver Historial**

```bash
GET /api/management/waste/{wasteId}/history
```

Verás todos los movimientos en orden cronológico.

---

## 🎯 Resumen del Mapeo

### **Request (API/Inglés) → BD (Español)**

```
POST /api/facility
{
  "name": "Planta Norte",
  "canTreat": true,
  "maxCapacity": 10000
}

↓ FacilityController
↓ FacilityService  
↓ FacilityRepository
↓ FacilityMapper.ToDatabase()

INSERT INTO Deposito (
  Nombre = 'Planta Norte',
  Tratamiento = 1,
  Peso = 10000
)
```

### **BD (Español) → Response (API/Inglés)**

```
SELECT * FROM Deposito WHERE IdDeposito = 123

Resultado BD:
- Nombre: 'Planta Norte'
- Tratamiento: true
- Peso: 10000
- Ubicacion: GEOGRAPHY

↓ FacilityMapper.ToDomain()
↓ FacilityService
↓ FacilityController

Response JSON:
{
  "name": "Planta Norte",
  "canTreat": true,
  "maxCapacity": 10000,
  "latitude": 4.701594,
  "longitude": -74.035126
}
```

---

## 📊 Testing desde Swagger

1. Abre: `http://localhost:5000`
2. Verás todos los endpoints disponibles
3. Expande `/api/Facility`
4. Click en `POST /api/facility` → Try it out
5. Pega el JSON de ejemplo
6. Agrega header: `X-Account-Id: 00000000-0000-0000-0001-000000000001`
7. Execute

---

## ✅ ¿Qué está Completo?

### **Mappers Implementados (4/22)**
- ✅ FacilityMapper (Facility ↔ Deposito)
- ✅ WasteMapper (Waste ↔ Residuo)
- ✅ ManagementMapper (Management ↔ Gestion)
- ✅ PersonMapper (Person ↔ Persona)

### **Repositories Implementados (4/22)**
- ✅ FacilityRepository
- ✅ WasteRepository
- ✅ ManagementRepository
- ✅ PersonRepository

### **Controllers Funcionales**
- ✅ FacilityController (CRUD completo)
- ✅ WasteController (CRUD completo)
- ✅ ManagementController (11 operaciones)
- ✅ InventoryController (Balance/Inventario)

### **Funcionalidades**
- ✅ Crear/Leer/Actualizar/Eliminar Facilities
- ✅ Crear/Leer/Actualizar/Eliminar Wastes
- ✅ Registrar las 11 operaciones de gestión
- ✅ Consultar historial de trazabilidad
- ✅ Multitenant con AccountId
- ✅ Mapeo automático Domain ↔ BD
- ✅ Conversión de tipos Guid ↔ long ↔ string
- ✅ Soporte para geography/geometry
- ✅ Soft deletes

---

## 🚀 ¡Listo para Probar!

La API está corriendo en `http://localhost:5000` y lista para:
- ✅ Crear facilities (depósitos/plantas)
- ✅ Generar residuos
- ✅ Transportar residuos
- ✅ Ver trazabilidad completa

**Próximos pasos:**
- Crear mappers para las otras 18 entidades
- O empezar a usar la API con las 4 entidades principales ya implementadas

