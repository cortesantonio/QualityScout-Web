# QualityScout Web v2.0

## 📋 Descripción

QualityScout es una aplicación web integral para el control de calidad de productos vinícolas, desarrollada con ASP.NET Core 6.0. El sistema permite gestionar productos, realizar controles de calidad, generar informes y proporciona **endpoints API especializados para la versión móvil** de la aplicación.

## 🏗️ Arquitectura del Sistema

El proyecto está estructurado siguiendo el patrón MVC (Model-View-Controller) con una capa adicional de **API endpoints móviles** que facilitan la integración con aplicaciones móviles.

### Estructura Principal
```
ScannerCC/
├── Controllers/          # Controladores web MVC
├── MobileEndpoints/      # 📱 API endpoints para versión móvil
├── Models/              # Modelos de datos y contexto de BD
├── Views/               # Vistas Razor para interfaz web
├── wwwroot/             # Archivos estáticos (CSS, JS, imágenes)
└── Migrations/          # Migraciones de Entity Framework
```

## 📱 Endpoints para Versión Móvil

El sistema incluye una **API REST completa diseñada específicamente para la versión móvil**, ubicada en la carpeta `MobileEndpoints/`:

### 🔐 Autenticación (`AuthApi.cs`)
- **POST** `/api/AuthApi/login` - Autenticación de usuarios móviles
- **POST** `/api/AuthApi/CreateUsuario` - Registro de nuevos usuarios
- **PUT** `/api/AuthApi/UpdatePassword/{Rut}` - Actualización de contraseñas

### 📦 Gestión de Productos (`ProductosApi.cs`)
- **GET** `/api/ProductosApi` - Listar todos los productos
- **GET** `/api/ProductosApi/{id}` - Obtener producto específico
- **GET** `/api/ProductosApi/GetProductoToEdit/{id}` - Datos para edición móvil
- **POST** `/api/ProductosApi/EditarProducto/{id}` - Editar producto desde móvil
- **POST** `/api/ProductosApi/CrearProducto` - Crear nuevo producto
- **POST** `/api/ProductosApi/EstaRegistrado` - Verificar registro por código
- **PUT** `/api/ProductosApi/UpdateActivo/{id}` - Activar/desactivar producto

### 📊 Dashboard y Controles (`ApiDashboard.cs`, `ApiControles.cs`)
- **GET** `/api/ApiDashboard` - Datos del dashboard móvil
- **GET** `/api/ApiControles` - Controles de calidad para móvil

### 📋 Informes (`InformesApi.cs`)
- **GET** `/api/InformesApi` - Informes disponibles para móvil

### 👥 Usuarios (`UsuariosApi.cs`)
- **GET** `/api/UsuariosApi` - Gestión de usuarios desde móvil

### 🔒 Encriptación (`EncryptionController.cs`)
- Servicios de encriptación para comunicación segura móvil-servidor

### 🎭 Roles (`RolsController.cs`)
- **GET** `/api/RolsController` - Gestión de roles para aplicación móvil

## 🛠️ Tecnologías Utilizadas

- **Framework**: ASP.NET Core 6.0
- **Base de Datos**: Entity Framework Core con soporte para:
  - SQL Server
  - SQLite
  - MySQL
- **Autenticación**: Cookie Authentication
- **API Documentation**: Swagger/OpenAPI
- **CORS**: Configurado para permitir conexiones desde aplicaciones móviles
- **Encriptación**: AES para comunicación segura con móviles

## 🚀 Características Principales

### Para Aplicación Web
- ✅ Gestión completa de productos vinícolas
- ✅ Control de calidad con reconocimiento por cámara
- ✅ Sistema de escaneo de códigos de barras
- ✅ Generación de informes detallados
- ✅ Gestión de usuarios y roles
- ✅ Dashboard con métricas en tiempo real

### Para Aplicación Móvil
- 📱 **API REST completa** para todas las funcionalidades
- 🔐 **Autenticación segura** con tokens
- 🔒 **Comunicación encriptada** AES
- 📊 **Sincronización de datos** en tiempo real
- 📷 **Soporte para escaneo móvil** de códigos
- 🌐 **CORS configurado** para conexiones cross-origin

## ⚙️ Configuración

### Requisitos Previos
- .NET 6.0 SDK
- SQL Server / SQLite / MySQL
- Visual Studio 2022 o VS Code

### Instalación

1. **Clonar el repositorio**
```bash
git clone [URL_DEL_REPOSITORIO]
cd QualityScout-Web-main
```

2. **Restaurar paquetes NuGet**
```bash
dotnet restore
```

3. **Configurar cadena de conexión**
Editar `appsettings.json` con tu cadena de conexión de base de datos.

4. **Ejecutar migraciones**
```bash
dotnet ef database update
```

5. **Ejecutar la aplicación**
```bash
dotnet run
```

### Configuración para Móviles

La aplicación está preconfigurada para recibir conexiones de aplicaciones móviles:

- **CORS**: Habilitado para todos los orígenes
- **HTTPS**: Deshabilitado en desarrollo para evitar problemas de certificados SSL en móviles
- **Encriptación**: Clave AES configurada para comunicación segura

## 📡 Uso de la API Móvil

### Autenticación
```http
POST /api/AuthApi/login
Content-Type: application/json

{
  "Rut": "[RUT_ENCRIPTADO]",
  "Password": "[PASSWORD_ENCRIPTADO]"
}
```

### Crear Producto desde Móvil
```http
POST /api/ProductosApi/CrearProducto
Authorization: Bearer [TOKEN]
Content-Type: application/json

{
  "CodigoBarra": "123456789",
  "NombreVino": "Vino Ejemplo",
  "PaisDestino": "Chile",
  // ... otros campos
}
```

### Verificar Producto Registrado
```http
POST /api/ProductosApi/EstaRegistrado
Authorization: Bearer [TOKEN]
Content-Type: application/json

"123456789"
```

## 🔧 Funcionalidades Específicas para Móviles

### Escaneo de Códigos
La API móvil registra automáticamente cada escaneo realizado desde dispositivos móviles, incluyendo:
- Usuario que realizó el escaneo
- Fecha y hora del escaneo
- Producto escaneado

### Sincronización de Datos
Los endpoints móviles están optimizados para:
- Transferencia eficiente de datos
- Manejo de conexiones intermitentes
- Validación de tokens en cada petición

### Seguridad Móvil
- Encriptación AES de datos sensibles
- Tokens de autenticación únicos por usuario
- Validación de autorización en endpoints críticos

## 📚 Modelos de Datos

- **Productos**: Información completa de vinos
- **ProductoDetalles**: Detalles específicos (capacidad, tipo de cápsula, etc.)
- **ProductoHistorial**: Fechas de cosecha, producción y envasado
- **BotellaDetalles**: Especificaciones de botellas
- **InformacionQuimica**: Datos químicos del producto
- **Controles**: Registros de control de calidad
- **Escaneos**: Historial de escaneos realizados
- **Usuarios**: Gestión de usuarios del sistema
- **Informes**: Reportes generados


---

**Nota Importante**: Los endpoints de la API móvil están diseñados específicamente para la aplicación móvil de QualityScout y requieren autenticación mediante tokens Bearer para su uso.