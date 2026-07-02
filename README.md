# Solución CRM - Backend & Frontend (.NET 10)

Esta solución implementa la estructura base de un sistema CRM utilizando una **Arquitectura Hexagonal (Clean Architecture)** con desacoplamiento total de sus capas y seguridad integrada en base de datos.

---

## 🛠️ Tecnologías Utilizadas

- **Núcleo:** .NET 10 (ASP.NET Core Web API + Blazor Server)
- **Persistencia:** PostgreSQL, Dapper (Micro-ORM de alta velocidad), Npgsql
- **Criptografía:** AES-256 (Cifrado simétrico de credenciales) y BCrypt (Hasing seguro de contraseñas de usuarios)
- **Autenticación:** JWT (JSON Web Tokens) con expiración configurable

---

## 🏗️ Estructura del Proyecto (Arquitectura Hexagonal)

La capa `CRM.ApiHub` está estructurada de la siguiente manera:
- **Domain:** Entidades puras y puertos (interfaces) de repositorios. Totalmente agnóstica de frameworks.
- **Application:** Casos de uso (ej. `LoginUseCase`), contratos de servicios y DTOs de comunicación.
- **Infrastructure:** Adaptadores concretos (acceso a DB con Dapper, generación de tokens JWT, e inyección de dependencias).
- **Api:** Controladores y middlewares HTTP de ASP.NET Core.

---

## 🔒 Conexión Segura (Cifrado de Credenciales)

Las credenciales del PostgreSQL remoto están protegidas. En el archivo `appsettings.json`, la cadena de conexión se guarda cifrada con el prefijo `Encrypted:` mediante AES-256:
```json
"ConnectionStrings": {
  "DefaultConnection": "Encrypted:14P+tcDhmupBRJM49Gfcc+a8ukIUUKgtc7cvTlmaSb9H2Bd//lyQFXfTJgYyuFPhO/BxYfrEEOrIBwkeosgudUPRs6TY1FLOZIXmIPwyO36qx/GoY1A4ZFu3X05lsXBw"
}
```
El `DbConnectionFactory` detecta este prefijo y descifra la cadena dinámicamente en memoria en tiempo de ejecución, protegiendo las contraseñas en texto plano.

---

## 🚀 Cómo Inicializar la Solución

### 1. Prerrequisitos
- Tener instalado el SDK de .NET 10.
- Acceso a internet (el backend se conectará automáticamente a la base de datos PostgreSQL remota y segura configurada).

### 2. Compilar la Solución
Desde la raíz de la carpeta del proyecto, ejecuta:
```powershell
dotnet build CRM.sln
```

### 3. Ejecutar el Backend (API)
Navega o apunta al proyecto del API y levántalo:
```powershell
dotnet run --project CRM.ApiHub/CRM.ApiHub.csproj --launch-profile "http"
```
- La API estará escuchando en: `http://localhost:5068`
- La documentación interactiva de **Swagger** estará disponible en: [http://localhost:5068/swagger](http://localhost:5068/swagger)

### 4. Ejecutar el Frontend (Blazor)
En otra terminal, levanta el servidor web del Frontend:
```powershell
dotnet run --project CRM.WebFrontend/CRM.WebFrontend.csproj
```
- El frontend estará disponible en la URL indicada en la consola (por defecto `http://localhost:5000` o similar).

---

## 🧪 Pruebas de Autenticación (Swagger)

Para comprobar de forma rápida el funcionamiento del endpoint de Login con JWT sin necesidad de conocer o comprometer las contraseñas reales de los usuarios pre-existentes en la base de datos:

1. Ve a [http://localhost:5068/swagger](http://localhost:5068/swagger).
2. Abre el endpoint **`POST /api/auth/login`**.
3. Envía el siguiente JSON de prueba:
   ```json
   {
     "username": "testuser",
     "password": "Password123!"
   }
   ```
4. El servidor responderá con `200 OK` y entregará el token JWT firmado de forma exitosa.
