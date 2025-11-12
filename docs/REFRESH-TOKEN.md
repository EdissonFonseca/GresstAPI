# 🔄 RefreshToken - Sistema de Tokens de Actualización

## ✅ Implementado

Sistema completo de RefreshToken para renovar AccessToken cuando expire sin volver a pedir credenciales.

---

## 🎯 **¿Por qué RefreshToken?**

### **Problema:**
- AccessToken debe ser de corta duración (15 min) por seguridad
- El usuario NO debe hacer login cada 15 minutos
- Necesitamos renovar el AccessToken sin pedir credenciales

### **Solución:**
```
AccessToken:  Corta duración (15 min) → Usado en cada request
RefreshToken: Larga duración (7 días) → Usado solo para renovar AccessToken
```

---

## 📊 **Flujo Completo**

### **1. Login → Obtener Ambos Tokens**

```bash
POST /api/auth/login
{
  "username": "user@example.com",
  "password": "password123"
}
```

**Respuesta:**
```json
{
  "success": true,
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "Q3VzdG9tLXJlZnJlc2gtdG9rZW4=...",
  "userId": "00000000-0000-0000-0000-000000000001",
  "accountId": "00000000-0000-0000-0001-000000000001",
  "username": "John Doe",
  "email": "user@example.com",
  "roles": ["User"],
  "accessTokenExpiresAt": "2025-11-12T14:15:00Z",    // 15 min
  "refreshTokenExpiresAt": "2025-11-19T14:00:00Z"    // 7 días
}
```

### **2. Cliente Guarda Ambos Tokens**

```javascript
// Node.js / React / Angular / etc.
localStorage.setItem('accessToken', response.accessToken);
localStorage.setItem('refreshToken', response.refreshToken);
localStorage.setItem('accessTokenExpiry', response.accessTokenExpiresAt);
```

---

### **3. Cliente Usa AccessToken en Requests**

```bash
GET /api/facility
Authorization: Bearer {accessToken}
```

---

### **4. AccessToken Expira → Refrescar Automáticamente**

```bash
POST /api/auth/refresh
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
  "accessTokenExpiresAt": "2025-11-12T14:30:00Z",
  "refreshTokenExpiresAt": "2025-11-19T14:15:00Z"
}
```

---

## 💻 **Implementación en Cliente (Node.js/JavaScript)**

### **Opción 1: Interceptor Axios (Recomendado)**

```javascript
const axios = require('axios');

let accessToken = localStorage.getItem('accessToken');
let refreshToken = localStorage.getItem('refreshToken');

const apiClient = axios.create({
    baseURL: 'http://localhost:5000/api'
});

// Request interceptor: Agregar token
apiClient.interceptors.request.use(config => {
    if (accessToken) {
        config.headers.Authorization = `Bearer ${accessToken}`;
    }
    return config;
});

// Response interceptor: Manejar token expirado
apiClient.interceptors.response.use(
    response => response, // Si está OK, seguir
    async error => {
        const originalRequest = error.config;

        // Si es 401 y no hemos intentado refresh
        if (error.response?.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;

            try {
                // Intentar refresh
                const response = await axios.post('http://localhost:5000/api/auth/refresh', {
                    accessToken: accessToken,
                    refreshToken: refreshToken
                });

                // Guardar nuevos tokens
                accessToken = response.data.accessToken;
                refreshToken = response.data.refreshToken;
                localStorage.setItem('accessToken', accessToken);
                localStorage.setItem('refreshToken', refreshToken);

                // Reintentar request original con nuevo token
                originalRequest.headers.Authorization = `Bearer ${accessToken}`;
                return apiClient(originalRequest);

            } catch (refreshError) {
                // Refresh falló, redirigir a login
                console.error('Refresh token inválido, redirigir a login');
                window.location.href = '/login';
                return Promise.reject(refreshError);
            }
        }

        return Promise.reject(error);
    }
);

// Uso normal
async function getFacilities() {
    const response = await apiClient.get('/facility');
    return response.data;
}
```

---

### **Opción 2: Verificar Antes de Cada Request**

```javascript
async function makeAuthenticatedRequest(url, options = {}) {
    const accessTokenExpiry = new Date(localStorage.getItem('accessTokenExpiry'));
    const now = new Date();

    // Si el token expira en menos de 1 minuto, refrescarlo
    if (accessTokenExpiry - now < 60000) {
        await refreshAccessToken();
    }

    // Hacer request con token válido
    return fetch(url, {
        ...options,
        headers: {
            ...options.headers,
            'Authorization': `Bearer ${localStorage.getItem('accessToken')}`
        }
    });
}

async function refreshAccessToken() {
    const response = await fetch('http://localhost:5000/api/auth/refresh', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            accessToken: localStorage.getItem('accessToken'),
            refreshToken: localStorage.getItem('refreshToken')
        })
    });

    const data = await response.json();
    
    if (data.success) {
        localStorage.setItem('accessToken', data.accessToken);
        localStorage.setItem('refreshToken', data.refreshToken);
        localStorage.setItem('accessTokenExpiry', data.accessTokenExpiresAt);
    } else {
        // Redirigir a login
        window.location.href = '/login';
    }
}
```

---

## 🔐 **Logout con Revocación de RefreshToken**

```bash
POST /api/auth/logout
Authorization: Bearer {accessToken}
{
  "refreshToken": "token-to-revoke"
}
```

```javascript
async function logout() {
    await axios.post('http://localhost:5000/api/auth/logout', {
        refreshToken: localStorage.getItem('refreshToken')
    }, {
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('accessToken')}`
        }
    });

    // Limpiar storage
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('accessTokenExpiry');

    // Redirigir a login
    window.location.href = '/login';
}
```

---

## 📊 **Tabla RefreshToken en BD**

```sql
RefreshToken:
├── Id (PK, int, identity)
├── IdUsuario (FK → Usuario)
├── Token (string, 500 chars, unique)
├── JwtId (string) → ID del AccessToken asociado
├── IsUsed (bool) → Si ya se usó para refresh
├── IsRevoked (bool) → Si fue revocado (logout)
├── CreatedDate (datetime)
└── ExpiryDate (datetime) → Cuando expira el RefreshToken
```

**Crear tabla:**
```bash
# Ejecutar script SQL incluido
sqlcmd -S server -d QA.Gresst -i CREATE_REFRESH_TOKEN_TABLE.sql
```

---

## ⚙️ **Configuración**

### **appsettings.json:**

```json
{
  "Authentication": {
    "AccessTokenExpirationMinutes": "15",  // AccessToken: 15 minutos
    "RefreshTokenExpirationDays": "7"      // RefreshToken: 7 días
  }
}
```

**Valores recomendados:**
- **Desarrollo:** AccessToken=60min, RefreshToken=30días
- **Producción:** AccessToken=15min, RefreshToken=7días
- **Alta seguridad:** AccessToken=5min, RefreshToken=1día

---

## 🔒 **Seguridad**

### **1. RefreshToken es de UN SOLO USO**
```
✅ Usuario usa RefreshToken → Se marca IsUsed=true
✅ Nuevo RefreshToken es generado
❌ Intentar reusar RefreshToken → Error "ya fue usado"
```

### **2. RefreshToken se Revoca en Logout**
```bash
POST /api/auth/logout
→ RefreshToken se marca IsRevoked=true
→ No se puede usar nunca más
```

### **3. RefreshToken Expira**
```
Después de 7 días (configurable):
❌ RefreshToken.ExpiryDate < now
→ Usuario debe hacer login nuevamente
```

### **4. Rotación de Tokens**
Cada refresh genera:
- Nuevo AccessToken
- Nuevo RefreshToken (el anterior se marca IsUsed)

---

## 📱 **Ejemplos por Plataforma**

### **React:**
```javascript
import axios from 'axios';
import { useState, useEffect } from 'react';

function useAuth() {
    const [accessToken, setAccessToken] = useState(localStorage.getItem('accessToken'));
    const [refreshToken, setRefreshToken] = useState(localStorage.getItem('refreshToken'));

    useEffect(() => {
        // Configurar interceptor axios
        const interceptor = axios.interceptors.response.use(
            response => response,
            async error => {
                if (error.response?.status === 401 && !error.config._retry) {
                    error.config._retry = true;
                    try {
                        const response = await axios.post('/api/auth/refresh', {
                            accessToken,
                            refreshToken
                        });
                        setAccessToken(response.data.accessToken);
                        setRefreshToken(response.data.refreshToken);
                        localStorage.setItem('accessToken', response.data.accessToken);
                        localStorage.setItem('refreshToken', response.data.refreshToken);
                        error.config.headers.Authorization = `Bearer ${response.data.accessToken}`;
                        return axios(error.config);
                    } catch {
                        // Redirect to login
                        window.location.href = '/login';
                    }
                }
                return Promise.reject(error);
            }
        );

        return () => axios.interceptors.response.eject(interceptor);
    }, [accessToken, refreshToken]);

    return { accessToken, refreshToken };
}
```

### **Angular:**
```typescript
@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const accessToken = localStorage.getItem('accessToken');
        const authReq = req.clone({
            headers: req.headers.set('Authorization', `Bearer ${accessToken}`)
        });

        return next.handle(authReq).pipe(
            catchError((error: HttpErrorResponse) => {
                if (error.status === 401) {
                    return this.handle401Error(authReq, next);
                }
                return throwError(error);
            })
        );
    }

    private handle401Error(request: HttpRequest<any>, next: HttpHandler) {
        return this.authService.refreshToken().pipe(
            switchMap((tokens: any) => {
                const authReq = request.clone({
                    headers: request.headers.set('Authorization', `Bearer ${tokens.accessToken}`)
                });
                return next.handle(authReq);
            }),
            catchError((err) => {
                this.router.navigate(['/login']);
                return throwError(err);
            })
        );
    }
}
```

---

## ✅ **Ventajas del Sistema**

1. ✅ **Seguridad:** AccessToken corto (15 min) minimiza riesgo si es robado
2. ✅ **UX:** Usuario no hace login cada 15 minutos
3. ✅ **Control:** RefreshToken se puede revocar (logout)
4. ✅ **Rotación:** Cada refresh genera nuevos tokens
5. ✅ **Un Uso:** RefreshToken no se puede reusar
6. ✅ **Expiración:** RefreshToken eventual expira (7 días)

---

## 🔍 **Troubleshooting**

### **Error: "Refresh token inválido"**
- RefreshToken no existe en BD
- RefreshToken ya fue usado (IsUsed=true)
- RefreshToken fue revocado (IsRevoked=true)
- RefreshToken expiró

**Solución:** Redirigir a login

### **Error: "Token ya fue usado"**
- Intentas reusar un RefreshToken
- Posible ataque (token replay)

**Solución:** Revocar TODOS los tokens del usuario

---

**🎉 Sistema RefreshToken Completo y Seguro!**

