using Microsoft.EntityFrameworkCore;
using PracticaWeb_GAAJ.Modelos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add services to the container.

builder.Services.AddControllers();

//Inyección por depencencia del string de conexion al contexto
builder.Services.AddDbContext<BibliotecaContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BibliotecaDbConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
