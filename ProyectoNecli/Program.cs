using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Necli.Logica.Interface;
using Necli.Logica.Service;
using Necli.Persistencia.DbContext;
using Necli.Persistencia.Interface;
using Necli.Persistencia.Repository;
using Necli.Persistencia.Utils;

var builder = WebApplication.CreateBuilder(args);

// Configuración de conexión a la base de datos
var conexion = builder.Configuration.GetConnectionString("local");
builder.Services.AddDbContext<NecliDbContext>(options => options.UseSqlServer(conexion));

// Inyección de dependencias
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ICuentaRepository, CuentaRepository>();
builder.Services.AddScoped<ICuentaService, CuentaService>();
builder.Services.AddScoped<ITransaccionRepository, TransaccionRepository>();
builder.Services.AddScoped<ITransaccionService, TransaccionService>();
builder.Services.AddScoped<ICorreoService, CorreoService>();
builder.Services.AddScoped<ReporteProgramadoService>();

// Configurar SmtpSettings desde appsettings.json
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));


// Configuración JWT
var secretKey = builder.Configuration["Jwt:SecretKey"] ?? "CLAVE_SUPER_SECRETA_DE_32+CHARS";
var issuer = builder.Configuration["Jwt:Issuer"] ?? "NecliAPI";
var audience = builder.Configuration["Jwt:Audience"] ?? "NecliCliente";

builder.Services.AddScoped<IJwtService>(sp =>
    new JwtService(secretKey, issuer, audience));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

// Configuración CORS para Swagger y pruebas locales
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirSwagger", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Controladores y prevención de ciclos
builder.Services.AddHttpClient();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

// Swagger + botón "Authorize"
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingrese: Bearer [espacio] + su token JWT"
    });

    opt.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("PermitirSwagger"); // Activar política CORS

app.UseAuthentication(); // DEBE IR ANTES DE Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();
