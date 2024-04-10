using NSwag.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<KeyvalueDb>(opt => opt.UseInMemoryDatabase("KeyvalueList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "KeyvalueAPI";
    config.Title = "KeyvalueAPI v1";
    config.Version = "v1";
});
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "KeyvalueAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

app.MapGet("/keyvalueitems", async (KeyvalueDb db) =>
    await db.Keyvalues.ToListAsync());

app.MapGet("/keyvalueitems/complete", async (KeyvalueDb db) =>
    await db.Keyvalues.Where(t => t.IsComplete).ToListAsync());

app.MapGet("/keyvalueitems/{id}", async (int id, KeyvalueDb db) =>
    await db.Keyvalues.FindAsync(id)
        is Keyvalue keyvalue
            ? Results.Ok(keyvalue)
            : Results.NotFound());

app.MapPost("/keyvalueitems", async (Keyvalue keyvalue, KeyvalueDb db) =>
{
    db.Keyvalues.Add(keyvalue);
    await db.SaveChangesAsync();

    return Results.Created($"/keyvalueitems/{keyvalue.Id}", keyvalue);
});

app.MapPut("/keyvalueitems/{id}", async (int id, Keyvalue inputKeyvalue, KeyvalueDb db) =>
{
    var keyvalue = await db.Keyvalues.FindAsync(id);

    if (keyvalue is null) return Results.NotFound();

    keyvalue.Name = inputKeyvalue.Name;
    keyvalue.IsComplete = inputKeyvalue.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/keyvalueitems/{id}", async (int id, KeyvalueDb db) =>
{
    if (await db.Keyvalues.FindAsync(id) is Keyvalue keyvalue)
    {
        db.Keyvalues.Remove(keyvalue);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();

// var builder = WebApplication.CreateBuilder(args);
// var app = builder.Build();

// app.MapGet("/", () => "Hello World!");

// app.Run();
