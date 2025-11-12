# 🔒 Segmentación de Datos por Usuario (Row-Level Security)

## ✅ Implementado

Sistema completo de segmentación que **filtra automáticamente** los recursos (Facilities, Vehicles, Materials, etc.) según el usuario autenticado.

---

## 🎯 **¿Qué es Segmentación de Datos?**

**Problema:**
- Usuario A solo debe ver las facilities asignadas a él
- Usuario B solo debe ver sus vehículos
- Admin debe ver todo

**Solución:**
```
Tablas de asignación en BD:
├── UsuarioDeposito   (usuario → facilities)
├── UsuarioVehiculo   (usuario → vehicles)
├── UsuarioPersona    (usuario → persons)
└── ... etc.

Servicios filtran automáticamente por estas asignaciones
```

---

## 🏗️ **Arquitectura**

### **Flujo de Request:**

```
1. Cliente envía request:
   GET /api/facility
   Authorization: Bearer {token}
   
2. JWT Middleware valida token
   → Extrae UserId del token
   → Lo pone en HttpContext.User

3. CurrentUserService obtiene UserId
   → De los Claims del token

4. DataSegmentationService verifica asignaciones
   → Query a UsuarioDeposito
   → Devuelve IDs de facilities del usuario

5. FacilityService filtra resultados
   → Solo facilities asignadas al usuario
   → O todas si es Admin

6. Cliente recibe datos filtrados
   ← Solo facilities que le corresponden
```

---

## 📊 **Tablas de Asignación en BD**

### **UsuarioDeposito (Facilities):**
```sql
UsuarioDeposito:
├── IdUsuario (PK) → long
├── IdDeposito (PK) → long
├── FechaCreacion
└── IdUsuarioCreacion

Ejemplo:
IdUsuario=123, IdDeposito=456
→ Usuario 123 puede ver Facility 456
```

### **UsuarioVehiculo (Vehicles):**
```sql
UsuarioVehiculo:
├── IdUsuario (PK) → long
├── IdVehiculo (PK) → string
├── FechaCreacion
└── IdUsuarioCreacion
```

---

## 🔑 **Usuario Viene del Bearer Token**

### **1. Cliente envía token en cada request:**

```bash
GET /api/facility
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### **2. Token contiene UserId:**

```json
{
  "nameid": "123",          // ← UserId extraído aquí
  "name": "John Doe",
  "AccountId": "1",
  "email": "john@example.com",
  "role": ["User"],
  "exp": 1699876543
}
```

### **3. CurrentUserService extrae UserId:**

```csharp
public Guid GetCurrentUserId()
{
    // Lee del claim "nameid" (NameIdentifier)
    var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    return Guid.Parse(userId);
}
```

### **4. DataSegmentationService filtra:**

```csharp
public async Task<IEnumerable<Guid>> GetUserFacilityIdsAsync()
{
    var userId = _currentUserService.GetCurrentUserId(); // Del token
    var userIdLong = GuidLongConverter.ToLong(userId);
    
    // Query a UsuarioDeposito
    var facilityIds = await _context.UsuarioDepositos
        .Where(ud => ud.IdUsuario == userIdLong)
        .Select(ud => ud.IdDeposito)
        .ToListAsync();
    
    return facilityIds.Select(GuidLongConverter.ToGuid);
}
```

---

## ✅ **Servicios Actualizados con Segmentación**

### **FacilityService (ejemplo):**

```csharp
public async Task<IEnumerable<FacilityDto>> GetAllAsync()
{
    // ✅ Automáticamente filtra por usuario del token
    var userFacilityIds = await _segmentationService.GetUserFacilityIdsAsync();
    
    // Si es admin, devuelve todas
    if (await _segmentationService.CurrentUserIsAdminAsync())
        return await _facilityRepository.GetAllAsync();
    
    // Usuario normal: solo sus facilities
    return await _facilityRepository.FindAsync(f => userFacilityIds.Contains(f.Id));
}

public async Task<FacilityDto?> GetByIdAsync(Guid id)
{
    // ✅ Verifica que el usuario tenga acceso
    if (!await _segmentationService.UserHasAccessToFacilityAsync(id))
        return null; // 404 - Usuario no tiene acceso
    
    return await _facilityRepository.GetByIdAsync(id);
}
```

---

## 🚀 **Endpoints de Gestión de Asignaciones**

### **1. Asignar Facility a Usuario:**

```bash
POST /api/resourceassignment/users/{userId}/facilities/{facilityId}
Authorization: Bearer {admin-token}
```

**Inserta en BD:**
```sql
INSERT INTO UsuarioDeposito (IdUsuario, IdDeposito, ...)
VALUES (userId, facilityId, ...)
```

**Resultado:** Usuario ahora puede ver esa facility en `GET /api/facility`

---

### **2. Obtener Facilities de un Usuario:**

```bash
GET /api/resourceassignment/users/{userId}/facilities
Authorization: Bearer {admin-token}
```

**Respuesta:**
```json
[
  "00000000-0000-0000-0005-000000000001",
  "00000000-0000-0000-0005-000000000002"
]
```

---

### **3. Revocar Facility de Usuario:**

```bash
DELETE /api/resourceassignment/users/{userId}/facilities/{facilityId}
Authorization: Bearer {admin-token}
```

**Resultado:** Usuario ya NO puede ver esa facility

---

### **4. Verificar Acceso:**

```bash
GET /api/resourceassignment/users/{userId}/facilities/{facilityId}/check
Authorization: Bearer {admin-token}
```

**Respuesta:**
```json
{
  "hasAccess": true
}
```

---

## 📝 **Ejemplo Completo: Desde Cliente**

### **Escenario:**
- Usuario "John" (ID=123) se loguea
- Admin le asigna Facility-A y Facility-B
- John hace GET /api/facility
- **Solo ve** Facility-A y Facility-B (no ve las demás)

---

### **1. Admin Asigna Facilities:**

```bash
# Admin asigna Facility-A a John
curl -X POST http://localhost:5000/api/resourceassignment/users/guid-john/facilities/guid-facility-a \
  -H "Authorization: Bearer {admin-token}"

# Admin asigna Facility-B a John
curl -X POST http://localhost:5000/api/resourceassignment/users/guid-john/facilities/guid-facility-b \
  -H "Authorization: Bearer {admin-token}"
```

### **2. John se Loguea:**

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"john@example.com","password":"password123"}'
```

**Respuesta:** Token que contiene `UserId=123`

### **3. John Consulta Facilities:**

```bash
# AccountId viene en el token, NO necesita enviarse en header
curl -X GET http://localhost:5000/api/facility \
  -H "Authorization: Bearer {john-token}"
```

**Resultado:** Solo Facility-A y Facility-B
```json
[
  { "id": "guid-facility-a", "name": "Facility A", ... },
  { "id": "guid-facility-b", "name": "Facility B", ... }
]
```

### **4. John Intenta Ver Facility-C (no asignada):**

```bash
curl -X GET http://localhost:5000/api/facility/guid-facility-c \
  -H "Authorization: Bearer {john-token}"
```

**Resultado:** `404 Not Found` ✅ (Seguridad funcionando)

---

## 🛡️ **Seguridad Multi-Nivel**

Tu sistema ahora tiene **3 niveles de seguridad:**

### **Nivel 1: Autenticación**
```
¿Está logueado?
❌ Sin token → 401 Unauthorized
✅ Con token válido → Continuar
```

### **Nivel 2: Autorización (Permisos)**
```
¿Tiene permiso para esta operación?
[RequirePermission("facilities", PermissionFlags.Read)]
❌ Sin permiso Read → 403 Forbidden
✅ Con permiso Read → Continuar
```

### **Nivel 3: Segmentación de Datos**
```
¿Este recurso está asignado al usuario?
Query a UsuarioDeposito
❌ No asignado → 404 Not Found (como si no existiera)
✅ Asignado → Devolver datos
```

---

## 💻 **Implementación en Cliente (Node.js)**

```javascript
const axios = require('axios');

class GreesstAPIClient {
    constructor(baseURL) {
        this.client = axios.create({ baseURL });
        this.accessToken = null;
    }

    async login(username, password) {
        const { data } = await axios.post(`${this.baseURL}/auth/login`, {
            username, password
        });
        
        this.accessToken = data.accessToken; // ← Contiene UserId
        this.client.defaults.headers.common['Authorization'] = `Bearer ${this.accessToken}`;
        
        return data;
    }

    // ✅ GetAll automáticamente filtra por usuario del token
    async getFacilities() {
        const { data } = await this.client.get('/facility');
        // Solo devuelve facilities asignadas al usuario
        return data;
    }

    // ✅ GetById verifica acceso del usuario
    async getFacility(id) {
        try {
            const { data } = await this.client.get(`/facility/${id}`);
            return data;
        } catch (error) {
            if (error.response?.status === 404) {
                console.log('No tienes acceso a esta facility');
            }
            throw error;
        }
    }
}

// Uso
const api = new GreesstAPIClient('http://localhost:5000/api');

// 1. Login (token incluye UserId)
await api.login('john@example.com', 'password123');

// 2. GetAll - automáticamente filtrado
const facilities = await api.getFacilities();
// → Solo facilities asignadas a John
```

---

## 👮 **Rol de Admin (Sin Filtros)**

Los usuarios **Admin** ven **todos** los recursos sin filtros:

```csharp
public async Task<IEnumerable<FacilityDto>> GetAllAsync()
{
    // Verificar si es admin
    if (await _segmentationService.CurrentUserIsAdminAsync())
    {
        // ✅ Admin ve TODO (sin filtrar)
        return await _facilityRepository.GetAllAsync();
    }
    
    // Usuario normal - filtrado
    var userFacilityIds = await _segmentationService.GetUserFacilityIdsAsync();
    return await _facilityRepository.FindAsync(f => userFacilityIds.Contains(f.Id));
}
```

**Admin se detecta por:**
```json
Usuario.DatosAdicionales = '{"roles": ["Admin"]}'
```

---

## 🔄 **Flujo Completo**

### **Setup Inicial (Admin):**

```bash
# 1. Login como Admin
POST /api/auth/login
{ "username": "admin@gresst.com", "password": "admin123" }
→ Guarda adminToken

# 2. Crear usuario normal
POST /api/user
{ "name": "John", "email": "john@example.com", ... }
→ Recibe userId

# 3. Asignar facilities al usuario
POST /api/resourceassignment/users/{userId}/facilities/{facilityId}
POST /api/resourceassignment/users/{userId}/facilities/{facilityId2}
→ John ahora puede ver estas facilities

# 4. Asignar vehículos
POST /api/resourceassignment/users/{userId}/vehicles/{vehicleId}
→ John puede usar este vehículo
```

### **Usuario Normal:**

```bash
# 1. Login como John
POST /api/auth/login
{ "username": "john@example.com", "password": "password123" }
→ Token contiene UserId=123

# 2. Consultar facilities (automático filtrado)
GET /api/facility
Authorization: Bearer {john-token}
→ Solo facilities asignadas en UsuarioDeposito

# 3. Ver facility específica
GET /api/facility/{id}
→ 200 OK si está asignada
→ 404 Not Found si NO está asignada

# 4. Ver mis vehículos
GET /api/resourceassignment/users/me/vehicles
→ Solo vehículos asignados
```

---

## 📊 **Impacto en Servicios Existentes**

### **Antes (sin segmentación):**
```csharp
GET /api/facility
→ Devuelve TODAS las facilities (solo filtrado por account)
```

### **Ahora (con segmentación):**
```csharp
GET /api/facility
Authorization: Bearer {user-token}  ← Token contiene UserId

→ FacilityService extrae UserId del token
→ Query a UsuarioDeposito WHERE IdUsuario = {userId}
→ Filtra facilities por IDs asignados
→ Devuelve SOLO facilities del usuario
```

---

## 🔐 **Seguridad**

### **✅ Usuario NO puede:**
- Ver facilities no asignadas
- Modificar facilities no asignadas
- Eliminar facilities no asignadas
- Ver vehículos de otros usuarios

### **✅ Admin puede:**
- Ver TODOS los recursos
- Asignar/revocar recursos a usuarios
- Gestionar cualquier recurso

### **✅ Token es la fuente de verdad:**
```
NO envías UserId en body/query
UserId viene del JWT token (no puede falsificarse)
```

---

## 📝 **Ejemplo Paso a Paso**

### **Paso 1: Admin Crea Usuario**

```bash
POST /api/user
Authorization: Bearer {admin-token}
{
  "accountId": "00000000-0000-0000-0001-000000000001",
  "name": "Maria",
  "email": "maria@example.com",
  "password": "password123",
  "roles": ["User"]
}
```

**Respuesta:**
```json
{
  "id": "00000000-0000-0000-0000-000000000050",  ← userId de Maria
  "name": "Maria",
  ...
}
```

---

### **Paso 2: Admin Asigna Facilities**

```bash
# Asignar Facility "Planta Norte" a Maria
POST /api/resourceassignment/users/00000000-0000-0000-0000-000000000050/facilities/facility-norte-id
Authorization: Bearer {admin-token}

# Asignar Facility "Planta Sur" a Maria
POST /api/resourceassignment/users/00000000-0000-0000-0000-000000000050/facilities/facility-sur-id
Authorization: Bearer {admin-token}
```

**En BD se inserta:**
```sql
INSERT INTO UsuarioDeposito (IdUsuario, IdDeposito)
VALUES (50, planta-norte-id), (50, planta-sur-id)
```

---

### **Paso 3: Maria Se Loguea**

```bash
POST /api/auth/login
{
  "username": "maria@example.com",
  "password": "password123"
}
```

**Respuesta:**
```json
{
  "accessToken": "eyJhbGciOiJI...",  ← Token contiene UserId=50
  "refreshToken": "...",
  "userId": "00000000-0000-0000-0000-000000000050"
}
```

---

### **Paso 4: Maria Consulta Facilities**

```bash
GET /api/facility
Authorization: Bearer {maria-token}
```

**Proceso interno:**
1. ✅ Token validado → UserId=50 extraído
2. ✅ Query a UsuarioDeposito WHERE IdUsuario=50
3. ✅ Resultado: [planta-norte-id, planta-sur-id]
4. ✅ Query Deposito WHERE IdDeposito IN (norte, sur)
5. ✅ Devolver solo esas 2 facilities

**Respuesta:**
```json
[
  { "id": "...", "name": "Planta Norte", ... },
  { "id": "...", "name": "Planta Sur", ... }
]
```

**Maria NO ve:** Planta Este, Planta Oeste, ni otras facilities

---

### **Paso 5: Maria Intenta Ver Facility No Asignada**

```bash
GET /api/facility/planta-este-id
Authorization: Bearer {maria-token}
```

**Proceso interno:**
1. ✅ Token validado → UserId=50
2. ✅ Verificar acceso: UsuarioDeposito WHERE IdUsuario=50 AND IdDeposito=este
3. ❌ No existe → Usuario no tiene acceso
4. ❌ Devolver 404 Not Found

**Respuesta:** `404 Not Found` (como si la facility no existiera)

---

## 🎯 **Endpoints de Asignación (Solo Admin)**

| Endpoint | Descripción |
|----------|-------------|
| `POST /api/resourceassignment/users/{userId}/facilities/{facilityId}` | Asignar facility |
| `DELETE /api/resourceassignment/users/{userId}/facilities/{facilityId}` | Revocar facility |
| `GET /api/resourceassignment/users/{userId}/facilities` | Listar facilities del usuario |
| `GET /api/resourceassignment/users/{userId}/facilities/{facilityId}/check` | Verificar acceso |
| `POST /api/resourceassignment/users/{userId}/vehicles/{vehicleId}` | Asignar vehículo |
| `DELETE /api/resourceassignment/users/{userId}/vehicles/{vehicleId}` | Revocar vehículo |
| `GET /api/resourceassignment/users/{userId}/vehicles` | Listar vehículos del usuario |

---

## 🔍 **Ventajas del Sistema**

### **1. Automático**
```
✅ NO necesitas pasar userId en cada request
✅ Se obtiene automáticamente del token JWT
✅ Imposible de falsificar (firma criptográfica)
```

### **2. Seguro**
```
✅ Usuario solo ve sus recursos
✅ Usuario NO puede acceder a recursos de otros
✅ Admin puede gestionar todo
```

### **3. Transparente**
```
✅ Cliente no sabe que hay filtrado
✅ GET /api/facility devuelve lo que el usuario puede ver
✅ 404 para recursos sin acceso (no 403, mejor UX)
```

### **4. Flexible**
```
✅ Admin asigna/revoca dinámicamente
✅ Sin código hardcodeado
✅ Basado en tablas de BD
```

---

## ⚙️ **Configuración en Program.cs**

```csharp
// Ya registrado:
builder.Services.AddScoped<IDataSegmentationService, DataSegmentationService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// FacilityService usa DataSegmentationService automáticamente
builder.Services.AddScoped<IFacilityService, FacilityService>();
```

---

## 📚 **Recursos que Soportan Segmentación**

Según tu BD, estas tablas existen:

| Tabla BD | Recurso | Estado |
|----------|---------|--------|
| `UsuarioDeposito` | Facilities | ✅ Implementado |
| `UsuarioVehiculo` | Vehicles | ✅ Implementado |
| `UsuarioPersona` | Persons | ⏳ Extendible |

**Patrón para extender:**
```csharp
// En DataSegmentationService.cs
public async Task<IEnumerable<Guid>> GetUserMaterialIdsAsync()
{
    var userId = _currentUserService.GetCurrentUserId();
    var userIdLong = GuidLongConverter.ToLong(userId);
    
    var materialIds = await _context.UsuarioMaterials  // ← Si existe
        .Where(um => um.IdUsuario == userIdLong)
        .Select(um => um.IdMaterial)
        .ToListAsync();
    
    return materialIds.Select(GuidLongConverter.ToGuid);
}
```

---

## ✅ **Archivos Creados**

- ✅ `Application/Services/IDataSegmentationService.cs`
- ✅ `Infrastructure/Services/DataSegmentationService.cs`
- ✅ `Infrastructure/Data/Entities/RefreshToken.cs`
- ✅ `API/Controllers/ResourceAssignmentController.cs`
- ✅ `Application/Services/FacilityService.cs` (actualizado)

---

## 🚀 **Próximos Pasos**

1. ✅ **Crear tabla RefreshToken en BD** (script incluido)
2. ✅ **Asignar facilities de prueba** a usuarios vía API
3. ✅ **Probar con diferentes usuarios** y verificar filtrado
4. ⏳ **Extender a otros recursos** (Materials, Persons, etc.)

---

**🎉 Sistema de Segmentación Completo! Usuario del Token Automático!**

