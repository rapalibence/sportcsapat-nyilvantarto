using SportcsapatApi.Data;
using SportcsapatApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<SportStore>();

// A weboldal (frontend) külön porton fut majd, ezért kell a CORS.
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Swagger UI a http://localhost:5152/swagger címen, a fenti OpenAPI leírásból.
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "Sportcsapat nyilvántartó API v1"));
}

app.UseHttpsRedirection();
app.UseCors();

app.MapGet("/", () => Results.Ok(new { name = "Sportcsapat nyilvántartó API", version = "0.1" }))
    .WithName("ApiInfo");

app.MapTeamEndpoints();
app.MapPlayerEndpoints();
app.MapMatchEndpoints();
app.MapTrainingEndpoints();

app.Run();
