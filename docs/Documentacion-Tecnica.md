# Documentación Técnica — Sistema de Solicitudes de Envío (ShippingRequestSystem)

**Versión del documento:** 1.0 · **Fecha:** 2026-10-26 · **Repositorio:** `ShippingRequestSystem` (rama `main`)

## 1. Resumen del sistema

Aplicación interna de MEAX para solicitar, cotizar, aprobar y dar seguimiento a envíos de material (nacionales e internacionales). Un solicitante captura el envío; Trade Staff sube las cotizaciones de paquetería; el solicitante elige una; si aplica se llena un formulario de Premium Freight; y la solicitud recorre una cadena formal de aprobación hasta quedar aprobada o rechazada.

Se despliega como **sub-aplicación de IIS** (`http://meax.one/ShippingRequest`) y se integra con **Meax One (meaxHub)** para el inicio de sesión (SSO).

## 2. Stack tecnológico

| Capa | Tecnología |
|---|---|
| Plataforma | .NET 8 (`net8.0`) |
| Interfaz | Blazor Web App con render interactivo de servidor (Interactive Server) |
| Acceso a datos | Entity Framework Core 8 + SQL Server |
| Autenticación | JWT HS256 del hub Meax One + cookie local de sesión |
| Estilos | Sistema de diseño MEAX One (`meaxone-template.css`, clases `mo-*`) |
| Hospedaje | IIS (ASP.NET Core Module) en `MEAXS066` |

Paquetes NuGet principales: `Microsoft.AspNetCore.Authentication.JwtBearer` 8.0.11, `Microsoft.EntityFrameworkCore.SqlServer` / `.Design` / `.Tools` 8.0.28.

## 3. Arquitectura

### 3.1 Estructura de carpetas (`ShippingRequestSystem/`)

| Carpeta | Contenido |
|---|---|
| `Auth/` | Autenticación: `HubAuthExtensions` (producción), `DevAuthExtensions` (desarrollo), `MeaxClaims`, `AuthMode` |
| `Components/Pages/` | Páginas (cada una con su `@page`) |
| `Components/Pages/Admin/` | Paneles de administración |
| `Components/Shared/` | Componentes reutilizables (formularios, tablas, selector de clientes, etc.) |
| `Components/Layout/` | `MainLayout`, `NavMenu`, `EmptyLayout`, `DevModeBanner` |
| `Controllers/` | `AuthController` (login de desarrollo y logout) |
| `Data/` | `AppDbContext` |
| `Models/` | Entidades EF Core y enums |
| `ViewModels/` | Modelos de captura de los formularios |
| `Services/` | Lógica de negocio (ver sección 8) |
| `Middleware/` | `RequireAuthMiddleware` |
| `Helpers/` | Extensiones de enums (nombres para mostrar, variantes de badge) |
| `Migrations/` | Migraciones de EF Core |
| `wwwroot/` | CSS, JS, imágenes, favicon y `uploads/` (archivos subidos) |

### 3.2 Pipeline de la aplicación (`Program.cs`)

Orden de middleware:

1. `UseExceptionHandler("/Error")` y `UseHsts()` (solo fuera de Development)
2. `UseHttpsRedirection()`, `UseStaticFiles()`
3. `UseAuthentication()`
4. `UseHubSsoTokenBridge()` (solo modo producción)
5. `UseAuthorization()`
6. `RequireAuthMiddleware` (exige sesión en toda la app)
7. `UseAntiforgery()`, `MapControllers()`, `MapRazorComponents<App>().AddInteractiveServerRenderMode()`

### 3.3 Inyección de dependencias

Servicios registrados como *scoped*: `IApprovalService`, `IShippingRequestService`, `IQuoteService`, `IPermissionService`, `IPartCatalogService`, `ICustomerCatalogService`, `IEmailService`, `CurrentUserService`, `FileService`, `FolioService`. Los servicios de datos usan `IDbContextFactory<AppDbContext>` (un contexto por operación) para evitar problemas de concurrencia en Blazor Server.

### 3.4 Modo de render y layout

Las páginas interactivas llevan `@rendermode InteractiveServer`. `MainLayout` es **estático** (sin `@rendermode`): recibe el contenido de la página como parámetro y no puede ser interactivo; por eso el menú de usuario y el colapso del sidebar usan JavaScript plano (`moToggleSidebar`, `moToggleUserMenu`).

## 4. Modelo de datos

Base de datos: `meax_db` en SQL Server `10.228.25.66`. Prefijo de las tablas propias: `prcPurchase*`.

### 4.1 Tablas

| Tabla (entidad) | Propósito |
|---|---|
| `prcPurchaseShippingRequest` (`ShippingRequest`) | Solicitud de envío (datos del solicitante, comprador, destinatario, envío, factura, pago, evidencias, estado, costo) |
| `prcPurchaseShippingPackage` (`ShippingPackage`) | Pallet o caja. Una caja puede estar dentro de un pallet (`ParentPalletId`). Guarda peso bruto y dimensiones del contenedor |
| `prcPurchaseShippingItem` (`ShippingItem`) | Partida dentro de un paquete (número de parte, cantidad, UOM, peso neto y precio; el peso bruto y las dimensiones están en el paquete) |
| `prcPurchaseShippingQuote` (`ShippingQuote`) | Cotización de paquetería (proveedor, monto, PDF); una queda `IsSelected` |
| `prcPurchasePremiumFreightInfo` (`PremiumFreightInfo`) | Datos del formulario Premium Freight (folio `PF-`) |
| `prcPurchaseApprovalStep` (`ApprovalStep`) | Pasos de la cadena de aprobación de cada solicitud |
| `prcPurchaseApprovalRolePermission` (`ApprovalRolePermission`) | Quién ocupa cada rol de aprobación (administrado en `/admin/permissions`) |
| `prcPurchaseSystemAdmin` (`SystemAdmin`) | Administradores del sistema |
| `prcPurchaseFolioCounter` (`FolioCounter`) | Contador de folios por prefijo y año |
| `prcPurchasePartCatalog` (`PartCatalog`) | Catálogo de números de parte (autocompletar) |
| `prcPurchaseCustomerCatalog` (`CustomerCatalog`) | Catálogo de clientes/destinos de Trade (86 registros iniciales) |
| `meax_all_user` (`MeaxUser`) | **Tabla externa, solo lectura.** Directorio de usuarios: login de red, nombre, email, departamento, puesto |

### 4.2 Relaciones principales

- `ShippingRequest` 1—N `ShippingPackage`, `ShippingQuote`, `ApprovalStep`; 1—1 `PremiumFreightInfo`.
- `ShippingPackage` 1—N `ShippingItem`; autorrelación `ParentPallet` → `ChildBoxes` (caja dentro de pallet).
- `SystemAdmin` es único por `MeaxUserId`; `PartCatalog.PartNumber` es único.
- `CustomerCatalog.CustomerCode` **no** es único: un cliente con varios domicilios se registra una vez por domicilio.

### 4.3 Peso y dimensiones

Peso bruto y dimensiones (largo, ancho, alto) se capturan **una vez por caja o pallet**, no por partida. El peso neto sí es por partida. Regla de validación: por cada contenedor, el peso bruto debe ser **mayor** que la suma de los pesos netos de sus partidas (si el peso bruto es 0, no se valida).

### 4.4 Folios

`FolioService` genera `SR-AAAA-NNNN` (solicitud) y `PF-AAAA-NNNN` (Premium Freight) con una transacción *Serializable* dentro de la estrategia de ejecución de EF (necesario por `EnableRetryOnFailure`).

### 4.5 Enums y persistencia como enteros

EF Core guarda los enums como **enteros según el orden de declaración**. Nunca se debe reordenar ni insertar miembros en medio de un enum con datos ya guardados; solo renombrar o agregar al final. `ParcelCarrier` usa valores explícitos (`FedEx=1, UPS=2, DHL=3, Otro=4`) porque se retiró `SHL`.

`RequestStatus`:

| Valor | Estado | Significado |
|---|---|---|
| 0 | `Draft` | Borrador (wizard sin terminar) |
| 1 | `PendingTradeStaffQuotes` | Trade Staff debe subir IGI, Incoterm, Tipo de Operación y cotizaciones |
| 2 | `PendingUserQuoteSelection` | El solicitante elige la cotización |
| 3 | `PendingPremiumFreight` | El solicitante llena Premium Freight |
| 4 | `PendingTradeStaffApproval` | Aprobación formal de Trade Staff |
| 5 | `PendingTradeCoordinator` | Aprobación de Trade Coordinator |
| 6 | `PendingTradeManager` | Aprobación de Trade Manager |
| 7 | `PendingUserManager` | Aprobación del Manager del solicitante |
| 8 | `PendingProcurementManager` | Aprobación de Procurement Manager |
| 9 | `PendingSalesManager` | Aprobación de Sales Manager |
| 10 | `PendingFinanceManager` | Aprobación de Finanzas |
| 11 | `PendingPresident` | Aprobación de Presidencia |
| 12 | `Approved` | Aprobada |
| 13 | `Rejected` | Rechazada |

El orden numérico **no** es el orden del flujo; el orden real lo define `ApprovalService.BuildApprovalFlow`.

## 5. Flujo de negocio

### 5.1 Ciclo de vida de una solicitud

1. **Captura (wizard `/requests/new`, 6 pasos):** Solicitante → Direcciones → Envío → Paquetes → Evidencias → Observaciones. Se autoguarda un **borrador** al avanzar de paso; cada usuario retoma su borrador (se busca por email). Al enviar, la solicitud pasa a `PendingTradeStaffQuotes`, se genera el folio `SR-` y se crean los pasos de aprobación.
2. **Trade Staff (`/requests/{id}/staff-review`):** captura % IGI, Incoterm, Tipo de Operación (A1/RT, obligatorio y vacío al inicio), Certificado de Origen (opcional, PDF) y **al menos 1 cotización** (máx. 5; el formulario abre con una y se agregan más con "+ Agregar Cotización"; con cuenta de cliente o proveedor basta una), cada una con proveedor, monto y PDF. Al guardar → `PendingUserQuoteSelection`.
3. **Selección de cotización (`/requests/{id}/select-quote`):** el solicitante elige una; se guarda su monto como costo aproximado. Si la factura es **Comercial** o el costo es **≥ 5,000 USD** → `PendingPremiumFreight`; si no, entra directo a la cadena (`PendingUserManager`).
4. **Premium Freight (`/requests/{id}/premium-freight`):** formulario con direcciones de recolección/entrega, causa y contramedidas; genera folio `PF-`. Según el pago se activan pasos opcionales (ver 5.2). Después → `PendingUserManager`.
5. **Cadena formal de aprobación** (ver 5.2) hasta `Approved`, o `Rejected` si alguien rechaza.
6. **Finanzas:** el Finance Manager de la solicitud puede enviar un resumen de facturación por correo (ver sección 10).

### 5.2 Cadena formal de aprobación

Los pasos se crean al enviar la solicitud, en este orden (`StepOrder` 1–8):

| # | Rol | Obligatorio | Quién aprueba |
|---|---|---|---|
| 1 | User Manager | Siempre | El Manager elegido por el solicitante en el paso 1 |
| 2 | Sales Manager | Solo si el pago de Premium Freight es *Reembolsable por Cliente* | Persona asignada al rol en `/admin/permissions` |
| 3 | Procurement Manager | Solo si el pago de Premium Freight es *Reembolsable por Proveedor* | Ídem |
| 4 | Finance Manager | Siempre | Ídem |
| 5 | President | Siempre | Ídem |
| 6 | Trade Staff | Siempre | Ídem (aprobación formal, distinta de la subida de cotizaciones) |
| 7 | Trade Manager | Siempre | Ídem |
| 8 | Trade Coordinator | Siempre | Ídem |

`ProcessApprovalAsync` marca el paso, y si fue aprobado avanza al siguiente paso obligatorio incompleto (asignando el `RequestStatus` correspondiente); si no quedan pasos → `Approved`; un rechazo → `Rejected`. Solo puede aprobar quien coincide por **email** con `ApproverEmail` del paso.

### 5.3 Pantalla "Pendientes" (`/approvals`)

`GetPendingStepsForApproverAsync` muestra a cada usuario:

- **Solicitudes en `PendingTradeStaffQuotes`:** a quienes tienen el rol *Trade Staff* en `ApprovalRolePermission`. Este caso se resuelve aparte porque ocurre *antes* de que exista un paso de aprobación aplicable.
- **Solicitudes dentro de la cadena:** solo el paso de **menor `StepOrder` incompleto y obligatorio** de cada solicitud, si está asignado a ese usuario (evita duplicados y mostrar pasos que aún no son su turno).

### 5.4 Reglas de validación del wizard

Pasos de la interfaz: 1 Solicitante · 2 Direcciones · 3 Envío · 4 Paquetes · 5 Evidencias · 6 Observaciones.

- **Paso 1 (Solicitante):** nombre, email, departamento, unidad de negocio, Manager (de la lista) y, si el alcance es Nacional, la categoría (**Compras** o **Traslado**).
- **Paso 2 (Direcciones):** razón social y dirección del destinatario.
- **Paso 3 (Envío):** propósito del envío, al menos un tramo de flete e indicar si es estibable; fecha de entrega (si se captura) **posterior a hoy**; si el pago es *Receiver*, **Parcel Account obligatorio** y se elige paquetería (FedEx, UPS, DHL, Otro).
- **Paso 4 (Paquetes):** peso bruto mayor que la suma de netos por contenedor (modal de error).
- **Paso 5 (Evidencias):** **4 fotos obligatorias** (Peso Neto, Peso Bruto, Dimensiones, Empaque Final; JPG/PNG, máx. 8 MB). En "Empaque Final" se indica que si el paquete lleva etiqueta también debe fotografiarse.
- **Paso 6 (Observaciones):** si no hay Folio de Baja de Material (MX-7104-F1), se exige justificación (se agrega a las observaciones).
- El **Incoterm, el COO y el Tipo de Operación** los determina Trade Staff, no el solicitante.

### 5.5 Catálogos usados en la captura

- **Manager/Supervisor:** lista con buscador sobre `meax_all_user`, limitada a puestos `DGM`, `MGR`, `AMGR`.
- **Clientes:** selector en *Comprador* y *Destinatario* que autollena razón social, RFC y dirección desde `CustomerCatalog` (los campos siguen editables; se puede capturar a mano si el cliente no está).
- **Números de parte:** autocompletar en las partidas desde `PartCatalog`.

## 6. Autenticación y autorización

### 6.1 Modos de autenticación

El modo se decide en `Program.cs`:

```
modo dev = Environment.IsDevelopment() && Auth:DevLogin == true
```

Doble candado: en IIS el entorno es *Production*, así que el modo dev es imposible ahí aunque el indicador se copie por error.

| | Producción (Meax One) | Desarrollo |
|---|---|---|
| Registro | `AddHubAuth(...)` | `AddDevAuth()` |
| Login | Redirección al hub (SSO) | Página `/login` con usuario libre + contraseña fija (`Auth:DevPassword`) |
| Sesión | JWT del hub → cookie local `ShippingRequestSystem.Auth` | Cookie `ShippingRequestSystem.Auth` |
| Indicador | — | Banner "MODO DESARROLLO — login sin Meax One" |

En ambos modos el principal tiene **exactamente los mismos claims**, por lo que el resto de la app no tiene condicionales de modo.

### 6.2 Flujo en producción (SSO)

1. Request sin sesión → el esquema de autenticación hace *challenge* y redirige a `{Hub:BaseUrl}/auth/sso?system={Hub:SystemCode}&returnUrl=<URL absoluta con PathBase>`.
2. El hub autentica y regresa a `returnUrl?meax_token=<jwt de 5 min>&meax_role=<rol>`.
3. El JWT se valida (HS256; emisor `meaxHub`, audiencia `meax-services`, tolerancia de reloj 1 min). El token se lee de la cookie `.MEAX.JWT` (mismo host) o de `?meax_token=`.
4. `UseHubSsoTokenBridge` convierte el token recibido por query en una **cookie local de sesión** (8 h, deslizante) y redirige a la misma URL sin `meax_token`/`meax_role`.
5. **Latido:** `App.razor` hace `fetch('{Hub:BaseUrl}/auth/heartbeat', POST, credentials: include)` cada 12 minutos para mantener viva la cookie `.MEAX.JWT`; si el hub no responde se ignora silenciosamente.
6. **Logout** (`/auth/logout`): borra la cookie local y `.MEAX.JWT`, y regresa a Meax One.

### 6.3 Claims y datos del usuario

Claims del JWT: `sub`/`PcLoginId`, `DisplayName`, `Department`, `Position`, `EmployeeId` (nómina, puede venir vacío), `SystemRole` (solo en tokens SSO), `jti`.

`CurrentUserService` los expone (`Username` = PcLoginId, `DisplayName`, `Department`, `Position`, `EmployeeId`, `SystemRole`). **El JWT no trae email**: `Email` se resuelve una vez contra `meax_all_user` por el `PcLoginId`, porque todo el sistema de aprobaciones compara por email. La propiedad síncrona `Email` usa `Task.Run(...)` para evitar un *deadlock* de Blazor (ver sección 13).

### 6.4 Roles y permisos

- **Roles de aprobación** (Trade Staff, Trade Coordinator, Trade Manager, Procurement Manager, Sales Manager, Finance Manager, President): se asignan por persona en `/admin/permissions` (tabla `ApprovalRolePermission`). *User Manager* es por solicitud.
- **Administrador:** pertenece a la tabla `SystemAdmin` (`/admin/admins`); controla el acceso a `/admin/*` y a la sección Administración del menú.
- `SystemRole` (claim del hub) se expone pero **no** se usa hoy para autorizar nada en la app.

### 6.5 Autorización global

`RequireAuthMiddleware` exige sesión en toda la app y dispara el *challenge* del esquema activo. Quedan públicos: `/_framework`, `/_blazor`, `/login`, `/auth`, `/css`, `/js`, `/favicon` y cualquier endpoint con `[AllowAnonymous]`. **No** se usa `RequireAuthorization()`/política de fallback sobre `MapRazorComponents`, porque eso también protege `_framework/blazor.web.js` y deja la app colgada antes de poder autenticar.

## 7. Páginas y rutas

| Ruta | Página | Quién la usa |
|---|---|---|
| `/` | `Home` | Todos (dashboard, solicitudes recientes) |
| `/requests` | `RequestList` | Todos |
| `/requests/new` | `NewRequest` | Solicitante (wizard de 6 pasos) |
| `/requests/{id}` | `RequestDetail` | Todos; muestra acciones según estado y rol |
| `/requests/{id}/staff-review` | `TradeStaffReview` | Trade Staff |
| `/requests/{id}/select-quote` | `SelectQuote` | Solicitante |
| `/requests/{id}/premium-freight` | `PremiumFreightForm` | Solicitante |
| `/approvals` | `Approvals` | Aprobadores |
| `/admin` | `AdminPanel` | Administradores |
| `/admin/permissions` | `PermissionsPanel` | Administradores (roles de aprobación) |
| `/admin/admins` | `AdminsPanel` | Administradores |
| `/admin/parts` | `PartCatalogPanel` | Administradores (números de parte) |
| `/admin/customers` | `CustomerCatalogPanel` | Administradores (catálogo de clientes) |
| `/login` | `Login` | Solo modo desarrollo |
| `/auth/login` (POST), `/auth/logout` (GET) | `AuthController` | Login dev / cierre de sesión |
| `/Error` | `Error` | Manejo de excepciones |

`Counter` y `Weather` son páginas de la plantilla sin uso real.

## 8. Servicios

| Servicio | Responsabilidad |
|---|---|
| `ShippingRequestService` | Crear/guardar borradores y solicitudes, mapear paquetes e items, consultas |
| `QuoteService` | Guardar IGI/Incoterm/Tipo de Operación/cotizaciones, seleccionar cotización, guardar Premium Freight |
| `ApprovalService` | Construir la cadena, procesar aprobaciones/rechazos, calcular Pendientes |
| `PermissionService` | Búsqueda/listado de usuarios (`meax_all_user`), roles de aprobación, administradores |
| `PartCatalogService` | Catálogo de números de parte |
| `CustomerCatalogService` | Catálogo de clientes (listar, agregar, editar, activar/desactivar) |
| `FolioService` | Folios `SR-`/`PF-` |
| `FileService` | Guardado de PDFs (cotizaciones, COO) y fotos de evidencia |
| `EmailService` | Resumen de facturación a Finanzas |
| `CurrentUserService` | Identidad del usuario actual a partir de los claims |

## 9. Archivos subidos

Se guardan en `wwwroot/uploads/`, con nombre GUID, y la ruta relativa (`/uploads/...`) queda en la base de datos:

| Carpeta | Contenido | Límite |
|---|---|---|
| `uploads/evidence/` | Fotos de evidencia (JPG/PNG) | 8 MB |
| `uploads/quotes/` | PDF de cotizaciones | 10 MB |
| `uploads/coo/` | PDF del Certificado de Origen | 10 MB |

Los archivos se leen y guardan **en el momento de seleccionarlos** (no al presionar Guardar): al seleccionar, la interfaz reemplaza el `<InputFile>` por el nombre del archivo y el navegador pierde la referencia (`_blazorFilesById`). Al mostrarlos se usa la ruta sin `/` inicial para respetar el `PathBase`.

## 10. Correo

`EmailService.SendFinanceSummaryAsync` envía un resumen HTML (datos generales, paquetes con su peso bruto/dimensiones, items, PDF de la cotización elegida adjunto) a `Smtp:FinanceRecipients`. Lo dispara el Finance Manager de la solicitud con el botón "Enviar a Finanzas" (estado `PendingFinanceManager` o `Approved`). Si `Smtp:Host` o los destinatarios están vacíos, lanza un error indicando que no está configurado.

## 11. Configuración

### 11.1 `appsettings.json`

| Clave | Valor / uso |
|---|---|
| `Jwt:Issuer` / `Jwt:Audience` | `meaxHub` / `meax-services` |
| `Hub:BaseUrl` | `http://meax.one` (también destino del logo de la barra superior) |
| `Hub:SystemCode` | `SHP` (código con el que el sistema está registrado en el hub) |
| `Smtp:Host`, `Port`, `EnableSsl`, `User`, `Password`, `From`, `FinanceRecipients` | Correo a Finanzas (hoy `Host` y destinatarios vacíos) |
| `Logging`, `AllowedHosts` | Estándar |

### 11.2 Secretos: solo variables de entorno

| Variable de entorno | Uso |
|---|---|
| `ConnectionStrings__DefaultConnection` (o `ConnectionStrings__MeaxDb`) | Cadena de conexión a `meax_db` |
| `Jwt__Secret` | Secreto HS256, **idéntico byte a byte al del hub**, mínimo 32 bytes |
| `Smtp__Password` (si aplica) | Contraseña SMTP |

Si falta la cadena de conexión o `Jwt:Secret` la aplicación falla al arrancar con un mensaje explícito. En el servidor, las variables deben ser **de sistema** y, tras cambiarlas, hay que ejecutar `iisreset` (reciclar el Application Pool no basta).

### 11.3 `appsettings.Development.json`

Solo existe para desarrollo e incluye `Auth:DevLogin`, `Auth:DevPassword` (por omisión `dev`) y `Auth:DevUser` (`PcLoginId`, `DisplayName`, `Department`, `Position`, `EmployeeId`, `SystemRole`). **No debe copiarse al servidor.**

## 12. Despliegue

### 12.1 Entorno de producción

- Servidor `MEAXS066`, sitio IIS como **sub-aplicación** `/ShippingRequest`; ruta física `\\MEAXS066\inetpub\Services\ShippingRequest`.
- Base de datos `meax_db` en `10.228.25.66`. El vínculo `EBSDB` (Oracle EBS) existe en ese servidor pero la aplicación **no** lo usa.
- ASP.NET Core Module informa el `PathBase` automáticamente; no se usa `UsePathBase` en código.

### 12.2 Reglas para que funcione bajo un PathBase

- Enlaces y `NavigateTo` **sin `/` inicial** (relativos al `<base href>`); `App.razor` calcula `<base href>` con el `PathBase`.
- En C#, redirecciones con `Url.Content("~/...")`; URLs absolutas incluyendo `PathBase`.
- Las URLs de otros sistemas salen de configuración (`Hub:BaseUrl`).

### 12.3 Procedimiento de publicación

1. Publicar desde Visual Studio con el perfil `FolderProfile1` (`DeleteExistingFiles=false`) o con `dotnet publish -c Release`. Debe ser una **publicación completa**: una copia parcial deja fuera DLLs (por ejemplo `System.IdentityModel.Tokens.Jwt.dll`) y el sitio responde 500.
2. Aplicar migraciones pendientes (`dotnet ef database update`): en producción **no** se aplican automáticamente (solo en Development).
3. Restablecer el permiso de escritura sobre `wwwroot\uploads` (ver 12.4).
4. Verificar que las variables de entorno existan y reiniciar IIS si cambiaron.
5. Probar abriendo el sitio; ante un 500, revisar el **Visor de eventos de Windows** (Aplicación, origen `.NET Runtime`/`IIS AspNetCoreModuleV2`).

### 12.4 Permisos de `uploads`

Con `DeleteExistingFiles=false` (valor actual) las publicaciones conservan `wwwroot\uploads` y su permiso. Si la carpeta se recrea (publicación con borrado, servidor nuevo), se pierde el permiso del App Pool y al subir archivos aparece "Access to the path ... is denied"; entonces ejecutar:

```
icacls "C:\inetpub\Services\ShippingRequest\wwwroot\uploads" /grant "IIS AppPool\<NOMBRE_DEL_APP_POOL>:(OI)(CI)M" /T
```

Alternativa robusta (pendiente): mover `uploads` fuera de `wwwroot` y servirlo con un `UseStaticFiles` propio.

### 12.5 Perfiles de publicación

- `FolderProfile1.pubxml` → `\\MEAXS066\inetpub\Services\ShippingRequest`, `DeleteExistingFiles=false` (**no cambiar a `true`**: borraría `wwwroot\uploads` con todos los archivos subidos).
- `FolderProfile.pubxml` → `\\meaxs013\inetpub\shippingRequest`, `DeleteExistingFiles=false` (apunta al servidor anterior; no usar).

## 13. Desarrollo local

1. Requisitos: SDK de .NET 8 (o superior), acceso a SQL Server `10.228.25.66` y las variables de entorno `ConnectionStrings__MeaxDb` (o `...DefaultConnection`) definidas.
2. Ejecutar: `dotnet run` (perfil `http` → `http://localhost:5231`) o el perfil de `.claude/launch.json`.
3. Iniciar sesión en `/login` con el usuario de `Auth:DevUser` (por omisión `KK51583`) y la contraseña `dev`.
4. En Development la aplicación ejecuta `Database.Migrate()` al arrancar (cuidado: apunta a la misma base compartida).
5. Migraciones: `dotnet ef migrations add <Nombre>` y `dotnet ef database update`.

Migraciones existentes: `InitialCreate`, `AddQuotesAndNewStatuses`, `AddQuotesAndStaffFields`, `AddTradeStaffApprovalStatus`, `AddQuoteFilePath`, `AddPermissionsAndAdmins` (+`2`), `AddFoliosAndCounter`, `AddFleteEvidenciaCatalogoBorrador`, `AddNationalCategoryAndCooDocument`, `RemoveItemGrossWeightDims`, `AddCustomerCatalog` (incluye la carga inicial de 86 clientes), `MakeOperationTypeNullable` (Tipo de Operación vacío hasta que lo elige Trade Staff).

## 14. Convenciones y trampas conocidas

- **No reordenar enums** con datos guardados (ver 4.5).
- **`blazor.web.js` debe ser público:** no gatear `MapRazorComponents` con `RequireAuthorization()` ni con política de fallback.
- **Deadlock con `CurrentUser.Email`:** resuelve contra la base de datos; llamarlo de forma síncrona dentro de `OnInitializedAsync` bloquea el contexto de Blazor. La propiedad usa `Task.Run`; en código nuevo preferir `await CurrentUser.GetEmailAsync()`.
- **Archivos:** leer el `IBrowserFile` en el mismo handler de `OnChange` (ver sección 9).
- **`Jwt:Secret` ≥ 32 bytes:** con uno más corto la validación falla en cada request y el síntoma es un bucle silencioso hacia `/auth/sso`; la app lo valida al arrancar.
- **Rutas con `PathBase`:** nunca escribir `href="/..."` ni `NavigateTo("/...")`.
- **Estilos:** `box-sizing: border-box` global en `app.css` (la plantilla MEAX One lo asume); no hay emojis decorativos ni morado en la UI.
- **`meaxone-template.css` y `meaxone-table.js`** son de la plantilla MEAX One; no se modifican.

## 15. Deuda técnica y riesgos conocidos

1. **Aprobación fuera de orden.** `RequestDetail` muestra "Aprobar/Rechazar" a quien tenga *algún* paso incompleto asignado, y `ProcessApprovalAsync` solo valida el email del paso, no que los pasos anteriores estén completos. Un usuario con un rol posterior podría aprobar antes de su turno (la pantalla Pendientes sí respeta el orden). Se recomienda validar el orden en el servicio.
2. **Un solo aprobador por rol en la cadena formal.** `BuildApprovalFlow` toma la primera persona activa de cada rol; no soporta varios aprobadores ni suplentes. La excepción es la captura de IGI/cotizaciones de Trade Staff (paso previo a la cadena), que ya puede hacerla cualquier persona con ese rol.
3. **Correo a Finanzas sin configurar.** `Smtp:Host` y `Smtp:FinanceRecipients` están vacíos; mientras tanto el botón "Enviar a Finanzas" falla con mensaje controlado.
4. **Umbral de Premium Freight fijo** (5,000 USD) en código (`QuoteService`, `ShippingRequest.IsPremiumFreight`).
5. **`uploads` dentro de `wwwroot`:** un publish con `DeleteExistingFiles=true` borraría los archivos y el permiso del App Pool; no hay respaldo ni limpieza de archivos huérfanos.
6. **Paquetes NuGet sin uso:** `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `System.DirectoryServices`, `System.DirectoryServices.AccountManagement` y `Microsoft.AspNetCore.Authentication.Cookies` 2.3.11 (la autenticación por AD se retiró al integrar Meax One).
7. **Páginas de plantilla** `Counter` y `Weather`, y `wwwroot/favicon.png` heredado.
8. **Catálogos solo para administradores.** Si Trade debe mantener el catálogo de clientes sin ser administrador, hay que ajustar el permiso de `/admin/customers`.
9. **Secretos en el historial de git.** Una versión anterior de `appsettings.json` (commit `d23cb05` y el commit inicial) contiene la cadena de conexión y el secreto JWT. Si el repositorio se comparte fuera del equipo, rotarlos.
10. **Integración con EBS no implementada.** Se evaluó traer el catálogo de clientes desde Oracle EBS: solo cubre parcialmente (53 de 86 clientes; RFC y moneda casi vacíos), por eso el catálogo de Trade es la fuente principal. Hay un vínculo `EBSDB` disponible en `meax_db` si se requiere después.
11. **Sin pruebas automatizadas** en el repositorio.
