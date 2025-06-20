using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PlayStack_game_catalog_service.Catalog.Application.DTOs;
using PlayStack_game_catalog_service.Catalog.Application.UseCases;
using PlayStack_game_catalog_service.Catalog.Application.Validators;
using PlayStack_game_catalog_service.Catalog.Domain.Interfaces;
using PlayStack_game_catalog_service.Catalog.Infrastructure;
using PlayStack_game_catalog_service.Catalog.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Injeção de dependências
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<GetGameByIdUseCase>();
builder.Services.AddScoped<CreateGameUseCase>();
builder.Services.AddScoped<GetAllGamesUseCase>();
builder.Services.AddScoped<DeleteGameUseCase>();
builder.Services.AddScoped<UpdateGameUseCase>();

// Registro do validador de GameDto com FluentValidation
builder.Services.AddScoped<IValidator<GameDto>, GameValidator>();

Console.WriteLine("ConnectionString carregada: " + builder.Configuration.GetConnectionString("PostgreSQL"));

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

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

app.MapGet("/games", async (GetAllGamesUseCase useCase) =>
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

app.MapPost("/game", async (GameDto game, CreateGameUseCase useCase) =>
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

app.MapPut("/game/{id}", async (int id, GameDto game, UpdateGameUseCase useCase) =>
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