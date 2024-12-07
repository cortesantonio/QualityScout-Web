using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Localization;
using ScannerCC.Models;
using System.Globalization;
using QualityScout.MobileEndpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Auth/LoginIn";
        options.LogoutPath = "/Auth/LogOut";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

// Agrega CORS al contenedor de servicios: ayuda a la conexion por api desde la version mobile.
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAllOrigins",
		builder =>
		{
			builder.AllowAnyOrigin()
				   .AllowAnyMethod()
				   .AllowAnyHeader();
		});
});

builder.Services.AddSession();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization(); //Middleware


// app.UseHttpsRedirection(); Se desactiva redireccion a HTTPS para evitar errores de certificados ssl en la app movil durante desarrollo.
app.UseSession();
app.UseCors("AllowAllOrigins");


// Rutas personalizadas para "Home"
app.MapControllerRoute(
    name: "Inicio",
    pattern: "Inicio",
    defaults: new { controller = "Home", action = "Index" }
);

app.MapControllerRoute(
    name: "Acceso",
    pattern: "Acceso",
    defaults: new { controller = "Home", action = "Index1" }
);

app.MapControllerRoute(
    name: "Login",
    pattern: "Login",
    defaults: new { controller = "Home", action = "Index2" }
);

// Rutas personalizadas para "Controlcalidad"
app.MapControllerRoute(
    name: "Control-de-calidad",
    pattern: "Control-de-calidad",
    defaults: new { controller = "Controlcalidad", action = "Index" }
);

// Rutas personalizadas para "Especialista"
app.MapControllerRoute(
    name: "Especialista",
    pattern: "Especialista",
    defaults: new { controller = "Especialista", action = "Index" }
);

app.MapControllerRoute(
    name: "Dashboard",
    pattern: "Dashboard",
    defaults: new { controller = "Especialista", action = "Dashboard" }
);

// Rutas personalizadas para "BotellaDetalles"
app.MapControllerRoute(
    name: "Gestion-botellas",
    pattern: "Gestion-botellas",
    defaults: new { controller = "BotellaDetalles", action = "GestionBotellas" }
);

app.MapControllerRoute(
    name: "Registrar-botella",
    pattern: "Registrar-botella",
    defaults: new { controller = "BotellaDetalles", action = "Create" }
);

app.MapControllerRoute(
    name: "Editar-botella",
    pattern: "Editar-botella/{id?}",
    defaults: new { controller = "BotellaDetalles", action = "Edit" }
);

app.MapControllerRoute(
    name: "Ver-botella",
    pattern: "Ver-botella/{id?}",
    defaults: new { controller = "BotellaDetalles", action = "Details" }
);

app.MapControllerRoute(
    name: "Eliminar-botella",
    pattern: "Eliminar-botella/{id?}",
    defaults: new { controller = "BotellaDetalles", action = "Delete" }
);

// Rutas personalizadas para "Controles"
app.MapControllerRoute(
    name: "Gestion-controles",
    pattern: "Gestion-controles",
    defaults: new { controller = "Controles", action = "GestionControles" }
);

app.MapControllerRoute(
    name: "Controles",
    pattern: "Controles",
    defaults: new { controller = "Controles", action = "Controles" }
);

app.MapControllerRoute(
    name: "Registrar-control",
    pattern: "Registrar-control/{idProducto?}",
    defaults: new { controller = "Controles", action = "CreateControl" }
);

app.MapControllerRoute(
    name: "Editar-control",
    pattern: "Editar-control/{id?}",
    defaults: new { controller = "Controles", action = "Edit2" }
);

app.MapControllerRoute(
    name: "Ver-control",
    pattern: "Ver-control/{id?}",
    defaults: new { controller = "Controles", action = "Details2" }
);

app.MapControllerRoute(
    name: "Eliminar-control",
    pattern: "Eliminar-control/{id?}",
    defaults: new { controller = "Controles", action = "Delete2" }
);

app.MapControllerRoute(
    name: "Reconocimiento-con-camara",
    pattern: "Reconocimiento-con-camara/{id?}",
    defaults: new { controller = "Controles", action = "Reconocimiento" }
);

// Rutas personalizadas para "Escaneos"
app.MapControllerRoute(
    name: "Gestion-escaneos",
    pattern: "Gestion-escaneos",
    defaults: new { controller = "Escaneos", action = "GestionEscaneos" }
);

app.MapControllerRoute(
    name: "Escaner",
    pattern: "Escaner",
    defaults: new { controller = "Escaneos", action = "Scanner" }
);

// Rutas personalizadas para "InformacionQuimica"
app.MapControllerRoute(
    name: "Gestion-informaciones-quimicas",
    pattern: "Gestion-informaciones-quimicas",
    defaults: new { controller = "InformacionQuimica", action = "GestionInformacion" }
);

app.MapControllerRoute(
    name: "Registrar-informacion-quimica",
    pattern: "Registrar-informacion-quimica",
    defaults: new { controller = "InformacionQuimica", action = "Create" }
);

app.MapControllerRoute(
    name: "Editar-informacion-quimica",
    pattern: "Editar-informacion-quimica/{id?}",
    defaults: new { controller = "InformacionQuimica", action = "Edit" }
);

app.MapControllerRoute(
    name: "Ver-informacion-quimica",
    pattern: "Ver-informacion-quimica/{id?}",
    defaults: new { controller = "InformacionQuimica", action = "Details" }
);

app.MapControllerRoute(
    name: "Eliminar-informacion-quimica",
    pattern: "Eliminar-informacion-quimica/{id?}",
    defaults: new { controller = "InformacionQuimica", action = "Delete" }
);

// Rutas personalizadas para "Informes"
app.MapControllerRoute(
    name: "Gestion-informes",
    pattern: "Gestion-informes",
    defaults: new { controller = "Informes", action = "GestionInformes" }
);

app.MapControllerRoute(
    name: "Registrar-informe",
    pattern: "Registrar-informe",
    defaults: new { controller = "Informes", action = "Create" }
);

app.MapControllerRoute(
    name: "Editar-informe",
    pattern: "Editar-informe/{id?}",
    defaults: new { controller = "Informes", action = "Edit" }
);

app.MapControllerRoute(
    name: "Ver-informe",
    pattern: "Ver-informe/{id?}",
    defaults: new { controller = "Informes", action = "Details" }
);

app.MapControllerRoute(
    name: "Eliminar-informe",
    pattern: "Eliminar-informe/{id?}",
    defaults: new { controller = "Informes", action = "Delete" }
);

app.MapControllerRoute(
    name: "Previsualizar-informe",
    pattern: "Previsualizar-informe/{id?}",
    defaults: new { controller = "Informes", action = "Previsualizar" }
);

// Rutas personalizadas para "Producto"
app.MapControllerRoute(
    name: "Gestion-productos",
    pattern: "Gestion-productos",
    defaults: new { controller = "Producto", action = "GestionProductos" }
);

app.MapControllerRoute(
    name: "Registrar-producto",
    pattern: "Registrar-producto",
    defaults: new { controller = "Producto", action = "Create" }
);

app.MapControllerRoute(
    name: "Editar-producto",
    pattern: "Editar-producto/{id?}",
    defaults: new { controller = "Producto", action = "Edit" }
);

app.MapControllerRoute(
    name: "Ver-producto",
    pattern: "Ver-producto/{id?}",
    defaults: new { controller = "Producto", action = "Details" }
);

// Rutas personalizadas para "ProductoDetalles"
app.MapControllerRoute(
    name: "Gestion-detalles-productos",
    pattern: "Gestion-detalles-productos",
    defaults: new { controller = "ProductoDetalles", action = "GestionProductosD" }
);

app.MapControllerRoute(
    name: "Registrar-detalle-producto",
    pattern: "Registrar-detalle-producto",
    defaults: new { controller = "ProductoDetalles", action = "Create" }
);

app.MapControllerRoute(
    name: "Editar-detalle-producto",
    pattern: "Editar-detalle-producto/{id?}",
    defaults: new { controller = "ProductoDetalles", action = "Edit" }
);

app.MapControllerRoute(
    name: "Ver-detalle-producto",
    pattern: "Ver-detalle-producto/{id?}",
    defaults: new { controller = "ProductoDetalles", action = "Details" }
);

app.MapControllerRoute(
    name: "Eliminar-detalle-producto",
    pattern: "Eliminar-detalle-producto/{id?}",
    defaults: new { controller = "ProductoDetalles", action = "Delete" }
);

// Rutas personalizadas para "ProductoHistorial"
app.MapControllerRoute(
    name: "Gestion-historiales-productos",
    pattern: "Gestion-historiales-productos",
    defaults: new { controller = "ProductoHistorial", action = "GestionProductosH" }
);

app.MapControllerRoute(
    name: "Registrar-historial-producto",
    pattern: "Registrar-historial-producto",
    defaults: new { controller = "ProductoHistorial", action = "Create" }
);

app.MapControllerRoute(
    name: "Editar-historial-producto",
    pattern: "Editar-historial-producto/{id?}",
    defaults: new { controller = "ProductoHistorial", action = "Edit" }
);

app.MapControllerRoute(
    name: "Ver-historial-producto",
    pattern: "Ver-historial-producto/{id?}",
    defaults: new { controller = "ProductoHistorial", action = "Details" }
);

app.MapControllerRoute(
    name: "Eliminar-historial-producto",
    pattern: "Eliminar-historial-producto/{id?}",
    defaults: new { controller = "ProductoHistorial", action = "Delete" }
);

// Rutas personalizadas para "Usuario"

app.MapControllerRoute(
    name: "Gestion-usuarios",
    pattern: "Gestion-usuarios",
    defaults: new { controller = "Usuario", action = "GestionUsuarios" }
);

app.MapControllerRoute(
    name: "Registrar-usuario",
    pattern: "Registrar-usuario",
    defaults: new { controller = "Usuario", action = "Create" }
);

app.MapControllerRoute(
    name: "Editar-usuario",
    pattern: "Editar-usuario/{id?}",
    defaults: new { controller = "Usuario", action = "Edit" }
);

app.MapControllerRoute(
    name: "Ver-usuario",
    pattern: "Ver-usuario/{id?}",
    defaults: new { controller = "Usuario", action = "Details" }
);

app.MapControllerRoute(
    name: "Editar-contrasena",
    pattern: "Editar-contrasena/{id?}",
    defaults: new { controller = "Usuario", action = "EditC" }
);

// Ruta predeterminada 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);



app.Run();
