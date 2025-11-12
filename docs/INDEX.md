# 📚 Documentación Gresst API

## 🚀 Inicio Rápido

1. **[SISTEMA-COMPLETO.md](./SISTEMA-COMPLETO.md)** - Comienza aquí para entender el sistema completo
2. **[AUTENTICACION.md](./AUTENTICACION.md)** - Sistema de login y JWT
3. **[EJEMPLOS-CRUDS.md](./EJEMPLOS-CRUDS.md)** - Ejemplos prácticos de uso

---

## 🔐 Seguridad y Autenticación

| Documento | Descripción |
|-----------|-------------|
| [AUTENTICACION.md](./AUTENTICACION.md) | Sistema de login, logout, JWT dual (BD + Externa) |
| [REFRESH-TOKEN.md](./REFRESH-TOKEN.md) | RefreshToken para renovar AccessToken |
| [AUTORIZACION.md](./AUTORIZACION.md) | Permisos granulares CRUD por opciones |
| [SEGMENTACION-DATOS.md](./SEGMENTACION-DATOS.md) | Filtrado de datos por usuario (row-level security) |
| [AUTENTICACION-AUTORIZACION-RESUMEN.md](./AUTENTICACION-AUTORIZACION-RESUMEN.md) | Resumen ejecutivo de los 3 niveles de seguridad |

---

## 🏗️ Arquitectura y Diseño

| Documento | Descripción |
|-----------|-------------|
| [ARCHITECTURE.md](./ARCHITECTURE.md) | Clean Architecture, capas, patrones |
| [MAPEO-COMPLETO.md](./MAPEO-COMPLETO.md) | Estrategia de mapeo Domain ↔ Database (Inglés ↔ Español) |
| [SISTEMA-COMPLETO.md](./SISTEMA-COMPLETO.md) | Diagrama de flujo completo con los 3 niveles de seguridad |

---

## 🔧 Refactorizaciones y Mejoras

| Documento | Descripción |
|-----------|-------------|
| [REFACTORING-GUIDCONVERTER.md](./REFACTORING-GUIDCONVERTER.md) | Factorización de conversiones Guid ↔ Long |
| [REFACTORING-ACCOUNT-USER.md](./REFACTORING-ACCOUNT-USER.md) | Separación Account (Domain) vs User (Infrastructure) |
| [SIMPLIFICACION-HEADERS.md](./SIMPLIFICACION-HEADERS.md) | Eliminación de header X-Account-Id redundante |

---

## 📖 Ejemplos y Tutoriales

| Documento | Descripción |
|-----------|-------------|
| [EJEMPLOS-CRUDS.md](./EJEMPLOS-CRUDS.md) | Ejemplos completos de CRUD con cURL |
| [CLIENT-EXAMPLE-NODEJS.js](./CLIENT-EXAMPLE-NODEJS.js) | Cliente Node.js completo con auto-refresh |
| [CREATE_REFRESH_TOKEN_TABLE.sql](./CREATE_REFRESH_TOKEN_TABLE.sql) | Script SQL para crear tabla RefreshToken |

---

## 🚀 Deployment

| Documento | Descripción |
|-----------|-------------|
| [DEPLOYMENT-WINDOWS.md](./DEPLOYMENT-WINDOWS.md) | Guía completa para desplegar en Windows Server con IIS |

---

## 🎯 Por Dónde Empezar

### **Si eres nuevo:**
1. 📘 Lee [SISTEMA-COMPLETO.md](./SISTEMA-COMPLETO.md) para entender el flujo completo
2. 🔐 Lee [AUTENTICACION.md](./AUTENTICACION.md) para implementar login
3. 💻 Usa [EJEMPLOS-CRUDS.md](./EJEMPLOS-CRUDS.md) para probar la API

### **Si vas a integrar:**
1. 🔑 Implementa login con [CLIENT-EXAMPLE-NODEJS.js](../CLIENT-EXAMPLE-NODEJS.js)
2. 🛡️ Lee [SEGMENTACION-DATOS.md](./SEGMENTACION-DATOS.md) para entender el filtrado
3. 📝 Consulta [EJEMPLOS-CRUDS.md](./EJEMPLOS-CRUDS.md) para cada endpoint

### **Si vas a desplegar:**
1. 🚀 Sigue [DEPLOYMENT-WINDOWS.md](./DEPLOYMENT-WINDOWS.md)
2. ⚙️ Configura `appsettings.json` según [AUTENTICACION.md](./AUTENTICACION.md)

---

## 📊 Estructura de la Documentación

```
docs/
├── INDEX.md (este archivo)
│
├── 🔐 Seguridad
│   ├── AUTENTICACION.md
│   ├── AUTORIZACION.md
│   ├── SEGMENTACION-DATOS.md
│   └── REFRESH-TOKEN.md
│
├── 🏗️ Arquitectura
│   ├── SISTEMA-COMPLETO.md
│   ├── ARCHITECTURE.md
│   └── MAPEO-COMPLETO.md
│
├── 📖 Tutoriales
│   └── EJEMPLOS-CRUDS.md
│
├── 🔧 Refactorizaciones
│   ├── REFACTORING-GUIDCONVERTER.md
│   ├── REFACTORING-ACCOUNT-USER.md
│   └── SIMPLIFICACION-HEADERS.md
│
└── 🚀 Deployment
    └── DEPLOYMENT-WINDOWS.md
```

---

**🎉 Toda la documentación organizada y accesible!**

