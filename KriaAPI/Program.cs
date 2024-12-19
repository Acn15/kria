using KriaAPI.Data; // Namespace do AppDbContext
using KriaAPI.Services; // Namespaces para UsuarioService e RepositorioService
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar o DbContext com banco InMemory
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("KriaDB"));

// Registrar os serviços no contêiner de dependências
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<RepositorioService>(); // Novo serviço para repositórios

// Adicionar serviços ao container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(); // Habilitar suporte para controllers

var app = builder.Build();

// Configurar o pipeline de requisição HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Mapear controllers automaticamente
app.MapControllers();
app.MapGet("/", () => "API is running! Use /api/usuarios or /api/repositorios."); // Mensagem inicial

app.Run();
