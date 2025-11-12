# 🏗️ Refactorización: Account y User - Separación Domain vs Infrastructure

## ✅ Completado

Se ha limpiado la arquitectura para separar correctamente las entidades de **dominio de negocio** vs **concerns técnicos**.

---

## 🗑️ **Eliminado del Domain**

### ❌ `Domain/Entities/User.cs` - ELIMINADO
**Razón:** `User` NO es del dominio de negocio de gestión de residuos, es solo para **autenticación/autorización**.

**Ubicación correcta:**
- `Infrastructure/Data/Entities/Usuario.cs` ✅ (tabla BD scaffolded)
- `Infrastructure/Services/UserService.cs` ✅ (gestión de usuarios)
- `Infrastructure/Authentication/` ✅ (login/logout)

---

## ✨ **Account Refactorizado**

### Antes (mezclado):
```csharp
// Domain/Entities/Account.cs - ANTES ❌
public class Account : BaseEntity
{
    public string Name { get; set; }
    public string Role { get; set; }                           // String no tipado
    public Guid AdministratorId { get; set; }                  // ❌ Usuario de auth
    public virtual User? Administrator { get; set; }           // ❌ Referencia a User
    public Dictionary<string, string> Parameters { get; set; } // ❌ Técnico
    public Dictionary<string, string> Settings { get; set; }   // ❌ Técnico
    public bool PermissionsBySite { get; set; }                // ❌ Técnico
    public string? Status { get; set; }                        // String no tipado
}
```

### Ahora (limpio):
```csharp
// Domain/Entities/Account.cs - AHORA ✅
public class Account : BaseEntity
{
    // ✅ Solo aspectos de NEGOCIO
    public string Name { get; set; }
    public string? Code { get; set; }
    public AccountRole Role { get; set; }           // ✅ Enum tipado
    public AccountStatus Status { get; set; }       // ✅ Enum tipado
    
    // ✅ Relaciones de negocio
    public Guid PersonId { get; set; }
    public virtual Person? Person { get; set; }
    public Guid? ParentAccountId { get; set; }
    
    // ✅ Capacidades de negocio (computed)
    public bool IsGenerator => Role == AccountRole.Generator || Role == AccountRole.Both;
    public bool IsOperator => Role == AccountRole.Operator || Role == AccountRole.Both;
    public bool IsActiveForBusiness => Status == AccountStatus.Active;
}
```

---

## 🆕 **Nuevos Enums Creados**

### `Domain/Enums/AccountRole.cs`
```csharp
public enum AccountRole
{
    Generator,  // "N" en BD - Genera residuos
    Operator,   // "S" en BD - Operador logístico
    Both        // "B" en BD - Ambos roles
}
```

### `Domain/Enums/AccountStatus.cs`
```csharp
public enum AccountStatus
{
    Active,      // "A" en BD
    Inactive,    // "I" en BD
    Suspended,   // "S" en BD
    Blocked      // "B" en BD
}
```

---

## 🔄 **AccountMapper Actualizado**

Se actualizó `Infrastructure/Mappers/AccountMapper.cs` para:
- ✅ Usar `GuidLongConverter` (refactorización anterior)
- ✅ Mapear `AccountRole` enum ↔ string BD ("N", "S", "B")
- ✅ Mapear `AccountStatus` enum ↔ string BD ("A", "I", "S", "B")
- ✅ Eliminar referencias a `User` y campos técnicos

---

## 📊 **Separación Clara**

| Concepto | Ubicación | Propósito |
|----------|-----------|-----------|
| **Account** | `Domain/Entities` ✅ | Organización de negocio (genera/opera residuos) |
| **Person** | `Domain/Entities` ✅ | Persona física/actor de negocio |
| **Usuario** | `Infrastructure/Data/Entities` ✅ | Solo autenticación (login/password) |
| **UserService** | `Infrastructure/Services` ✅ | Gestión de usuarios (CRUD) |
| **AuthController** | `API/Controllers` ✅ | Login/Logout endpoints |

---

## ✅ **Beneficios de la Refactorización**

### 1. **Domain Limpio**
- Solo entidades de negocio
- Sin concerns técnicos (auth, config, permisos)
- Enums tipados y expresivos

### 2. **Separación de Responsabilidades**
```
Account (Domain)
└── ¿Qué hace en el negocio? → Genera o maneja residuos

Usuario (Infrastructure)  
└── ¿Qué hace? → Se autentica en el sistema
```

### 3. **Arquitectura Clara**
```
Domain (Negocio):
├── Account → Organización
├── Person → Persona
├── Waste → Residuo
└── Management → Gestión

Infrastructure (Técnico):
├── Usuario → Autenticación
├── UserService → CRUD usuarios
└── Authentication/ → Login/Logout
```

### 4. **Más Mantenible**
- Cambios de autenticación NO afectan Domain
- Domain se enfoca solo en reglas de negocio
- Más fácil de testear

---

## 🎯 **Modelo de Negocio Correcto**

### Account representa:
- ✅ Empresa generadora de residuos
- ✅ Operador logístico (recolector, transportista)
- ✅ Planta de tratamiento
- ✅ Actor organizacional del negocio

### Usuario NO representa:
- ❌ NO es actor de negocio
- ❌ Solo login/password/roles
- ❌ Concern técnico de la aplicación

---

## 📝 **Próximos Pasos Recomendados**

1. ✅ **Completado:** Refactorizar GuidLongConverter
2. ✅ **Completado:** Eliminar User del Domain
3. ✅ **Completado:** Limpiar Account
4. ⏳ **Pendiente:** Implementar IUserService completamente
5. ⏳ **Pendiente:** Documentar relación Account ↔ Person ↔ Usuario
6. ⏳ **Pendiente:** Crear tests unitarios para mappers

---

## 🔍 **Archivos Modificados**

- ✅ `Domain/Entities/User.cs` → **ELIMINADO**
- ✅ `Domain/Entities/Account.cs` → **REFACTORIZADO**
- ✅ `Domain/Enums/AccountRole.cs` → **CREADO**
- ✅ `Domain/Enums/AccountStatus.cs` → **CREADO**
- ✅ `Infrastructure/Mappers/AccountMapper.cs` → **ACTUALIZADO**
- ✅ `Infrastructure/Data/GreesstDbContext.cs` → **ACTUALIZADO**
- ✅ `Infrastructure/Services/UserService.cs` → **CREADO**
- ✅ `Application/Services/IUserService.cs` → **CREADO**
- ✅ `Application/DTOs/UserDto.cs` → **CREADO**
- ✅ `API/Controllers/UserController.cs` → **CREADO**

---

## ✅ **Verificación**

```bash
✅ Build exitoso (0 errores)
✅ User eliminado del Domain
✅ Account limpio y tipado
✅ Separación clara Domain/Infrastructure
✅ Enums expresivos y consistentes
```

---

**Arquitectura ahora es Clean Architecture correcta! 🎉**

