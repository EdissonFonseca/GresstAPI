# API Roles (Gresst)

Roles are **account-scoped**: each user belongs to one account and has one or more roles that apply within that account.

## Role definitions

| Role    | Description |
|---------|-------------|
| **Admin** | Full account management: users, authorizations, settings. Can see all account data and manage other users. |
| **User**  | Standard operator: create and edit operational data (facilities, routes, orders, requests) within the account. Data may be segmented by facility or scope. |
| **Viewer** | Read-only: view reports and data within the account. No create/update/delete. |

## Hierarchy

- **Admin** implies full access (everything User and Viewer can do).
- **User** implies operational access (everything Viewer can do, plus create/edit).
- **Viewer** is read-only.

## How roles are used

1. **Storage**: Roles are stored per user (e.g. in `Usuario.DatosAdicionales` as `{ "roles": ["Admin"] }` or `["User"]`).
2. **Login**: At login, roles are read from the user and added to the JWT as `ClaimTypes.Role` claims. The API uses these claims for authorization.
3. **Policies**: The API registers policies such as `AdminOnly` (requires Admin role) and uses them on endpoints with `RequireAuthorization("AdminOnly")`.
4. **Defaults**: New user under an existing account gets **User**. First user of a new account (account registration) gets **Admin**.

## Constants

Use `Gresst.Application.Constants.ApiRoles` for role names and helpers:

- `ApiRoles.Admin`, `ApiRoles.User`, `ApiRoles.Viewer`
- `ApiRoles.DefaultRole`, `ApiRoles.AccountAdminRole`
- `ApiRoles.PolicyAdminOnly`, `ApiRoles.PolicyAdminOrUser`
- `ApiRoles.IsAdmin(roles)`, `ApiRoles.IsAdminOrUser(roles)`
