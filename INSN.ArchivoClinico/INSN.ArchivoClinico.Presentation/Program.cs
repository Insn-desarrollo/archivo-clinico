using INSN.ArchivoClinico.Application.Interfaces;
using INSN.ArchivoClinico.Infrastructure.Services;
using INSN.ArchivoClinico.Controllers;
using INSN.ArchivoClinico.Domain.Interfaces;
using INSN.ArchivoClinico.Infrastructure.Data;
using INSN.ArchivoClinico.Infrastructure.Repositories;
using INSN.ArchivoClinico.Presentation.Util.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Npgsql;
using System.Data;
using INSN.ArchivoClinico.UtilFactory.Base;

var builder = WebApplication.CreateBuilder(args);

// Configuración de autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

// Habilitar el manejo de sesiones
builder.Services.AddSession();

// Registro de dependencias
builder.Services.AddScoped<IHistoriasRepository, HistoriasRepository>();
builder.Services.AddScoped<ICuentaRepository, CuentaRepository>();
builder.Services.AddScoped<IEvaluacionRepository, EvaluacionRepository>();
builder.Services.AddScoped<TokenValidationFilter>(); 
builder.Services.AddScoped<IHistoriasService, HistoriasService>();
builder.Services.AddScoped<ICuentaService, CuentaService>();
builder.Services.AddScoped<IEvaluacionService, EvaluacionService>();
builder.Services.AddScoped<IExternoService, ExternoService>();
builder.Services.AddScoped<IFuaEmitidoService, FuaEmitidoService>();

//builder.Services.AddScoped<IAtencionHceService, AtencionHceService>();

// Configuración para HTTP Client
builder.Services.AddHttpClient<AtencionController>();
builder.Services.AddHttpClient<AtencionHceService>();
builder.Services.AddHttpClient<AuthService>();
builder.Services.AddSingleton<AuthService>();

builder.Services.AddScoped<IJwtBuilder, JwtBuilder>(); // Servicio para crear el JWT

// Configurar el contexto de datos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Agregar configuración de conexión a PostgreSQL
builder.Services.AddScoped<IDbConnection>(sp =>
    new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();
app.UsePathBase("/apwArchivoClinico");
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseMiddleware<TokenMiddleware>(); 

// Mapa de rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "Index",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "triaje",
    pattern: "{controller=Atencion}/{action=Triaje}/{id?}");

app.MapControllerRoute(
    name: "Index",
    pattern: "{controller=Menu}/{action=Index}/{id?}");

app.MapControllerRoute(
     name: "Index",
     pattern: "{controller=InformacionAtenciones}/{action=Index}/{id?}"); 

app.MapControllerRoute(
    name: "ActualizarTriaje",
    pattern: "{controller=Atencion}/{action=ActualizarTriaje}");

app.MapControllerRoute(
          name: "CargarAtencionesSinCuentaTriaje",
          pattern: "{controller=Atencion}/{action=CargarAtencionesSinCuentaTriaje}");

app.MapControllerRoute(
          name: "CargarEvaluacionesOrdenes",
          pattern: "{controller=Atencion}/{action=CargarEvaluacionesOrdenes}");

app.MapControllerRoute(
    name: "ActualizarAuditoriaItem",
    pattern: "{controller=Atencion}/{action=ActualizarAuditoriaItem}");

app.Run();
