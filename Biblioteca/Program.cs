using Biblioteca;
using Biblioteca.Servicios;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

#region Configuraci�n de la base de datos
// Configuraci�n de la cadena de conexi�n
var connectionString = builder.Configuration.GetConnectionString("Biblioteca");

// Configuraci�n del contexto de la base de datos
builder.Services.AddDbContext<ContextoBiblioteca>(options =>
    options.UseSqlServer(connectionString));
#endregion

#region Configuraci�n de los servicios de negocio
builder.Services.AddTransient(typeof(ServicioBase<>), typeof(ServicioBase<>));
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

//app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
