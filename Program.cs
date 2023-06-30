using EntityFrameworkCourse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// add database in memory
//builder.Services.AddDbContext<TareasContext>(p => p.UseInMemoryDatabase("TareasDB"));

// connect to SQl Server 

// to connect with a User from SQL Server
//builder.Services.AddSqlServer<TareasContext>("Data Source=NameComputer//SQLEXPRESS;Initial Catalog=TareasDB;user id=sa;password=dominic");

// to connect with Windows Authentication
builder.Services.AddSqlServer<TareasContext>(builder.Configuration.GetConnectionString("cnTareas"));


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// add database conexion  in memory and sql server
app.MapGet("/dbconexion", async ([FromServices] TareasContext dbContext) => 
{
    dbContext.Database.EnsureCreated();
    return Results.Ok("Base de datos en memoria: " + dbContext.Database.IsInMemory());
});

app.Run();
