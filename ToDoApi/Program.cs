using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TodoApi;


var builder = WebApplication.CreateBuilder(args);

// הוספת DbContext לשירותים
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql("name=ToDoDB", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.40-mysql")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.AddControllers();

// הוסף שירותי Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

 var app = builder.Build();
//הפעל את Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // זה ייתן לך גישה ל-Swagger בכתובת הבסיס
    });
}

// הפעלת מדיניות CORS
app.UseCors("AllowAllOrigins");

app.MapGet("/tasks", async (ToDoDbContext db) => await db.Items.ToListAsync()); // שליפת כל המשימות

app.MapPost("/tasks", async (Item item, ToDoDbContext db) => 
{
    db.Items.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/tasks/{item.Id}", item); // הוספת משימה חדשה
});

app.MapPut("/tasks/{id}", async (int id, Item updatedItem, ToDoDbContext db) =>
{
    var task = await db.Items.FindAsync(id);
    if (task == null) return Results.NotFound();

    //task.Name = updatedItem.Name;
    task.IsComplete = updatedItem.IsComplete;
    await db.SaveChangesAsync();
    return Results.Ok(task);
});

app.MapDelete("/tasks/{id}", async (int id, ToDoDbContext db) =>
{
    var task = await db.Items.FindAsync(id);
    if (task == null) return Results.NotFound();

    db.Items.Remove(task);
    await db.SaveChangesAsync(); // מחיקת משימה
    return Results.NoContent();
});

app.Run();
