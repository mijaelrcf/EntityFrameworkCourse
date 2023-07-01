using EntityFrameworkCourse;
using EntityFrameworkCourse.Models;
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

app.MapGet("/api/tareas", async ([FromServices] TareasContext dbContext) => 
{
    // Get all 
    return Results.Ok(dbContext.Tareas);

    // Get Tareas with Prioridad = Baja
    //return Results.Ok(dbContext.Tareas.Where(p => p.PrioridadTarea == EntityFrameworkCourse.Models.Prioridad.Baja));

    // Get Tareas included Categorias
    //return Results.Ok(dbContext.Tareas.Include(p => p.Categoria).Where(p => p.PrioridadTarea == EntityFrameworkCourse.Models.Prioridad.Baja));
});

app.MapPost("/api/tareas", async ([FromServices] TareasContext dbContext, [FromBody] Tarea tarea) => 
{
    tarea.TareaId = Guid.NewGuid();
    tarea.FechaCreacion = DateTime.Now;
    await dbContext.AddAsync(tarea); // first way
    //await dbContext.Tareas.AddAsync(tarea); // second way

    await dbContext.SaveChangesAsync();

    return Results.Ok();
});

app.MapPut("/api/tareas/{id}", async ([FromServices] TareasContext dbContext, [FromBody] Tarea tarea, [FromRoute] Guid id) => 
{
    var tareaActual = dbContext.Tareas.Find(id); // basically find by Id of the entity

    if (tareaActual != null)
    {
        tareaActual.CategoriaId = tarea.CategoriaId;
        tareaActual.Titulo = tarea.Titulo;
        tareaActual.PrioridadTarea = tarea.PrioridadTarea;
        tareaActual.Descripcion = tarea.Descripcion;

        await dbContext.SaveChangesAsync();
        return Results.Ok();
    }

    return Results.NotFound();
});

app.MapDelete("/api/tareas/{id}", async ([FromServices] TareasContext dbContext, [FromRoute] Guid id) => 
{
    var tareaActual = dbContext.Tareas.Find(id); // basically find by Id of the entity

    if (tareaActual != null)
    {
        dbContext.Remove(tareaActual);

        await dbContext.SaveChangesAsync();
        
        return Results.Ok();
    }
    
    return Results.NotFound();
});

app.Run();
