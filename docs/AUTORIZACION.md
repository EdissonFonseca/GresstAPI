# 🔐 Sistema de Autorización - Permisos por Opciones

## ✅ Implementado

Sistema completo de autorización basado en permisos granulares (CRUD) para opciones del sistema.

---

## 🏗️ Arquitectura

```
✅ APPLICATION (Interfaces + DTOs):
   ├── IPermissionService.cs
   ├── PermissionDto.cs
   ├── PermissionFlags enum (C, R, U, D)
   └── PermissionHelper (parse/toString)

✅ INFRASTRUCTURE (Implementación):
   ├── PermissionService.cs
   └── Usa tablas: UsuarioOpcion + Opcion

✅ API (Controllers + Attributes):
   ├── PermissionController.cs (CRUD de permisos)
   └── RequirePermissionAttribute.cs (proteger endpoints)
```

---

## 📊 **Modelo de Base de Datos**

### **Tabla: Opcion**
```sql
Opcion:
├── IdOpcion (string, PK) → "facilities", "wastes", "reports"
├── IdOpcionSuperior (string?) → Jerarquía de módulos
├── Descripcion (string?) → "Gestión de Instalaciones"
├── Configurable (bool) → Tiene settings?
└── Settings (JSON?) → Configuración adicional
```

### **Tabla: UsuarioOpcion**
```sql
UsuarioOpcion:
├── IdUsuario (long, PK)
├── IdOpcion (string, PK)
├── Habilitado (bool) → ¿Usuario tiene acceso?
├── Permisos (string) → "CRUD" (C=Create, R=Read, U=Update, D=Delete)
└── Settings (string?) → Config específica usuario-opción
```

---

## 🎯 **PermissionFlags Enum**

```csharp
[Flags]
public enum PermissionFlags
{
    None = 0,
    Create = 1,   // C - Crear
    Read = 2,     // R - Leer
    Update = 4,   // U - Actualizar
    Delete = 8,   // D - Eliminar
    All = 15      // CRUD - Todos los permisos
}
```

### **Ejemplos:**
```csharp
PermissionFlags.Read                        = 2    → "R"
PermissionFlags.Create | PermissionFlags.Read = 3  → "CR"
PermissionFlags.All                         = 15   → "CRUD"
```

---

## 🔑 **Endpoints API**

### **1. Opciones del Sistema**

#### GET /api/permission/options
```bash
# Obtener todas las opciones del sistema
curl -X GET http://localhost:5000/api/permission/options \
  -H "Authorization: Bearer {token}"
```

**Respuesta:**
```json
[
  {
    "id": "facilities",
    "parentId": null,
    "description": "Gestión de Instalaciones",
    "isConfigurable": false
  },
  {
    "id": "wastes",
    "parentId": null,
    "description": "Gestión de Residuos",
    "isConfigurable": true
  }
]
```

---

### **2. Permisos de Usuario**

#### GET /api/permission/users/{userId}/permissions
```bash
# Obtener permisos de un usuario (solo Admin)
curl -X GET http://localhost:5000/api/permission/users/guid-user-id/permissions \
  -H "Authorization: Bearer {token}"
```

**Respuesta:**
```json
[
  {
    "userId": "00000000-0000-0000-0000-000000000001",
    "userName": "John Doe",
    "optionId": "facilities",
    "optionDescription": "Gestión de Instalaciones",
    "isEnabled": true,
    "permissions": 15,  // All = CRUD
    "settings": null
  },
  {
    "userId": "00000000-0000-0000-0000-000000000001",
    "userName": "John Doe",
    "optionId": "wastes",
    "optionDescription": "Gestión de Residuos",
    "isEnabled": true,
    "permissions": 2,  // Read only
    "settings": null
  }
]
```

---

#### GET /api/permission/me/permissions
```bash
# Obtener permisos del usuario actual
curl -X GET http://localhost:5000/api/permission/me/permissions \
  -H "Authorization: Bearer {token}"
```

---

### **3. Asignar Permisos**

#### POST /api/permission/assign
```bash
curl -X POST http://localhost:5000/api/permission/assign \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "00000000-0000-0000-0000-000000000001",
    "optionId": "facilities",
    "isEnabled": true,
    "permissions": 15  // All = CRUD
  }'
```

**Permisos comunes:**
- `15` = CRUD (todos)
- `3` = CR (crear y leer)
- `2` = R (solo lectura)
- `6` = RU (leer y actualizar)

---

### **4. Actualizar Permisos**

#### PUT /api/permission/users/{userId}/permissions/{optionId}
```bash
curl -X PUT http://localhost:5000/api/permission/users/guid-user/permissions/facilities \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "00000000-0000-0000-0000-000000000001",
    "optionId": "facilities",
    "isEnabled": true,
    "permissions": 2  // Solo lectura
  }'
```

---

### **5. Revocar Permisos**

#### DELETE /api/permission/users/{userId}/permissions/{optionId}
```bash
curl -X DELETE http://localhost:5000/api/permission/users/guid-user/permissions/facilities \
  -H "Authorization: Bearer {token}"
```

---

### **6. Verificar Permiso**

#### GET /api/permission/check?optionId={id}&permission={flag}
```bash
# Verificar si usuario actual tiene permiso Create en facilities
curl -X GET "http://localhost:5000/api/permission/check?optionId=facilities&permission=1" \
  -H "Authorization: Bearer {token}"
```

**Respuesta:**
```json
{
  "hasPermission": true
}
```

---

## 🛡️ **Proteger Endpoints con RequirePermission**

### **Uso en Controllers:**

```csharp
using Gresst.API.Authorization;
using Gresst.Application.DTOs;

[ApiController]
[Route("api/[controller]")]
public class FacilityController : ControllerBase
{
    // ✅ Requiere permiso de LECTURA en "facilities"
    [HttpGet]
    [RequirePermission("facilities", PermissionFlags.Read)]
    public async Task<ActionResult<IEnumerable<FacilityDto>>> GetAll()
    {
        // Solo usuarios con permiso Read en facilities pueden acceder
    }

    // ✅ Requiere permiso de CREACIÓN en "facilities"
    [HttpPost]
    [RequirePermission("facilities", PermissionFlags.Create)]
    public async Task<ActionResult<FacilityDto>> Create([FromBody] CreateFacilityDto dto)
    {
        // Solo usuarios con permiso Create en facilities pueden acceder
    }

    // ✅ Requiere permiso de ACTUALIZACIÓN en "facilities"
    [HttpPut("{id}")]
    [RequirePermission("facilities", PermissionFlags.Update)]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateFacilityDto dto)
    {
        // Solo usuarios con permiso Update en facilities pueden acceder
    }

    // ✅ Requiere permiso de ELIMINACIÓN en "facilities"
    [HttpDelete("{id}")]
    [RequirePermission("facilities", PermissionFlags.Delete)]
    public async Task<ActionResult> Delete(Guid id)
    {
        // Solo usuarios con permiso Delete en facilities pueden acceder
    }
}
```

---

## 🔍 **Verificar Permisos en Código**

### **Desde un Service:**

```csharp
public class FacilityService
{
    private readonly IPermissionService _permissionService;

    public async Task<bool> CanUserCreateFacility()
    {
        // Verificar si usuario actual puede crear facilities
        return await _permissionService.CurrentUserHasPermissionAsync(
            "facilities", 
            PermissionFlags.Create
        );
    }

    public async Task<FacilityDto> CreateFacility(CreateFacilityDto dto)
    {
        // Verificar permisos manualmente
        var hasPermission = await _permissionService.CurrentUserHasPermissionAsync(
            "facilities", 
            PermissionFlags.Create
        );

        if (!hasPermission)
            throw new UnauthorizedAccessException("No tienes permiso para crear instalaciones");

        // Crear facility...
    }
}
```

---

## 📝 **Ejemplos Completos**

### **Ejemplo 1: Asignar Todos los Permisos**

```bash
# Usuario puede hacer TODO (CRUD) en módulo "wastes"
curl -X POST http://localhost:5000/api/permission/assign \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "00000000-0000-0000-0000-000000000001",
    "optionId": "wastes",
    "isEnabled": true,
    "permissions": 15
  }'
```

---

### **Ejemplo 2: Solo Lectura**

```bash
# Usuario solo puede VER (Read) módulo "reports"
curl -X POST http://localhost:5000/api/permission/assign \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "00000000-0000-0000-0000-000000000001",
    "optionId": "reports",
    "isEnabled": true,
    "permissions": 2
  }'
```

---

### **Ejemplo 3: Crear y Leer (CR)**

```bash
# Usuario puede CREAR y LEER, pero NO actualizar ni eliminar
curl -X POST http://localhost:5000/api/permission/assign \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "00000000-0000-0000-0000-000000000001",
    "optionId": "certificates",
    "isEnabled": true,
    "permissions": 3
  }'
```

---

## 🌳 **Jerarquía de Opciones**

Las opciones pueden tener jerarquía usando `IdOpcionSuperior`:

```
facilities (padre)
├── facilities.create (hijo)
├── facilities.view (hijo)
└── facilities.delete (hijo)

wastes (padre)
├── wastes.generate (hijo)
├── wastes.transport (hijo)
└── wastes.dispose (hijo)
```

```bash
# Obtener opciones hijas de "facilities"
GET /api/permission/options/facilities/children
```

---

## ✅ **Beneficios del Sistema**

### 1. **Granularidad CRUD**
- Control fino por cada operación (Create, Read, Update, Delete)
- No solo "tiene acceso" o "no tiene acceso"

### 2. **Flexible**
```
Usuario A: CRUD completo en facilities
Usuario B: Solo Read en facilities
Usuario C: Create + Read en facilities
```

### 3. **Fácil de Usar**
```csharp
// Una línea protege el endpoint
[RequirePermission("facilities", PermissionFlags.Create)]
```

### 4. **No Contamina Domain**
- Permisos están en Infrastructure (✅ correcto)
- Domain solo tiene lógica de negocio

---

## 🚀 **Próximos Pasos**

1. ✅ **Completado:** Sistema básico de permisos
2. ⏳ **Recomendado:** Caché de permisos (performance)
3. ⏳ **Recomendado:** Permisos por grupos/roles
4. ⏳ **Recomendado:** Audit log de cambios de permisos
5. ⏳ **Recomendado:** UI para gestión visual de permisos

---

## 📚 **Archivos Creados**

- ✅ `Application/DTOs/PermissionDto.cs`
- ✅ `Application/Services/IPermissionService.cs`
- ✅ `Infrastructure/Services/PermissionService.cs`
- ✅ `API/Authorization/RequirePermissionAttribute.cs`
- ✅ `API/Controllers/PermissionController.cs`

---

**🎉 Sistema de Autorización Completo y Funcional!**

