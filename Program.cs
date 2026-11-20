var builder = WebApplication.CreateBuilder(args);

// Registrar los controladores (SyncController)
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Mapear las rutas de tus controladores
app.MapControllers();

app.Run();