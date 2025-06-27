using Catalog.Application.UseCases.CreateGameUseCase;
using Catalog.Application.UseCases.DeleteGameUseCase;
using Catalog.Application.UseCases.GetAllGamesUseCase;
using Catalog.Application.UseCases.GetGameByIdUseCase;
using Catalog.Application.UseCases.LoginUseCase;
using Catalog.Application.UseCases.UpdateGameUseCase;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.DependencyInjection;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "Catalog.API",
            ValidAudience = "Catalog.Client",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("sua-chave-secreta-supersegura"))
        };
    });

builder.Services
       .AddOpenApi()
       .AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<GetGameByIdUseCase>();
builder.Services.AddScoped<CreateGameUseCase>();
builder.Services.AddScoped<GetAllGamesUseCase>();
builder.Services.AddScoped<DeleteGameUseCase>();
builder.Services.AddScoped<UpdateGameUseCase>();
builder.Services.AddScoped<LoginUseCase>();

builder.Services.AddScoped<IValidator<CreateGameInput>, CreateGameValidator>();
builder.Services.AddScoped<IValidator<UpdateGameInput>, UpdateGameValidator>();

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

    const int maxRetries = 10;
    var retries = 0;

    while (true)
    {
        try
        {
            await db.Database.MigrateAsync();
            Console.WriteLine("Migrations aplicadas com sucesso.");
            break;
        }
        catch (Exception ex) when (retries < maxRetries)
        {
            retries++;
            Console.WriteLine($"Tentativa {retries}: aguardando banco... {ex.Message}");
            Thread.Sleep(2000); // espera 2 segundos
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login", [AllowAnonymous] (LoginRequest request, LoginUseCase useCase) =>
{
    var token = useCase.Execute(request.Username, request.Password);

    return token is null
        ? Results.Unauthorized()
        : Results.Ok(new { token });
});

app.MapGet("/game/{id}", async (int id, GetGameByIdUseCase useCase) =>
{
    try
    {
        var result = await useCase.ExecuteAsync(id);
        return result.Data is not null ? Results.Ok(result.Data) : Results.NotFound($"Game with ID {id} not found.");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
});

app.MapGet("/games", [Authorize] async (GetAllGamesUseCase useCase) =>
{
    try
    {
        var result = await useCase.ExecuteAsync();
        return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Errors);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
});

app.MapPost("/game", async (CreateGameInput game, CreateGameUseCase useCase) =>
{
    try
    {
        var result = await useCase.ExecuteAsync(game);
        return result.IsSuccess ? Results.Created($"/games/{result.Data!.Id}", result.Data) : Results.BadRequest(result.Errors);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
});

app.MapPut("/game/{id}", async (int id, UpdateGameInput game, UpdateGameUseCase useCase) =>
{
    try
    {
        var result = await useCase.ExecuteAsync(id, game);
        return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Errors);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
});

app.MapDelete("/game/{id}", async (int id, DeleteGameUseCase useCase) =>
{
    try
    {
        var result = await useCase.ExecuteAsync(id);
        return result.IsSuccess ? Results.Ok() : Results.NotFound(result.Errors);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
});

await app.RunAsync();