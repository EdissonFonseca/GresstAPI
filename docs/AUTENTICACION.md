# 🔐 Sistema de Autenticación Dual - Gresst API

## 📋 Descripción

El sistema implementa **dos estrategias de autenticación**:

1. **Base de Datos** (por defecto) - Usando tabla `Usuario` de SQL Server
2. **Proveedor Externo** - Auth0, Azure AD, Google, Firebase, etc.

---

## 🏗️ Arquitectura

```
Infrastructure/Authentication/
├── Models/
│   ├── AuthenticationResult.cs   ← Resultado de login
│   └── LoginRequest.cs            ← DTOs de peticiones
├── IAuthenticationService.cs      ← Interface común
├── DatabaseAuthenticationService.cs   ← Autenticación BD
├── ExternalAuthenticationService.cs   ← Autenticación externa
└── AuthenticationServiceFactory.cs    ← Selector de estrategia
```

**✅ User NO está en Domain** - Solo en Infrastructure/Authentication

---

## ⚙️ Configuración

### **appsettings.json**

```json
{
  "Authentication": {
    "Mode": "Database",  // "Database" o "External"
    "JwtSecret": "gresst-jwt-secret-key-change-in-production-must-be-at-least-32-characters-long",
    "AccessTokenExpirationMinutes": "15",   // ⭐ AccessToken corto (15 min)
    "RefreshTokenExpirationDays": "7",      // ⭐ RefreshToken largo (7 días)
    "ExternalProvider": {
      "Url": "https://your-auth-provider.com",
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret"
    }
  }
}
```

**Valores Recomendados:**
- **Desarrollo:** AccessToken=60min, RefreshToken=30días
- **Producción:** AccessToken=15min, RefreshToken=7días
- **Alta Seguridad:** AccessToken=5min, RefreshToken=1día

### **Cambiar Proveedor**

Para cambiar entre Base de Datos y Proveedor Externo, solo modifica:

```json
"Mode": "Database"  ← Usar BD
"Mode": "External"  ← Usar Auth0/Azure AD/etc.
```

---

## 🔑 Endpoints de Autenticación

### **1. Login (usa el proveedor configurado)**

```bash
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin@example.com",
  "password": "password123"
}
```

**Respuesta:** ⭐ Ahora incluye **AccessToken Y RefreshToken**

```json
{
  "success": true,
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "Q3VzdG9tLXJlZnJlc2gtdG9rZW4...",
  "userId": "00000000-0000-0000-0000-000000000001",
  "accountId": "00000000-0000-0000-0001-000000000001",
  "username": "Admin",
  "email": "admin@example.com",
  "roles": ["Admin", "User"],
  "accessTokenExpiresAt": "2025-11-12T14:15:00Z",   // 15 minutos
  "refreshTokenExpiresAt": "2025-11-19T14:00:00Z"   // 7 días
}
```

**⚠️ IMPORTANTE:** 
- Guarda **ambos tokens** en el cliente
- Usa `accessToken` para todos los requests (Authorization: Bearer)
- Usa `refreshToken` cuando `accessToken` expire

**Cookie:** Si el cliente es un navegador, la API también envía el access token en una cookie (`Set-Cookie`). Los endpoints protegidos aceptan el token por **Authorization: Bearer** o por **cookie** (nombre por defecto: `access_token`). Si no llega Bearer, se lee el token desde la cookie.

---

### **2. Login específico (Base de Datos)**

```bash
POST /api/auth/login/database
Content-Type: application/json

{
  "username": "admin@example.com",
  "password": "password123"
}
```

---

### **3. Login específico (Proveedor Externo)**

```bash
POST /api/auth/login/external
Content-Type: application/json

{
  "username": "user@gmail.com",
  "password": "password123"
}
```

---

### **4. Validar Token**

```bash
POST /api/auth/validate
Content-Type: application/json

{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

### **5. Refresh Token (⭐ NUEVO)**

**Cuando tu AccessToken expire, renuévalo con el RefreshToken:**

```bash
POST /api/auth/refresh
Content-Type: application/json

{
  "accessToken": "expired-access-token",
  "refreshToken": "valid-refresh-token"
}
```

**Respuesta:** Nuevos tokens
```json
{
  "success": true,
  "accessToken": "NEW-ACCESS-TOKEN...",
  "refreshToken": "NEW-REFRESH-TOKEN...",
  "userId": "...",
  "accountId": "...",
  "accessTokenExpiresAt": "2025-11-12T14:30:00Z",
  "refreshTokenExpiresAt": "2025-11-19T14:15:00Z"
}
```

**💡 Tu cliente debe:**
1. Detectar error 401 (Unauthorized)
2. Llamar `/api/auth/refresh` automáticamente
3. Guardar los nuevos tokens
4. Reintentar el request original

Ver `REFRESH-TOKEN.md` para ejemplos completos de implementación.

---

### **6. Logout**

```bash
POST /api/auth/logout
Authorization: Bearer {accessToken}
Content-Type: application/json

{
  "refreshToken": "token-to-revoke"
}
```

**Esto revoca el RefreshToken** para que no se pueda usar más.

---

### **6. Obtener Usuario Actual**

```bash
GET /api/auth/me
Authorization: Bearer {token}
```

**Respuesta:**

```json
{
  "userId": "00000000-0000-0000-0000-000000000001",
  "username": "Admin",
  "accountId": "00000000-0000-0000-0001-000000000001",
  "email": "admin@example.com",
  "roles": ["Admin", "User"],
  "isAuthenticated": true
}
```

---

## 🔒 Usar Autenticación en Otros Endpoints

### **Swagger**

1. Hacer login en `/api/auth/login`
2. Copiar el `token` de la respuesta
3. Click en **🔓 Authorize** en Swagger
4. Escribir: `Bearer {token}`
5. Click en **Authorize**
6. ✅ Todos los endpoints protegidos ahora funcionarán

### **Postman: usar solo cookie (sin Bearer)**

Para probar que la API acepta el token desde la cookie:

1. **Login:** `POST /api/v1/authentication/login` con body `{ "username": "...", "password": "..." }`. La respuesta incluye `accessToken` y además envía `Set-Cookie` con el token.
2. **Misma base URL:** Asegúrate de que la siguiente petición use **exactamente el mismo host y puerto** (ej. `http://localhost:49847`). Si usas `http://127.0.0.1:49847` en una y `http://localhost:49847` en otra, Postman no enviará la cookie.
3. **Sin Authorization:** En la petición a `GET /api/v1/me/profile` (o cualquier protegido), en la pestaña **Authorization** elige **No Auth**. Postman enviará la cookie guardada para ese dominio.
4. Si aun así obtienes 401, envía el token manualmente en la pestaña **Headers**: clave `Cookie`, valor `access_token=eyJ...` (pega el `accessToken` de la respuesta del login).

Revisa los logs en la carpeta `logs/` (archivo `log-YYYYMMDD.txt`): verás líneas `[JWT] Token from cookie...` si la cookie llegó, o `[JWT] No Bearer and no cookie...` si no. Si aparece `[JWT] Authentication failed` es que el token llegó pero la validación falló (token expirado, firma incorrecta, etc.).

### **cURL**

```bash
# 1. Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin@example.com",
    "password": "password123"
  }'

# Respuesta: Copiar el token

# 2. Usar el token en otras peticiones
curl -X GET http://localhost:5000/api/facility \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
# AccountId viene en el token, NO enviar en header
```

### **Axios (Node.js)**

```javascript
const axios = require('axios');

// 1. Login
const loginResponse = await axios.post('http://localhost:5000/api/auth/login', {
  username: 'admin@example.com',
  password: 'password123'
});

const token = loginResponse.data.token;
const accountId = loginResponse.data.accountId;

// 2. Configurar cliente con token
const apiClient = axios.create({
  baseURL: 'http://localhost:5000/api',
  headers: {
    'Authorization': `Bearer ${token}`
    // AccountId viene en el token JWT
  }
});

// 3. Usar en todas las peticiones
const facilities = await apiClient.get('/facility');
console.log(facilities.data);
```

---

## 👤 Tabla Usuario en BD

La autenticación usa la tabla `Usuario` con estos campos:

```sql
Usuario
├── IdUsuario       (long, PK)
├── IdCuenta        (long, FK → Cuenta)
├── Nombre          (string)
├── Apellido        (string?)
├── Correo          (string)
├── Clave           (string?, password hash)
├── IdEstado        (string: "A"=Activo, "I"=Inactivo)
├── IdPersona       (string?, FK → Persona)
└── DatosAdicionales (JSON: {"roles": ["Admin", "User"]})
```

**Login acepta:**
- `Correo` o `Nombre` como username
- Solo usuarios con `IdEstado = "A"` pueden autenticarse

---

## 🔐 Password Hashing

⚠️ **IMPORTANTE EN PRODUCCIÓN:**

Actualmente usa SHA256 simple. **DEBE cambiar a BCrypt o Argon2**:

```bash
# Instalar BCrypt
dotnet add package BCrypt.Net-Next

# En DatabaseAuthenticationService.cs
private bool VerifyPassword(string password, string? hashedPassword)
{
    return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
}

private string HashPassword(string password)
{
    return BCrypt.Net.BCrypt.HashPassword(password);
}
```

---

## 🌐 Proveedor Externo (Auth0 / Azure AD)

### **Auth0 Example**

```json
{
  "Authentication": {
    "Mode": "External",
    "ExternalProvider": {
      "Url": "https://YOUR_DOMAIN.auth0.com",
      "ClientId": "your_client_id",
      "ClientSecret": "your_client_secret"
    }
  }
}
```

### **Azure AD Example**

```json
{
  "Authentication": {
    "Mode": "External",
    "ExternalProvider": {
      "Url": "https://login.microsoftonline.com/{tenant}/v2.0",
      "ClientId": "your_azure_app_id",
      "ClientSecret": "your_azure_secret"
    }
  }
}
```

---

## 🎯 Roles y Permisos

Los roles se almacenan en `Usuario.DatosAdicionales` como JSON:

```json
{
  "roles": ["Admin", "Manager", "User"]
}
```

### **Proteger Endpoints por Rol**

```csharp
[HttpGet("admin-only")]
[Authorize(Roles = "Admin")]
public ActionResult AdminOnly()
{
    return Ok("Solo administradores pueden ver esto");
}

[HttpGet("manager-or-admin")]
[Authorize(Roles = "Admin,Manager")]
public ActionResult ManagerOrAdmin()
{
    return Ok("Solo managers o admins");
}
```

---

## ✅ Testing

### **Test con usuario de BD**

1. Crear usuario en tabla `Usuario`:

```sql
INSERT INTO Usuario (IdCuenta, Nombre, Correo, Clave, IdEstado, DatosAdicionales)
VALUES (
  1,
  'Admin',
  'admin@gresst.com',
  'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=',  -- SHA256 de "admin123"
  'A',
  '{"roles": ["Admin", "User"]}'
);
```

2. Login:

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin@gresst.com",
    "password": "admin123"
  }'
```

---

## 🔄 Sincronización con Proveedor Externo

Cuando un usuario se autentica con proveedor externo por primera vez:

1. ✅ Se valida con Auth0/Azure AD/etc.
2. ✅ Se busca en BD local por `Correo`
3. ❌ Si no existe → Se crea automáticamente en tabla `Usuario`
4. ✅ Se devuelve token y datos del usuario

**Ventaja:** Todos los usuarios (BD + externos) están en la misma tabla `Usuario`.

---

## 🚀 Próximos Pasos

1. ✅ Cambiar hashing a BCrypt/Argon2
2. ✅ Implementar refresh tokens
3. ✅ Token blacklist para logout real
4. ✅ Rate limiting para login
5. ✅ 2FA (Two-Factor Authentication)
6. ✅ Email verification
7. ✅ Password reset

---

## 📞 Soporte

- **User** NO es del dominio de negocio
- **Person** SÍ es del dominio (generador, transportista, etc.)
- La autenticación está aislada en `Infrastructure/Authentication`
- JWT es stateless, el servidor no guarda sesiones

¿Preguntas? Consulta `Program.cs` para ver la configuración completa.

