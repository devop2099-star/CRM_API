# 📋 Revisión Técnica — T-23: Semana 1 & Semana 2
> **Fecha de revisión:** 13 Julio 2026 | **Rama:** `develop` | **Revisor:** Tech Lead (Ronald)
> **Build status:** ✅ `dotnet build` — 0 errores / 8 warnings menores

---

## LEYENDA DE ESTADO

| Símbolo | Significado |
|---------|-------------|
| ✅ COMPLETO | Entrega 100% según especificación |
| ⚠️ PARCIAL | Implementado pero con deuda técnica o gaps significativos |
| ❌ FALTANTE | No implementado o no encontrado en la rama |
| 🐛 BUG | Implementado pero con un defecto concreto detectado |

---

## SEMANA 1 — Fundaciones: Auth, Modelos, Setup

### T-01 · Setup / Solución
**Estado: ✅ COMPLETO**

- Estructura de carpetas correcta: `CRM.ApiHub`, `CRM.WebFrontend`, `CRM.WebFrontend.Client`
- `CRM.sln` presente y compilando sin errores
- `appsettings.json` con connection string, JWT config y settings básicos
- Paquetes instalados: `Npgsql`, `Dapper`, `JwtBearer`, `BCrypt.Net-Next`

---

### T-02 · Modelos C# — Core
**Estado: ✅ COMPLETO — Auditado a nivel de propiedades**

`Domain/Entities/` contiene **29 archivos** (.gitkeep excluido). Los 8 POCOs del T-02 están todos presentes y auditados:

| POCO | Archivo | Schema/Tabla | `[Column]` | Observación |
|------|---------|-------------|------------|-------------|
| `Lead` | Lead.cs | `lead_service.lead` | ✅ Completo | `full_name` marcado como columna pero es generada por DB — no se inserta ✅ |
| `SalesOrder` | SalesOrder.cs | `sales_service.sales_order` | ✅ Completo | `id_substatus` nullable correcto |
| `OrderStatus` | OrderStatus.cs | `order_status` | ✅ Completo | ⚠️ Falta `[Schema]` — la tabla está en `sales_service` |
| `OrderSubstatus` | OrderSubstatus.cs | `order_substatus` | ✅ Completo | ⚠️ Falta `[Schema]` — tabla en `sales_service`. Columna `id_status` en BD pero el POCO mapea `order_status_id` |
| `LeadPreSale` | LeadPreSale.cs | `lead_service.lead_pre_sale` | ✅ Completo | Sin `[DatabaseGenerated]` en PK — correcto, la BD genera el id |
| `Campaign` | Campaign.cs | `campaign_service.campaign` | ✅ Completo | Columna PK mapeada como `id_cmpg` → `Id` ✅ |
| `SalesOrderHistoryEventRaw` | SalesOrderHistoryEventRaw.cs | DTO de proyección | Sin `[Column]` | ✅ Correcto — es un DTO de resultado de UNION ALL, no mapea a tabla real |
| `Product` | **Product.cs** — ✅ **EXISTE** | `product_service.product` | ✅ Completo | ⚠️ **DISCREPANCIA**: El repo usa `SELECT p.* ... WHERE p.id_prod = ...` pero el POCO usa `[Column("id")]` — la PK real en BD es `id_prod`, no `id`. Dapper fallará al mapear |
| `UserCampaign` | **UserCampaign.cs** — ✅ **EXISTE** | `user_campaign` | ✅ Completo | ⚠️ Falta `[Schema]` — tabla probablemente en `campaign_service` o `user_service`. Columnas `user_id`/`campaign_id` deben verificarse contra BD |

> **🔴 BUG CRÍTICO confirmado — `Product.Id` vs `id_prod`**: El SQL en `CatalogRepository` hace `SELECT p.*` de `product_service.product`, pero el POCO mapea la PK como `[Column("id")]`. La columna real en la tabla es `id_prod`. Dapper retornará `Id = 0` en todos los productos. **Corregir a `[Column("id_prod")]`.**

> **⚠️ DEUDA — `OrderStatus` y `OrderSubstatus` sin `[Schema]`**: El `CatalogRepository` usa `sales_service.order_status` y `sales_service.order_substatus` en SQL hardcodeado, lo que es correcto. Pero si alguien intenta usar estos POCOs con un ORM futuro o scaffolding, el `[Table]` sin `[Schema]` puede causar problemas.

---

### T-03 · BD — Verificación
**Estado: ⚠️ PARCIAL — Sin evidencia de entregable**

- No se encontró reporte de smoke test de tablas por schema en la carpeta `docs/` ni `optional/`.
- La conexión funciona (el API compila y los repositorios tienen SQL referenciando los schemas correctos: `sales_service`, `ext_ecosystem`, `access_control`).

> **⚠️ DEUDA**: Falta el documento de verificación de tablas prometido. Solicitar a Giuseppe o generar script de verificación.

---

### T-04 · Auth — Login JWT
**Estado: ✅ COMPLETO**

- `AuthController` → `POST /api/auth/login` implementado.
- `LoginUseCase` verifica credenciales: BCrypt para usuarios reales + fallback `password123` en Development.
- JWT con claims `id_user`, `username`, `roles` confirmados en `JwtTokenGenerator`.
- Tiempo de expiración configurable vía `appsettings.json`.
- Fallback de dev añade `test.backoffice` (no estaba en spec, mejora válida ✅).

---

### T-05 · Modelos C# — RRHH / Geo
**Estado: ✅ COMPLETO — Auditado. Con 2 discrepancias críticas de schema**

Todos los 8 POCOs del T-05 están presentes y auditados:

| POCO | Archivo | Schema/Tabla | `[Column]` | Observación |
|------|---------|-------------|------------|-------------|
| `Collaborator` | Collaborator.cs | `ext_ecosystem.collaborator` | ✅ Completo | 🔴 **DISCREPANCIA CRÍTICA**: El POCO usa `first_name`/`last_name`/`email`, pero la tabla real en el SQL del timeline es `name`, `paternal_surname`, `maternal_surname`, `id_user`. Las propiedades NO coinciden con la tabla real del FDW. |
| `StructuralDivision` | StructuralDivision.cs | `ext_ecosystem.structural_division` | ✅ Completo | Columnas básicas (`id`, `name`, `code`, `parent_id`, `is_active`). Sin uso directo en repositorios aún. |
| `AdminDivision` | AdminDivision.cs | `ext_ecosystem.admin_division` | ✅ Completo | Columnas `country_id`, `name`, `code`, `type`, `is_active`. Sin uso directo en repositorios aún. |
| `Country` | Country.cs | `country` (sin schema) | ✅ Completo | ⚠️ Falta `[Schema]`. Columnas parecen estándar. |
| `Nationality` | Nationality.cs | `nationality` (sin schema) | ✅ Completo | Muy reducido: solo `id`, `name`, `is_active`. Puede estar incompleto. |
| `OrderStatusHistory` | OrderStatusHistory.cs | `sales_service.sales_order_status_history` | ✅ Completo | Columnas completas con `id_history`, `from_status_id`, `to_status_id`, `changed_by`, etc. ✅ |
| `SalesOrderCustodyLog` | SalesOrderCustodyLog.cs | `sales_order_custody_log` (sin schema) | ✅ Completo | 🔴 **DISCREPANCIA CRÍTICA**: El POCO usa `sales_order_id`, `previous_custodian_id`, `current_custodian_id`, `action_date`. Pero el SQL del timeline usa `id_order`, `from_user_id`, `to_user_id`, `from_role`, `to_role`, `transfer_type`, `register`. Las columnas NO coinciden en absoluto. |
| `OrderDocument` | OrderDocument.cs | `sales_service.order_document` | ✅ Completo | Columnas completas y coherentes con el SQL del `OrderDocumentRepository`. ✅ |

### Resumen de discrepancias T-05

**🔴 CRÍTICO 1 — `Collaborator.cs` desfasado de la tabla real FDW:**
```csharp
// POCO actual (INCORRECTO para la tabla ext_ecosystem.collaborators)
public string FirstName { get; set; }  // columna real: "name"
public string LastName { get; set; }   // columnas reales: "paternal_surname", "maternal_surname"
public int UserId { get; set; }        // columna real: "id_user" ✅
public string Email { get; set; }      // verificar si existe en el FDW
```
El SQL en `SalesOrderRepository.GetOrderHistoryTimelineAsync` hace JOIN directo a `ext_ecosystem.collaborators` y usa `col.name`, `col.paternal_surname`, `col.maternal_surname` — **el POCO no se usa en el JOIN** (usa proyección anónima), por lo que no rompe el runtime actual. Pero si alguien intenta hacer `QueryAsync<Collaborator>()`, fallará.

**🔴 CRÍTICO 2 — `SalesOrderCustodyLog.cs` completamente desfasado:**
```csharp
// POCO actual (INCORRECTO para la tabla real)
public int SalesOrderId { get; set; }         // columna real: "id_order"
public int? PreviousCustodianId { get; set; } // columna real: "from_user_id"
public int CurrentCustodianId { get; set; }   // columna real: "to_user_id"
public DateTime ActionDate { get; set; }       // columna real: "register"
// FALTAN: from_role, to_role, transfer_type, comment
```
Estas columnas nunca coincidirán con la tabla `sales_service.sales_order_custody_log`. El POCO fue creado a ciegas, sin consultar la BD real.

---

### T-06 · Frontend — Setup Blazor Híbrido
**Estado: ✅ COMPLETO**

- `CRM.WebFrontend` (InteractiveServer) para supervisores.
- `CRM.WebFrontend.Client` (InteractiveWebAssembly) para asesores.
- `CustomAuthenticationStateProvider` / `PersistingServerAuthenticationStateProvider` implementados.
- MudBlazor instalado y configurado en ambos proyectos.
- `HttpClient` con `BaseAddress` configurado en `Program.cs`.

---

### T-07 · Auth — Middleware JWT + Refresh + /me
**Estado: ✅ COMPLETO**

- Middleware JWT Bearer activo en `Program.cs`.
- `GET /api/auth/me` retorna datos del usuario autenticado.
- `POST /api/auth/refresh-token` con Session Binding (IP + UserAgent) implementado en `RefreshTokenUseCase`.
- Controladores protegidos con `[Authorize]`.

---

### T-08 · Repositorios — Campañas y Catálogos
**Estado: ✅ COMPLETO**

- `CampaignRepository`: `GetAllActive()`, `GetById()` → verificado en `CampaignController`.
- `CatalogRepository`: `GetOrderStatuses()`, `GetOrderSubstatuses(idStatus)`, `GetProducts(idCmpg)`, `GetCurrencies()` → todos presentes.
- Dapper con queries parametrizadas confirmado.

---

### T-09 · Frontend — Login + Auth State
**Estado: ✅ COMPLETO**

- Página `/login` con formulario usuario/password implementada en `Login.razor` (601 líneas, diseño premium).
- Llama `POST /api/auth/login` y maneja JWT.
- Redirige según rol: `ASESOR → /asesor`, `SUPERVISOR → /supervisor`.
- `CustomAuthenticationStateProvider` parsea claims del JWT.

---

### T-10 · TL — Revisión S1 + Contratos API
**Estado: ✅ COMPLETO**

- `docs/contratos_api.md` con 359 líneas documenta Auth, Leads, Pre-Venta, Órdenes, Catálogos.
- Colección Postman exportada: `docs/CRM_CallCenter_Semana1.postman_collection.json`.

---

## SEMANA 2 — Core de ventas: Leads, Pre-venta, Órdenes

### T-11 · Dayan — Backend Leads
**Estado: ⚠️ PARCIAL — Con bug de custodia**

**Implementado:**
- `LeadController`: `GET /api/leads`, `GET /api/leads/{id}`, `POST /api/leads`, `PATCH /api/leads/{id}/status` ✅
- `GetLeadsUseCase`, `GetLeadByIdUseCase`, `CreateLeadUseCase`, `UpdateLeadStatusUseCase` ✅
- Filtros de búsqueda: `searchTerm`, `statusId`, `page`, `limit` ✅
- `[RequiresPermission("update_lead_status")]` en el endpoint PATCH ✅

**🐛 BUG CRÍTICO detectado en `UpdateLeadStatusUseCase`:**
```csharp
// PROBLEMA: Cada cambio de estado de un Lead crea automáticamente
// una SalesOrder fantasma y una incidencia de prueba. Esto es código
// de testing que fue mezclado con lógica de producción.
var order = await _salesOrderRepository.GetByLeadIdAsync(idLead, ct);
if (order == null)
{
    // Crea una SalesOrder ficticia solo para poder crear una incidencia
    var newOrder = new SalesOrder { ... };
    await _salesOrderRepository.CreateAsync(newOrder, ct);
}
var testIncident = new OrderIncident { CustomName = "Incidencia por cambio de estado..." };
await _incidentRepository.CreateAsync(testIncident);
```
> **⚠️ ACCIÓN REQUERIDA**: Extraer y eliminar el bloque de creación automática de `SalesOrder` e `OrderIncident` del `UpdateLeadStatusUseCase`. Esto contamina la base de datos en producción con registros de prueba.

**❌ FALTANTE:**
- `sales_service.validate_status_transition` — La spec pedía validar la transición de estado contra esta función PostgreSQL. No está implementada; el `UpdateLeadStatusUseCase` solo llama `_leadRepository.UpdateStatusAsync` sin validar si la transición es permitida.

---

### T-12 · Patricia — Backend Pre-Venta
**Estado: ⚠️ PARCIAL — Falta endpoint `/convert`**

**Implementado:**
- `PreSaleController`: `GET /api/presales`, `POST /api/presales`, `POST /presales/{id}/calls`, `POST /presales/{id}/assign` ✅
- `IPreSaleRepository` con `GetByUserAsync`, `CreateAsync`, `AddCallLogAsync`, `AssignAsync` ✅

**❌ FALTANTE:**
- `POST /presales/{id}/convert` — El endpoint está declarado en el controller pero `_repository.ConvertAsync(id, ...)` pasa `new { UserId = request.UserId }` como parámetro anónimo. La implementación real de `ConvertAsync` en `PreSaleRepository` debe verificarse — probablemente está como stub.

**🐛 BUG — Seguridad en `GET /presales`:**
```csharp
// El userId se recibe como query param sin autenticación de que 
// el usuario que consulta es el mismo que el userId solicitado.
[HttpGet]
public async Task<IActionResult> GetByUser([FromQuery] int userId)
{
    var preSales = await _repository.GetByUserAsync(userId);
```
> Un asesor puede consultar las pre-ventas de cualquier otro asesor pasando un `userId` diferente. **Debe extraerse el `userId` del JWT**, no del query param.

---

### T-13 · Giuseppe — Frontend Layout
**Estado: ✅ COMPLETO**

- `MainLayout` con sidebar implementado con diseño profesional.
- `NavMenu` con `<AuthorizeView>` diferenciado por roles: `ASESOR`, `SUPERVISOR`, `BACKOFFICE` ✅
- Links correctos: Dashboard, Pre-ventas, Ventas (asesor) / Dashboard, Equipo, Alertas (supervisor/backoffice) ✅
- Logout implementado ✅
- `AsesorDashboard.razor` en `/asesor` ✅

> **⚠️ DEUDA MENOR**: El `AsesorDashboard.razor` tiene cards de métricas pero están hardcodeadas (no consumen el API). Aceptable para S2 si el objetivo era solo navegación.

---

### T-14 · Dayan — Backend Órdenes de Venta
**Estado: ✅ COMPLETO**

- `SalesOrderController`: `GET /api/orders`, `GET /api/orders/{id}`, `POST /api/orders`, `PATCH /api/orders/{id}/status` ✅
- `SalesOrderRepository.GetByFiltersAsync`: filtra por `userId`, `statusId`, `campaignId`, `dateFrom`, `dateTo` ✅
- `UpdateStatusAsync`: usa transacción con `FOR UPDATE`, registra en `sales_order_status_history` ✅
- `set_config('app.current_user_id', ...)` para triggers de auditoría ✅
- Historial incluido como `GET /api/orders/{id}/history` ✅

**⚠️ DEUDA MENOR**: El endpoint `GET /api/orders` no extrae el `userId` automáticamente del JWT — lo recibe como query param opcional. Mismo patrón de seguridad que en pre-ventas.

---

### T-15 · Patricia — Middleware de Permisos (Custodia)
**Estado: ✅ COMPLETO**

- `RequiresPermissionAttribute` implementado como `IAsyncAuthorizationFilter` ✅
- Extrae `userId` del JWT (multiples claims verificados: `NameIdentifier`, `sub`, `id_user`) ✅
- Extrae `statusId` desde route, query string O body JSON (con `EnableBuffering`) ✅
- Llama `IPermissionService.CanUserActionAsync(userId, permissionKey, statusId)` que invoca función PostgreSQL ✅
- Retorna `403` con mensaje descriptivo ✅
- **TEST:** `[RequiresPermission("update_lead_status")]` está aplicado en `LeadController.UpdateLeadStatus` ✅

> Esta implementación es **la más robusta del sprint**. El manejo de body buffering para extraer el `statusId` es un detalle de calidad avanzada.

---

### T-16 · Giuseppe — Frontend Pre-Venta (WASM)
**Estado: ✅ COMPLETO**

- `PreSales.razor` en `/asesor/preventas` ✅
- Lista de pre-ventas activas con filtros por Operador y Cobertura ✅
- Paginación client-side implementada ✅
- Modal "Registrar llamada de compañía" con campos y loading state ✅
- Modal "Derivar a otro Asesor" con `<select>` dinámico de asesores (filtrados por rol `ASESOR` vía API) ✅
- Conectado al API (`/api/presales`, `/api/presales/{id}/calls`, `/api/presales/{id}/assign`) ✅

**⚠️ DEUDA**: El `@rendermode InteractiveServer` en un componente WASM es incorrecto semánticamente — debería ser `@rendermode InteractiveWebAssembly` o heredar del layout. Funciona porque el servidor hace el render, pero rompe el propósito del módulo WASM.

---

### T-17 · Dayan — Backend Documentos
**Estado: ✅ COMPLETO**

- `DocumentController`: `GET /api/orders/{id}/documents`, `POST /api/orders/{id}/documents` (multipart/form-data), `PATCH /api/documents/{id}/verify` ✅
- `UploadOrderDocumentUseCase`: guarda en ruta configurada, registra en `order_document` ✅
- `VerifyOrderDocumentUseCase`: actualiza estado y notas de verificación ✅
- Validaciones de archivo (vacío, tipo) y autenticación del actor presentes ✅

---

### T-18 · Patricia — Backend Formularios Dinámicos
**Estado: ✅ COMPLETO**

- `FormController`: `GET /api/forms/campaign/{id}/stage/{id}`, `GET /api/forms/{id}/fields`, `GET /api/forms/order/{id}/data`, `POST /api/forms/order/{id}/template/{id}`, `PUT /api/forms/data/{id}/status` ✅
- `FormRepository.GetTemplatesByCampaignStageAsync`, `GetFieldsByTemplateAsync`, `SaveOrderDataAsync` ✅
- `OrderDataRepository.GetByOrderAsync`, `UpdateFieldStatusAsync` ✅
- Integración con `INotificationService` al cambiar estado de campo ✅

---

### T-19 · Giuseppe — Frontend Nueva Venta + Formulario Dinámico (WASM)
**Estado: ✅ COMPLETO — Con un stub**

- `NewOrder.razor` en `/asesor/orders/new` ✅
- Selección de campaña carga etapas del API dinámicamente ✅
- Carga formulario dinámico según campaña + etapa ✅
- Renderiza campos por tipo: `text`, `select`, `date`, `file` ✅
- Validación client-side con `ValidationRegex` y `ValidationType` (DNI_ES, NIE_ES, PHONE_ES, CUPS_ENERGY) ✅
- Loading state en botón Guardar ✅

**⚠️ DEUDA — Stub hardcodeado:**
```csharp
var createOrderDto = new {
    idLead = 1,        // 🐛 HARDCODEADO — debería venir de la UI
    idUser = 101,      // 🐛 HARDCODEADO — debería ser el userId del JWT
    ...
};
```
> El `idLead` y `idUser` están hardcodeados. En producción, el asesor debe seleccionar el lead asociado a la venta.

---

### T-20 · Dayan — Backend Historial / Timeline
**Estado: ✅ COMPLETO — Calidad alta**

- `GET /api/orders/{id}/history` implementado ✅
- `GetSalesOrderHistoryUseCase` + `GetOrderHistoryTimelineAsync` ✅
- Timeline con `UNION ALL` que agrega: `STATUS_CHANGE`, `CUSTODY_TRANSFER`, `INCIDENT_DETECTED`, `INCIDENT_RESOLVED`, `DOCUMENT_UPLOADED`, `DOCUMENT_VERIFIED` ✅
- JOIN con `ext_ecosystem.collaborators` para nombres de actores ✅
- Ordenado por `timestamp ASC` ✅
- `ActorName` tiene fallback `"Sistema / Desconocido"` ✅

> Esta query es **la más compleja y mejor ejecutada del sprint**. 6 fuentes de eventos unificadas en un timeline.

---

### T-21 · Patricia — Ficha Alterna y Aprobaciones
**Estado: ⚠️ PARCIAL**

**Implementado:**
- `ApprovalController`: `POST /api/orders/{id}/approvals` (con validación de rol `SUPERVISOR`) ✅
- `PATCH /api/approvals/{id}` (aprobar/rechazar) ✅
- Notificación interna al aprobar ✅
- `AlternateProfileController`: `GET` y `POST` `/api/orders/{id}/alternate-profile` ✅

**❌ FALTANTE:**
- La validación de rol `SUPERVISOR` en `ApprovalController` está **duplicada**: una vez con `[Authorize(Roles = "SUPERVISOR")]` y otra vez dentro del método con `if (userRole != "SUPERVISOR")`. Esto es redundante y confuso, pero no es un bug.
- **Más importante**: `POST /api/orders/{id}/approvals` debería crear una *solicitud de excepción* (que luego el supervisor aprueba), no registrar directamente la aprobación. El flujo actual invierte la lógica: quien solicita la excepción no debería ser el supervisor, sino el asesor o backoffice.

> **⚠️ REVISAR CON EQUIPO**: Definir si `POST /approvals` crea la solicitud (rol: ASESOR/BACKOFFICE) y `PATCH /approvals/{id}` la resuelve (rol: SUPERVISOR). El código actual asigna ambas acciones al SUPERVISOR.

---

### T-22 · Giuseppe — Lista de Ventas del Asesor
**Estado: ⚠️ PARCIAL — Bug de endpoint**

**Implementado:**
- `Orders.razor` en `/asesor/orders` ✅
- Columnas: N° orden, cliente, campaña, estado (badge con color), substatus, fecha, días ✅
- Filtros: estado, campaña, fecha desde/hasta ✅
- Paginación ✅
- Click en fila muestra snackbar (detalle pendiente) ⚠️

**🐛 BUG CRÍTICO — Endpoint incorrecto:**
```csharp
// Orders.razor línea 243
var query = $"/api/campaigns/orders?userId={currentUserId}";
```
> El endpoint `GET /api/campaigns/orders` **no existe**. El correcto es `GET /api/orders`. Este bug haría que la pantalla de órdenes esté completamente vacía en runtime.

**⚠️ DEUDA:** La vista de detalle de una orden (`GoToDetail`) solo muestra un snackbar de "próximamente". No hay pantalla de detalle implementada.

---

## RESUMEN DE ENTREGA

| Tarea | Dev | Estado | Prioridad Fix |
|-------|-----|--------|---------------|
| T-01 Setup | Dayan | ✅ COMPLETO | — |
| T-02 Modelos Core | Patricia | ✅ COMPLETO | — |
| T-03 Smoke Test BD | Giuseppe | ⚠️ Sin entregable | BAJA |
| T-04 Auth Login | Dayan | ✅ COMPLETO | — |
| T-05 Modelos RRHH/Geo | Patricia | ⚠️ PARCIAL | BAJA |
| T-06 Frontend Setup | Giuseppe | ✅ COMPLETO | — |
| T-07 JWT Middleware + /me | Dayan | ✅ COMPLETO | — |
| T-08 Repos Campañas/Catálogos | Patricia | ✅ COMPLETO | — |
| T-09 Login Frontend | Giuseppe | ✅ COMPLETO | — |
| T-10 TL Contratos S1 | TL | ✅ COMPLETO | — |
| T-11 Leads Backend | Dayan | ⚠️ BUG + FALTANTE | **ALTA** |
| T-12 PreVenta Backend | Patricia | ⚠️ BUG Seguridad | **ALTA** |
| T-13 Layout Frontend | Giuseppe | ✅ COMPLETO | — |
| T-14 Órdenes Backend | Dayan | ✅ COMPLETO | — |
| T-15 Middleware Custodia | Patricia | ✅ COMPLETO | — |
| T-16 PreVenta Frontend | Giuseppe | ⚠️ rendermode incorrecto | MEDIA |
| T-17 Docs Backend | Dayan | ✅ COMPLETO | — |
| T-18 Forms Backend | Patricia | ✅ COMPLETO | — |
| T-19 Nueva Venta Frontend | Giuseppe | ⚠️ idLead/idUser hardcodeado | **ALTA** |
| T-20 Historial/Timeline | Dayan | ✅ COMPLETO | — |
| T-21 Aprobaciones/Ficha | Patricia | ⚠️ Flujo de roles invertido | MEDIA |
| T-22 Lista Ventas Frontend | Giuseppe | 🐛 Endpoint incorrecto | **ALTA** |

---

## ISSUES CRÍTICOS PARA RESOLVER ESTA SEMANA

### 🔴 BUG #1 — `UpdateLeadStatusUseCase` crea datos de prueba en producción
**Archivo:** `CRM.ApiHub/Application/UseCases/Leads/UpdateLeadStatusUseCase.cs`
**Acción:** Eliminar el bloque de creación automática de `SalesOrder` + `OrderIncident` (líneas 46-86).

### 🔴 BUG #2 — `GET /presales` expone datos de otros usuarios
**Archivo:** `CRM.ApiHub/Api/Controllers/PreSaleController.cs`
**Acción:** Reemplazar `[FromQuery] int userId` por extracción del claim del JWT.

### 🔴 BUG #3 — `Orders.razor` llama endpoint inexistente
**Archivo:** `CRM.WebFrontend.Client/Pages/Asesor/Orders.razor`, línea 243
**Acción:** Cambiar `/api/campaigns/orders` → `/api/orders`.

### 🟡 DEUDA #4 — `NewOrder.razor` tiene `idLead` y `idUser` hardcodeados
**Archivo:** `CRM.WebFrontend.Client/Pages/Asesor/NewOrder.razor`, línea 336-339
**Acción:** `idUser` debe venir del JWT. `idLead` necesita un selector de lead en el formulario.

### 🟡 DEUDA #5 — Validación de transición de estado en Leads no implementada
**Especificación T-11:** `sales_service.validate_status_transition`
**Acción:** Implementar en `UpdateLeadStatusUseCase` llamada a la función SQL de validación.

### 🟡 DEUDA #6 — Flujo de Aprobaciones con roles invertidos
**Archivo:** `ApprovalController.cs`
**Acción:** Definir con equipo: `POST` crea solicitud (asesor/backoffice), `PATCH` la resuelve (supervisor).

### 🔴 BUG #7 — `Product.cs`: PK mapeada con nombre de columna incorrecto
**Archivo:** `CRM.ApiHub/Domain/Entities/Product.cs`, línea 11
```csharp
// ACTUAL (INCORRECTO)
[Column("id")]
public int Id { get; set; }

// CORRECTO — la PK real en product_service.product es id_prod
[Column("id_prod")]
public int Id { get; set; }
```
**Impacto:** `CatalogRepository.GetProductsAsync()` hace `SELECT p.*` — Dapper no puede mapear `id_prod` a `Id` porque el `[Column]` dice `"id"`. Todos los productos retornan con `Id = 0`.

### 🔴 BUG #8 — `SalesOrderCustodyLog.cs`: columnas completamente desfasadas de la tabla real
**Archivo:** `CRM.ApiHub/Domain/Entities/SalesOrderCustodyLog.cs`

El POCO tiene columnas inventadas que no existen en `sales_service.sales_order_custody_log`. Las columnas reales (evidenciadas en el SQL del timeline del repositorio) son:

| POCO actual | Columna real en BD |
|-------------|-------------------|
| `SalesOrderId` | `id_order` |
| `PreviousCustodianId` | `from_user_id` |
| `CurrentCustodianId` | `to_user_id` |
| `Observations` | `comment` |
| `ActionDate` | `register` |
| *(no existe)* | `from_role` |
| *(no existe)* | `to_role` |
| *(no existe)* | `transfer_type` |

**Impacto actual:** No bloquea en runtime porque el POCO no se usa directamente en ninguna query (el timeline usa proyección anónima). **Riesgo futuro:** Si Dayan o Patricia intentan usar `QueryAsync<SalesOrderCustodyLog>()` para implementar el módulo de custodia de S3, fallará en producción.

### 🟡 DEUDA #9 — `Collaborator.cs`: columnas desfasadas del FDW real
**Archivo:** `CRM.ApiHub/Domain/Entities/Collaborator.cs`

La tabla real `ext_ecosystem.collaborators` usa `name`, `paternal_surname`, `maternal_surname`, `id_user`. El POCO usa `first_name`, `last_name`, `email`. No impacta el runtime actual pero requiere corrección antes de S3 cuando se necesite hacer queries directas de colaboradores.

---

## CONTRATOS S3 — Supervisión, BackOffice, Notificaciones

Para la siguiente semana se necesitan endpoints:

| Módulo | Endpoint | Notas |
|--------|----------|-------|
| Supervisor | `GET /supervisor/pipeline` | Todas las órdenes con filtros por equipo |
| Supervisor | `GET /supervisor/team` | Rendimiento de equipo (ventas/conversión) |
| Supervisor | `GET /supervisor/alerts` | Alertas activas del equipo |
| BackOffice | `GET /backoffice/queue` | Bandeja de documentos pendientes |
| BackOffice | `PATCH /backoffice/orders/{id}/custody` | Transferir custodia |
| Notificaciones | `GET /notifications/me` | Alertas del usuario autenticado |
| Notificaciones | `PATCH /notifications/{id}/read` | Marcar como leída |

El `Dashboard.razor` (BackOffice) ya tiene UI implementada contra `MockBackofficeService`. La próxima semana conectar contra el API real.
