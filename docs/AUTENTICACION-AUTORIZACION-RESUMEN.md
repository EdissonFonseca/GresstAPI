# 🔐 Autenticación y Autorización - Resumen Ejecutivo

## 📋 **Sistema Completo Implementado**

### ✅ **Autenticación (¿Quién eres?)**
- Login con usuario/contraseña (BD o proveedor externo)
- JWT AccessToken (15 min) + RefreshToken (7 días)
- Refresh automático cuando AccessToken expire
- Logout con revocación de RefreshToken

### ✅ **Autorización (¿Qué puedes hacer?)**
- Permisos granulares CRUD por opción del sistema
- Roles (Admin, User, etc.)
- Atributo `[RequirePermission]` para proteger endpoints
- CRUD completo para gestión de permisos

---

## 🔄 **Flujo Completo**

```
1. LOGIN
   Cliente → POST /api/auth/login {username, password}
   ←─────── {accessToken, refreshToken}
   
   Cliente guarda:
   - accessToken (usar en requests, expira en 15 min)
   - refreshToken (renovar accessToken, expira en 7 días)
   - accountId viene en el token, no necesita guardarse por separado

2. USAR API
   Cliente → GET /api/facility
             Authorization: Bearer {accessToken}
             (AccountId viene en el token, NO enviar en header)
   ←─────── [facilities data]

3. ACCESS TOKEN EXPIRA (después de 15 min)
   Cliente → GET /api/facility
             Authorization: Bearer {expired-access-token}
   ←─────── 401 Unauthorized
   
   Cliente detecta 401:
   Cliente → POST /api/auth/refresh 
             {accessToken, refreshToken}
   ←─────── {NEW accessToken, NEW refreshToken}
   
   Cliente guarda nuevos tokens
   
   Cliente → GET /api/facility (reintenta)
             Authorization: Bearer {new-access-token}
   ←─────── [facilities data] ✅

4. LOGOUT
   Cliente → POST /api/auth/logout
             {refreshToken}
   ←─────── RefreshToken revocado en BD
   
   Cliente elimina tokens del storage
```

---

## 🏗️ **Arquitectura**

### **NO están en Domain (correcto):**
```
❌ User → No es del negocio de residuos
❌ Usuario → Solo autenticación
❌ Permisos → Solo autorización
❌ RefreshToken → Técnico
```

### **SÍ están en Domain (correcto):**
```
✅ Account → Organización del negocio (genera/opera residuos)
✅ Person → Actor del negocio
✅ Waste, Facility, Management → Entidades core
```

### **Ubicación de Archivos:**

```
Infrastructure/
├── Authentication/          ✅ Login/Logout/RefreshToken
│   ├── DatabaseAuthenticationService.cs
│   ├── ExternalAuthenticationService.cs
│   └── Models/
│       ├── AuthenticationResult.cs
│       └── RefreshTokenRequest.cs
├── Services/                ✅ Gestión usuarios/permisos
│   ├── UserService.cs
│   └── PermissionService.cs
└── Data/Entities/
    ├── Usuario.cs           ✅ Autenticación
    ├── RefreshToken.cs      ✅ Tokens guardados
    ├── UsuarioOpcion.cs     ✅ Permisos usuario-opción
    └── Opcion.cs            ✅ Opciones del sistema

API/
├── Controllers/
│   ├── AuthController.cs    ✅ Login/Logout/Refresh
│   ├── UserController.cs    ✅ CRUD usuarios
│   └── PermissionController.cs ✅ CRUD permisos
└── Authorization/
    └── RequirePermissionAttribute.cs ✅ Proteger endpoints
```

---

## 🔑 **Endpoints Principales**

### **Autenticación:**
| Endpoint | Descripción |
|----------|-------------|
| `POST /api/auth/login` | Login → Obtiene accessToken + refreshToken |
| `POST /api/auth/refresh` | Renovar accessToken con refreshToken |
| `POST /api/auth/logout` | Revocar refreshToken |
| `GET /api/auth/me` | Info básica del token |

### **Usuarios:**
| Endpoint | Descripción |
|----------|-------------|
| `GET /api/user/me` | Perfil completo del usuario actual |
| `GET /api/user/{id}` | Usuario por ID |
| `GET /api/user/account/{accountId}` | Usuarios de una cuenta |
| `PUT /api/user/me` | Actualizar mi perfil |
| `POST /api/user/me/change-password` | Cambiar mi contraseña |

### **Permisos:**
| Endpoint | Descripción |
|----------|-------------|
| `GET /api/permission/options` | Opciones del sistema |
| `GET /api/permission/me/permissions` | Mis permisos |
| `GET /api/permission/users/{id}/permissions` | Permisos de un usuario |
| `POST /api/permission/assign` | Asignar permiso |
| `PUT /api/permission/users/{id}/permissions/{optionId}` | Actualizar permiso |
| `DELETE /api/permission/users/{id}/permissions/{optionId}` | Revocar permiso |
| `GET /api/permission/check` | Verificar si tengo permiso |

---

## 🛡️ **3 Formas de Proteger Endpoints**

### **1. Por Autenticación (¿está logueado?)**
```csharp
[HttpGet]
[Authorize]  // Solo usuarios autenticados
public async Task<ActionResult> GetAll() { }
```

### **2. Por Rol (¿es Admin?)**
```csharp
[HttpDelete("{id}")]
[Authorize(Roles = "Admin")]  // Solo Admins
public async Task<ActionResult> Delete(Guid id) { }
```

### **3. Por Permiso Granular (¿puede crear facilities?)**
```csharp
[HttpPost]
[RequirePermission("facilities", PermissionFlags.Create)]  // Permiso específico
public async Task<ActionResult> Create([FromBody] CreateFacilityDto dto) { }
```

---

## 📝 **Ejemplo Completo: Proteger CRUD de Facilities**

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]  // Base: requiere estar autenticado
public class FacilityController : ControllerBase
{
    [HttpGet]
    [RequirePermission("facilities", PermissionFlags.Read)]
    public async Task<ActionResult<IEnumerable<FacilityDto>>> GetAll() 
    {
        // Solo usuarios con permiso READ en facilities
    }

    [HttpGet("{id}")]
    [RequirePermission("facilities", PermissionFlags.Read)]
    public async Task<ActionResult<FacilityDto>> GetById(Guid id) 
    {
        // Solo usuarios con permiso READ en facilities
    }

    [HttpPost]
    [RequirePermission("facilities", PermissionFlags.Create)]
    public async Task<ActionResult<FacilityDto>> Create([FromBody] CreateFacilityDto dto) 
    {
        // Solo usuarios con permiso CREATE en facilities
    }

    [HttpPut("{id}")]
    [RequirePermission("facilities", PermissionFlags.Update)]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateFacilityDto dto) 
    {
        // Solo usuarios con permiso UPDATE en facilities
    }

    [HttpDelete("{id}")]
    [RequirePermission("facilities", PermissionFlags.Delete)]
    public async Task<ActionResult> Delete(Guid id) 
    {
        // Solo usuarios con permiso DELETE en facilities
    }
}
```

---

## 🗄️ **Setup Inicial en BD**

### **1. Crear tabla RefreshToken:**
```bash
sqlcmd -S server -d QA.Gresst -i CREATE_REFRESH_TOKEN_TABLE.sql
```

### **2. Insertar opciones del sistema:**
```sql
INSERT INTO Opcion (IdOpcion, Descripcion, Configurable) VALUES 
('facilities', 'Gestión de Instalaciones', 0),
('wastes', 'Gestión de Residuos', 0),
('management', 'Gestión de Operaciones', 0),
('reports', 'Reportes y Certificados', 1),
('inventory', 'Inventario', 0),
('users', 'Gestión de Usuarios', 0),
('permissions', 'Gestión de Permisos', 0);
```

### **3. Crear usuario Admin:**
```sql
INSERT INTO Usuario (IdCuenta, Nombre, Correo, Clave, IdEstado, DatosAdicionales)
VALUES (
  1,
  'Admin',
  'admin@gresst.com',
  'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=',  -- SHA256("admin123")
  'A',
  '{"roles": ["Admin"]}'
);
```

### **4. Asignar todos los permisos al Admin:**
```sql
DECLARE @AdminId bigint = (SELECT IdUsuario FROM Usuario WHERE Correo = 'admin@gresst.com');

INSERT INTO UsuarioOpcion (IdUsuario, IdOpcion, Habilitado, Permisos, IdUsuarioCreacion, FechaCreacion)
SELECT @AdminId, IdOpcion, 1, 'CRUD', @AdminId, GETUTCDATE()
FROM Opcion;
```

---

## 🚀 **Testing Rápido**

### **1. Login:**
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin@gresst.com","password":"admin123"}'
```

**Guardar:** `accessToken` y `refreshToken`

### **2. Usar AccessToken:**
```bash
curl -X GET http://localhost:5000/api/facility \
  -H "Authorization: Bearer {accessToken}" \
  -H "X-Account-Id: 00000000-0000-0000-0001-000000000001"
```

### **3. Cuando AccessToken expire:**
```bash
curl -X POST http://localhost:5000/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{"accessToken":"{expired}","refreshToken":"{valid-refresh}"}'
```

**Guardar:** Nuevos `accessToken` y `refreshToken`

---

## 📚 **Documentación Detallada**

- 📖 **AUTENTICACION.md** → Sistema de login/logout completo
- 🔄 **REFRESH-TOKEN.md** → Implementación RefreshToken detallada
- 🛡️ **AUTORIZACION.md** → Sistema de permisos granulares
- 💻 **CLIENT-EXAMPLE-NODEJS.js** → Cliente Node.js completo con auto-refresh

---

## ✅ **Checklist de Implementación**

- [x] ✅ Autenticación dual (BD + Externa)
- [x] ✅ JWT AccessToken
- [x] ✅ RefreshToken con rotación
- [x] ✅ Gestión de usuarios
- [x] ✅ Sistema de permisos granulares CRUD
- [x] ✅ Atributos para proteger endpoints
- [x] ✅ Tabla RefreshToken en BD
- [ ] ⏳ Ejecutar script SQL CREATE_REFRESH_TOKEN_TABLE.sql
- [ ] ⏳ Insertar opciones del sistema
- [ ] ⏳ Crear usuario Admin inicial
- [ ] ⏳ Cambiar hashing a BCrypt (producción)

---

**🎉 Sistema Profesional de Auth y Autorización Completo!**

