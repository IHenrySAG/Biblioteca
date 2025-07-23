using Biblioteca;
using Biblioteca.Common;
using Biblioteca.Servicios;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<NoAuthorizationFilter>();
builder.Services.AddScoped<AuthorizationFilter>();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<NoAuthorizationFilter>();
    options.Filters.AddService<AuthorizationFilter>();
}).AddJsonOptions(x =>x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); 

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "SGB-Unapec.Session"; // Tiempo de expiración de la sesión
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración de la sesión
    options.Cookie.HttpOnly = true; // La cookie de sesión no es accesible desde JavaScript
    options.Cookie.IsEssential = true; // La cookie es esencial para el funcionamiento del sitio
});

#region Configuración de la base de datos
// Configuración de la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("Biblioteca");

// Configuración del contexto de la base de datos
builder.Services.AddDbContext<ContextoBiblioteca>(options =>
    options.UseSqlServer(connectionString));
#endregion

#region Configuración de los servicios de negocio
builder.Services.AddTransient(typeof(ServicioBase<>), typeof(ServicioBase<>));
builder.Services.AddTransient<AutorServicio>();
builder.Services.AddTransient<EditoraServicio>();
builder.Services.AddTransient<EmpleadoServicio>();
builder.Services.AddTransient<IdiomaServicio>();
builder.Services.AddTransient<LibroServicio>();
builder.Services.AddTransient<PrestamoServicio>();
builder.Services.AddTransient<TandaLaborServicio>();
builder.Services.AddTransient<TipoBibliografiaServicio>();
builder.Services.AddTransient<EstudiantesServicio>();
#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

//app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

using(var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await SeedDatabase.SeedAdmin(serviceProvider);
}


app.Run();
