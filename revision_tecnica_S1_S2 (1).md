# 📋 Revisión Técnica — T-23: Semana 1 & Semana 2
> **Fecha de revisión:** 13 Julio 2026 | **Rama:** `develop` | **Revisor:** Tech Lead (Ronald)
> **Build status:** ✅ `dotnet build` — 0 errores / 8 warnings menores (CS8618 en AlternateProfile, no críticos)
> **Última actualización:** Post-pull 4a60a4f — correcciones S2 + base S3 incorporadas

---

## LEYENDA DE ESTADO

| Símbolo | Significado |
|---------|-------------|
| ✅ COMPLETO | Entrega 100% según especificación |
| ⚠️ PARCIAL | Implementado pero con deuda técnica o gaps significativos |
| ❌ FALTANTE | No implementado o no encontrado en la rama |
| 🐛 BUG | Implementado pero con un defecto concreto detectado |
| ✔️ CORREGIDO | Bug de revisión anterior ya resuelto en develop |

---

## SEMANA 1 — Fundaciones: Auth, Modelos, Setup

### T-01 · Setup / Solución
**Estado: ✅ COMPLETO**

- Estructura de carpetas correcta: `CRM.ApiHub`, `CRM.WebFrontend`, `CRM.WebFrontend.Client`
- `CRM.sln` presente y compilando sin errores
- `appsettings.json` con connection string, JWT config y settings básicos
- Paquetes instalados: `Npgsql`, `Dapper`, `JwtBearer`, `BCrypt.Net-Next`
- `Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true` configurado en `DependencyInjection.cs` ✅

---

### T-02 · Modelos C# — Core (Patricia)
**Estado: ✅ COMPLETO — Auditado a nivel de propiedades**

`Domain/Entities/` contiene **29 archivos**. Todos los POCOs core están presentes y auditados:

| POCO | Schema/Tabla | Estado columnas |
|------|-------------|-----------------|
| `Lead` | `lead_service.lead` | ✅ Completo |
| `SalesOrder` | `sales_service.sales_order` | ✅ Completo |
| `OrderStatus` | `sales_service.order_status` | ✔️ Schema y PK `id_status` corregidos |
| `OrderSubstatus` | `sales_service.order_substatus` | ✔️ Schema y PK `id_substatus` corregidos |
| `LeadPreSale` | `lead_service.lead_pre_sale` | ✅ Completo |
| `Campaign` | `campaign_service.campaign` | ✅ Completo |
| `SalesOrderHistoryEventRaw` | DTO proyección UNION ALL | ✅ Correcto (no mapea tabla real) |
| `Product` | `product_service.product` | ✔️ PK corregida a `[Column("id_prod")]` |
| `UserCampaign` | `user_campaign` | ⚠️ Sin `[Schema]` — verificar si es `user_service` o `campaign_service` |

---

### T-03 · BD — Verificación (Giuseppe)
**Estado: ⚠️ PARCIAL — Sin entregable**

- No se encontró reporte de smoke test en `docs/` ni `optional/`.
- La conexión funciona indirectamente (los repositorios compilan con SQL referenciando schemas correctos).

> **⚠️ DEUDA**: Solicitar a Giuseppe el script o documento de verificación de tablas.

---

### T-04 · Auth — Login JWT (Dayan)
**Estado: ✅ COMPLETO**

- `POST /api/auth/login` con BCrypt + fallback `password123` en Development.
- JWT con claims `id_user`, `username`, `roles` ✅
- `POST /api/auth/refresh-token` con Session Binding (IP + UserAgent) ✅

> ⚠️ **DEUDA menor**: El fallback `password123` debe desactivarse antes de producción. Agregar env check más robusto que solo `IsDevelopment()`.

---

### T-05 · Modelos C# — RRHH / Geo (Patricia)
**Estado: ✅ COMPLETO — Con correcciones aplicadas**

| POCO | Schema/Tabla | Estado columnas |
|------|-------------|-----------------|
| `Collaborator` | `ext_ecosystem.collaborators` | ✔️ Corregido: `name`, `paternal_surname`, `maternal_surname`, `id_user`, `document_number`, `state` |
| `SalesOrderCustodyLog` | `sales_service.sales_order_custody_log` | ✔️ Corregido: `id_log`, `id_order`, `from_user_id`, `to_user_id`, `from_role`, `to_role`, `transfer_type`, `comment`, `is_bulk`, `batch_id` |
| `OrderStatusHistory` | `sales_service.sales_order_status_history` | ✅ Completo |
| `OrderDocument` | `sales_service.order_document` | ✅ Completo |
| `StructuralDivision` | `ext_ecosystem.structural_division` | ✅ Sin uso directo aún |
| `AdminDivision` | `ext_ecosystem.admin_division` | ✅ Sin uso directo aún |
| `Country` | `country` (sin schema) | ⚠️ Falta `[Schema]` |
| `Nationality` | `nationality` (sin schema) | ⚠️ Falta `[Schema]` |

---

### T-06 · Frontend — Setup Blazor Híbrido (Giuseppe)
**Estado: ✅ COMPLETO**

- `CRM.WebFrontend` (InteractiveServer) para supervisores.
- `CRM.WebFrontend.Client` (InteractiveWebAssembly) para asesores.
- `CustomAuthenticationStateProvider` / `PersistingServerAuthenticationStateProvider` implementados.
- MudBlazor instalado y configurado en ambos proyectos ✅

---

### T-07 · Auth — Middleware JWT + Refresh + /me (Dayan)
**Estado: ✅ COMPLETO**

- Middleware JWT Bearer activo. `GET /api/auth/me`. `POST /api/auth/refresh-token` con Session Binding ✅

---

### T-08 · Repositorios — Campañas y Catálogos (Patricia)
**Estado: ✅ COMPLETO**

- `CampaignRepository`: `GetAllActive()`, `GetById()` ✅
- `CatalogRepository`: `GetOrderStatuses()`, `GetOrderSubstatuses(idStatus)`, `GetProducts(idCmpg)`, `GetCurrencies()` ✅
- La query de `GetProductsAsync` hace JOIN `campaign_service.campaign_product` sobre `product_service.product` ✅

---

### T-09 · Frontend — Login + Auth State (Giuseppe)
**Estado: ✅ COMPLETO**

- `/login` redirige según rol: `ASESOR → /asesor`, `SUPERVISOR → /supervisor` ✅

---

### T-10 · TL — Revisión S1 + Contratos API
**Estado: ✅ COMPLETO**

- `docs/contratos_api.md` (359 líneas), colección Postman exportada ✅

---

## SEMANA 2 — Core de ventas: Leads, Pre-venta, Órdenes

### T-11 · Dayan — Backend Leads
**Estado: ✅ COMPLETO — Bugs corregidos**

- CRUD completo + `PATCH /api/leads/{id}/status` con `[RequiresPermission]` ✅

**✔️ CORREGIDO — BUG #1:** `UpdateLeadStatusUseCase` ya no crea `SalesOrder` ni `OrderIncident` fantasmas. Ahora solo notifica via `INotificationService` ✅

**✔️ CORREGIDO — DEUDA #5:** `LeadRepository.UpdateStatusAsync` llama `sales_service.validate_status_transition(@FromStatus, @ToStatus, @Role)` via función PostgreSQL dentro de la misma transacción ✅

---

### T-12 · Patricia — Backend Pre-Venta
**Estado: ✅ COMPLETO — Todos los issues corregidos**

- Todos los endpoints implementados: `GET`, `POST`, `POST /calls`, `POST /assign`, `POST /convert` ✅

**✔️ CORREGIDO — BUG #2:** `GET /presales` ahora extrae `userId` del JWT. Si se pasa un `userId` diferente, verifica que el rol sea `SUPERVISOR` o `BACKOFFICE`, de lo contrario retorna `403` ✅

**✔️ CORREGIDO — FALTANTE:** `ConvertAsync` ahora implementado invocando `lead_service.convert_presale_to_order(@IdPresale, @IdCmpg, @UserId)` ✅

**🐛 BUG menor — fallback hardcodeado en `AddCallLog`:**
```csharp
// PreSaleController.cs línea 104
long userId = 1; // Si el claim falla, se usa userId=1 (primer registro de BD)
```
> Si el JWT no contiene el claim de userId, el call log se registrará a nombre del usuario con ID 1. Debe retornar `Unauthorized` en su lugar.

---

### T-13 · Giuseppe — Frontend Layout
**Estado: ✅ COMPLETO — Con fix de navegación**

- `NavMenu` con `<AuthorizeView>` diferenciado por roles: `ASESOR`, `SUPERVISOR`, `BACKOFFICE` ✅
- Logout implementado ✅

**✔️ CORREGIDO — BUG de doble highlight:** El NavLink de "Mis Ventas" (`/asesor/orders`) se sombreaba junto con "Nueva Orden" (`/asesor/orders/new`) porque Blazor usa match por prefijo por defecto. Se agregó `Match="NavLinkMatch.All"` para que solo se marque con URL exacta ✅

---

### T-14 · Dayan — Backend Órdenes de Venta
**Estado: ✅ COMPLETO**

- CRUD completo con transacciones, `FOR UPDATE`, triggers de auditoría, historial ✅

**✔️ MEJORADO:** `GET /api/orders` ahora extrae `userId` del JWT si no se provee como query param ✅

**⚠️ DEUDA de seguridad:** Si el JWT falla y no se provee `userId`, la query se ejecuta con `userId = null`, lo que podría retornar **todas las órdenes** según la implementación del repositorio. Debe retornar `Unauthorized` o filtrar por rol.

---

### T-15 · Patricia — Middleware de Permisos (Custodia)
**Estado: ✅ COMPLETO — Implementación robusta**

- `RequiresPermissionAttribute` extrae `statusId` desde route, query string o body JSON (con `EnableBuffering`) ✅
- Llama función PostgreSQL `IPermissionService.CanUserActionAsync` ✅

---

### T-16 · Giuseppe — Frontend Pre-Venta (WASM)
**Estado: ✅ COMPLETO — Bugs corregidos**

- Lista de pre-ventas activas con filtros por Operador y Cobertura ✅
- Paginación client-side, modales de llamada y derivación ✅
- Conectado al API (`/api/presales`, `/api/presales/{id}/calls`, `/api/presales/{id}/assign`) ✅

**✔️ CORREGIDO — DEUDA:** `PreSales.razor` ahora usa `@rendermode InteractiveWebAssembly` (antes usaba `InteractiveServer`, lo que rompía el propósito del módulo WASM y causaba delays al navegar entre páginas) ✅

---

### T-17 · Dayan — Backend Documentos
**Estado: ✅ COMPLETO**

- Upload multipart/form-data, verificación de documentos ✅

---

### T-18 · Patricia — Backend Formularios Dinámicos
**Estado: ✅ COMPLETO**

- CRUD de templates, campos dinámicos, `OrderData`, notificaciones al cambiar estado de campo ✅

---

### T-19 · Giuseppe — Frontend Nueva Venta + Formulario Dinámico (WASM)
**Estado: ⚠️ PARCIAL — `idLead` pendiente**

- Selección de campaña carga etapas del API dinámicamente ✅
- Formulario dinámico según campaña + etapa con campos por tipo (`text`, `select`, `date`, `file`) ✅
- Validación client-side con `ValidationRegex` y `ValidationType` (DNI_ES, NIE_ES, PHONE_ES, CUPS_ENERGY) ✅
- Loading state en botón Guardar ✅

**✔️ CORREGIDO — `idUser`:** Ahora se extrae correctamente del JWT vía `AuthenticationStateProvider` (antes era `idUser = 101` hardcodeado) ✅

**✔️ CORREGIDO — `@rendermode`:** Cambiado de `InteractiveServer` a `InteractiveWebAssembly` para evitar delays de navegación entre páginas del asesor ✅

**🟡 DEUDA pendiente — `idLead` hardcodeado:**
```csharp
// NewOrder.razor línea 351
idLead = 1, // TODO: Implementar selector de Lead en la UI para obtener el idLead real
```
> Todas las órdenes creadas quedan asociadas al Lead con ID 1 en la BD. Requiere un selector de lead o recibir `idLead` como parámetro de ruta desde la pantalla de Leads.

---

### T-20 · Dayan — Backend Historial / Timeline
**Estado: ✅ COMPLETO — Calidad alta**

- UNION ALL de 6 fuentes de eventos con JOIN a `ext_ecosystem.collaborators` (ahora mapeado correctamente) ✅
- `ActorName` con fallback `"Sistema / Desconocido"` ✅

---

### T-21 · Patricia — Ficha Alterna y Aprobaciones
**Estado: ✅ COMPLETO — Flujo corregido**

**✔️ CORREGIDO — DEUDA #6:** Flujo de roles ya correcto:
- `POST /api/orders/{id}/approvals` → `[Authorize(Roles = "ASESOR,BACKOFFICE")]` — crea solicitud en estado `PENDING` ✅
- `PATCH /api/approvals/{id}` → `[Authorize(Roles = "SUPERVISOR")]` — resuelve la solicitud ✅

**🐛 BUG menor — `AlternateProfileController` fallback inseguro:**
```csharp
// AlternateProfileController.cs línea 39
long createdBy = 1; // Si JWT falla, acepta el userId del body del request
else if (dto.CreatedBy > 0) createdBy = dto.CreatedBy;
```
> Permite que el cliente envíe cualquier `CreatedBy` en el body si el claim del JWT falla. Debe retornar `Unauthorized`.

---

### T-22 · Giuseppe — Lista de Ventas del Asesor
**Estado: ✅ COMPLETO — Todos los bugs corregidos**

- Columnas: N° orden, cliente, campaña, estado (badge con color), substatus, fecha, días ✅
- Filtros: estado, campaña, fecha desde/hasta ✅
- Paginación ✅

**✔️ CORREGIDO — BUG #3:** `Orders.razor` ahora llama `/api/orders?userId=...` en lugar de `/api/campaigns/orders` (que no existía) ✅

**✔️ CORREGIDO — `@rendermode`:** Cambiado de `InteractiveServer` a `InteractiveWebAssembly` para consistencia con el resto del módulo WASM ✅

**⚠️ DEUDA:** Vista de detalle de orden (`GoToDetail`) solo muestra snackbar "próximamente". Pendiente para S3.

---

## SEMANA 3 — Supervisión y BackOffice (Base implementada)

### S3-Backend · Dayan + Patricia — Supervisor y BackOffice
**Estado: ⚠️ PARCIAL — Estructura implementada, con bugs de seguridad críticos**

#### Implementado:
- `SupervisorController`: `GET /api/supervisor/orders`, `GET /api/supervisor/stats`, `POST /api/supervisor/bulk-transfer` ✅
- `BackofficeController`: `GET /api/backoffice/orders`, `GET /api/backoffice/pending-docs`, `PATCH /api/backoffice/orders/{id}/status`, `PATCH /api/backoffice/documents/{id}/verify` ✅
- `SupervisorRepository`: queries con CTEs para filtrar por campañas y portfolios del supervisor ✅
- `BackofficeRepository`: filtrado por `custody_user_id`, transacciones con `set_config` para triggers ✅
- `DependencyInjection.cs` actualizado con todos los nuevos servicios y repositorios ✅

---

## RESUMEN DE ENTREGA

| Tarea | Dev | Estado actual | Prioridad Fix |
|-------|-----|---------------|---------------|
| T-01 Setup | Dayan | ✅ COMPLETO | — |
| T-02 Modelos Core | Patricia | ✅ COMPLETO | — |
| T-03 Smoke Test BD | Giuseppe | ⚠️ Sin entregable | BAJA |
| T-04 Auth Login | Dayan | ✅ COMPLETO | — |
| T-05 Modelos RRHH/Geo | Patricia | ✅ COMPLETO | — |
| T-06 Frontend Setup | Giuseppe | ✅ COMPLETO | — |
| T-07 JWT Middleware + /me | Dayan | ✅ COMPLETO | — |
| T-08 Repos Campañas/Catálogos | Patricia | ✅ COMPLETO | — |
| T-09 Login Frontend | Giuseppe | ✅ COMPLETO | — |
| T-10 TL Contratos S1 | TL | ✅ COMPLETO | — |
| T-11 Leads Backend | Dayan | ✅ COMPLETO | — |
| T-12 PreVenta Backend | Patricia | ✅ COMPLETO | — |
| T-13 Layout Frontend | Giuseppe | ✅ COMPLETO | — |
| T-14 Órdenes Backend | Dayan | ✅ COMPLETO | — |
| T-15 Middleware Custodia | Patricia | ✅ COMPLETO | — |
| T-16 PreVenta Frontend | Giuseppe | ✅ COMPLETO (rendermode corregido) | — |
| T-17 Docs Backend | Dayan | ✅ COMPLETO | — |
| T-18 Forms Backend | Patricia | ✅ COMPLETO | — |
| T-19 Nueva Venta Frontend | Giuseppe | ⚠️ idLead hardcodeado (idUser y rendermode corregidos) | **MEDIA** |
| T-20 Historial/Timeline | Dayan | ✅ COMPLETO | — |
| T-21 Aprobaciones/Ficha | Patricia | ✅ COMPLETO | — |
| T-22 Lista Ventas Frontend | Giuseppe | ✅ COMPLETO (endpoint y rendermode corregidos) | — |

---

## ISSUES ACTIVOS — POST-PULL 4a60a4f

### 🔴 BUG CRÍTICO #1 — `SupervisorController` sin restricción de rol
**Archivo:** `CRM.ApiHub/Api/Controllers/SupervisorController.cs`, línea 12
```csharp
// ACTUAL — cualquier usuario autenticado puede llamar estos endpoints
[Authorize]
[Route("api/supervisor")]
public class SupervisorController : ControllerBase

// CORRECTO
[Authorize(Roles = "SUPERVISOR")]
```
**Impacto:** Un ASESOR autenticado puede llamar `GET /api/supervisor/orders` y ver **todas las órdenes del equipo** del supervisor (o de todos los asesores si el supervisor no tiene campañas asignadas — ver BUG #4). Brecha de confidencialidad grave.

---

### 🔴 BUG CRÍTICO #2 — `BackofficeController` sin restricción de rol
**Archivo:** `CRM.ApiHub/Api/Controllers/BackofficeController.cs`, línea 12
```csharp
// ACTUAL — cualquier usuario autenticado puede llamar estos endpoints
[Authorize]

// CORRECTO
[Authorize(Roles = "BACKOFFICE,SUPERVISOR")]
```
**Impacto:** Un ASESOR puede llamar `GET /api/backoffice/orders`, `GET /api/backoffice/pending-docs` y `PATCH /api/backoffice/documents/{id}/verify`. Puede verificar documentos sin ser BackOffice.

---

### 🔴 BUG CRÍTICO #3 — CTE `eligible_advisors` expone todas las órdenes si el supervisor no tiene campañas
**Archivo:** `CRM.ApiHub/Infrastructure/Persistence/SupervisorRepository.cs`, líneas 53-62
```sql
-- El bug: si el supervisor NO tiene campañas NI portfolios asignados,
-- la condición NOT EXISTS devuelve TRUE, y se incluyen TODOS los asesores
eligible_advisors AS (
    SELECT id_user FROM team_members
    UNION
    SELECT id_user 
    FROM access_control.user_role 
    WHERE id_role = 1 AND NOT EXISTS (
        SELECT 1 FROM user_service.user_campaign WHERE id_user = @SupervisorId ...
        UNION ALL
        SELECT 1 FROM user_service.user_portfolio WHERE id_user = @SupervisorId ...
    )
)
```
**Impacto:** Un supervisor sin campañas/portfolios asignados ve **TODAS las órdenes de todos los asesores del sistema**. En entornos de prueba (con usuarios sin asignaciones) esto es una fuga masiva de datos. La rama `UNION` del fallback debe eliminarse o redefinirse con lógica de negocio explícita.

---

### 🔴 BUG CRÍTICO #4 — `SalesOrderController.GetOrders` puede retornar todas las órdenes si JWT falla
**Archivo:** `CRM.ApiHub/Api/Controllers/SalesOrderController.cs`, líneas 48-55
```csharp
if (!userId.HasValue)
{
    var userIdClaim = User.FindFirst(...);
    if (userIdClaim != null && long.TryParse(...)) userId = parsedId;
    // Si el claim falla: userId sigue siendo null → query sin filtro de usuario
}
var orders = await _getSalesOrdersUseCase.ExecuteAsync(userId, ...); // userId = null → ¿todas las órdenes?
```
**Acción:** Si `userId` queda `null`, retornar `Unauthorized()` o `BadRequest()`. No ejecutar la query.

---

### 🔴 BUG CRÍTICO #5 — `SupervisorRepository.BulkTransfer` usa `id_status = 3` hardcodeado
**Archivo:** `CRM.ApiHub/Infrastructure/Persistence/SupervisorRepository.cs`, línea 253
```csharp
// Hardcodeado como número mágico — si el status "Pendiente BackOffice" no es 3 en la BD, todo falla
SET id_status = 3,
```
**Acción:** Usar una constante nombrada o leer el ID del catálogo. Mínimo, documentar el contrato en la BD.

---

### 🟡 BUG menor #6 — `PreSaleController.AddCallLog` tiene fallback `userId = 1`
**Archivo:** `CRM.ApiHub/Api/Controllers/PreSaleController.cs`, línea 104
```csharp
long userId = 1; // Si el claim falla, el log queda registrado como usuario ID 1
```
**Acción:** Si el claim no se puede parsear, retornar `Unauthorized()`.

---

### 🟡 BUG menor #7 — `AlternateProfileController` acepta `CreatedBy` del body si JWT falla
**Archivo:** `CRM.ApiHub/Api/Controllers/AlternateProfileController.cs`, líneas 39-47
```csharp
long createdBy = 1;
if (userClaim != null && ...) createdBy = parsedId;
else if (dto.CreatedBy > 0) createdBy = dto.CreatedBy; // Cliente controla el actor
```
**Acción:** Si JWT falla → `Unauthorized()`. No aceptar el `CreatedBy` del cuerpo del request.

---

### 🟡 DEUDA #8 — `NewOrder.razor` tiene `idLead = 1` hardcodeado (Giuseppe)
**Archivo:** `CRM.WebFrontend.Client/Pages/Asesor/NewOrder.razor`, línea 351
```csharp
idLead = 1, // TODO: Implementar selector de Lead en la UI para obtener el idLead real
```
**Estado:** `idUser` ya fue corregido (extraído del JWT ✅). Solo queda pendiente `idLead`.
**Acción:** Recibir `idLead` como `[Parameter]` de ruta (ej. `/asesor/orders/new/{idLead}`) o agregar un `<select>` de leads activos del asesor.

---

### 🟡 DEUDA #9 — `InMemoryRefreshTokenStore` no es apta para múltiples instancias
**Archivo:** `DependencyInjection.cs`, línea 51
```csharp
services.AddSingleton<IRefreshTokenStore, InMemoryRefreshTokenStore>();
```
**Impacto:** En despliegue multi-instancia (load balancer), los refresh tokens no se comparten entre instancias. Los usuarios perderán sesión al hacer round-robin. Migrar a Redis o BD antes de producción.

---

### 🟡 DEUDA #10 — `SupervisorRepository` tiene la misma CTE repetida 3 veces
**Archivo:** `CRM.ApiHub/Infrastructure/Persistence/SupervisorRepository.cs`

Las CTEs `supervisor_campaigns`, `supervisor_portfolios`, `team_members` y `eligible_advisors` se repiten idénticas en `GetTeamOrdersAsync` y `GetTeamStatsAsync` (dos queries separadas dentro del mismo método). Esto es duplicación significativa. Extraer a un método privado `GetEligibleAdvisorIds(connection, supervisorId)` o como vista/función en PostgreSQL.

---

### 🟡 DEUDA #11 — `GetPendingVerificationUseCase` no filtra por BackOffice asignado
**Archivo:** `BackofficeRepository.GetPendingVerificationAsync`
```sql
SELECT * FROM sales_service.order_document WHERE verification_status = 'PENDING' AND is_active = true
```
**Impacto:** Retorna **todos** los documentos pendientes del sistema, no solo los de las órdenes asignadas al BackOffice autenticado. Un BackOffice ve documentos de órdenes que no le pertenecen. Debe filtrarse por el `backofficeId` del usuario autenticado.

---

## CONTRATOS S3 — Estado de implementación

| Módulo | Endpoint | Estado |
|--------|----------|--------|
| Supervisor | `GET /api/supervisor/orders` | ✅ Implementado — pendiente fix de roles y CTE |
| Supervisor | `GET /api/supervisor/stats` | ✅ Implementado — pendiente fix de roles |
| Supervisor | `POST /api/supervisor/bulk-transfer` | ✅ Implementado — pendiente fix `id_status=3` |
| BackOffice | `GET /api/backoffice/orders` | ✅ Implementado — pendiente fix de roles |
| BackOffice | `GET /api/backoffice/pending-docs` | 🐛 Implementado — retorna todos los docs del sistema |
| BackOffice | `PATCH /api/backoffice/orders/{id}/status` | ✅ Implementado |
| BackOffice | `PATCH /api/backoffice/documents/{id}/verify` | ✅ Implementado — pendiente fix de roles |
| Notificaciones | `GET /notifications/me` | ❌ No implementado |
| Notificaciones | `PATCH /notifications/{id}/read` | ❌ No implementado |

**Dashboard BackOffice:** UI contra `MockBackofficeService` — pendiente conexión al API real.

---

## PRIORIDADES DE FIX — SEMANA 3

| Prioridad | Issue | Dev responsable |
|-----------|-------|-----------------|
| 🔴 INMEDIATO | BUG #1: `SupervisorController` sin rol | Dayan |
| 🔴 INMEDIATO | BUG #2: `BackofficeController` sin rol | Patricia |
| 🔴 INMEDIATO | BUG #3: CTE supervisor expone todos los asesores | Dayan |
| 🔴 INMEDIATO | BUG #4: `GET /orders` puede retornar todo sin userId | Dayan |
| 🔴 INMEDIATO | BUG #5: `id_status = 3` hardcodeado en bulk-transfer | Dayan |
| 🟡 ESTA SEMANA | BUG #6: fallback `userId=1` en PreSale AddCallLog | Patricia |
| 🟡 ESTA SEMANA | BUG #7: `AlternateProfile` acepta actor del body | Patricia |
| 🟡 ESTA SEMANA | DEUDA #8: `idLead=1` en NewOrder.razor | Giuseppe |
| 🟡 BACKLOG | DEUDA #9: `InMemoryRefreshTokenStore` para producción | Dayan |
| 🟡 BACKLOG | DEUDA #10: CTE duplicada en SupervisorRepository | Dayan |
| 🟡 BACKLOG | DEUDA #11: `pending-docs` sin filtro de backoffice | Patricia |
