using FluentValidation;
using PlayStack_game_catalog_service.Catalog.Application.DTOs;
using PlayStack_game_catalog_service.Catalog.Application.UseCases;
using PlayStack_game_catalog_service.Catalog.Application.Validators;
using PlayStack_game_catalog_service.Catalog.Domain.Entities;
using PlayStack_game_catalog_service.Catalog.Domain.Interfaces;
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


// Registro do validador de GameDto com FluentValidation
builder.Services.AddScoped<IValidator<GameDto>, GameValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/game/{id}", async (int id, GetGameByIdUseCase useCase) =>
{
    var result = await useCase.ExecuteAsync(id);
    return result.Data is not null ? Results.Ok(result.Data) : Results.NotFound($"Game with ID {id} not found.");
});

app.MapGet("/games", async (GetAllGamesUseCase useCase) =>
{
    var result = await useCase.ExecuteAsync();
    return result.Data is null || result.Data.Any() ? Results.Ok(result.Data) : Results.NotFound("No games found.");
});

app.MapPost("/game", async (GameDto game, CreateGameUseCase useCase) =>
{
    var result = await useCase.ExecuteAsync(game);
    return result.IsSuccess ? Results.Created($"/games/{result.Data!.Id}", result.Data) : Results.BadRequest(result.Errors);
});

app.MapPut("/game/{id}", async (int id, Game game, IGameRepository repository) =>
{
    game.Id = id; // Ensure the ID is set correctly
    await repository.UpdateAsync(game);
    return Results.NoContent();
});

app.MapDelete("/game/{id}", async (int id, DeleteGameUseCase useCase) =>
{
    var result = await useCase.ExecuteAsync(id);
    return result.IsSuccess ? Results.NoContent() : Results.NotFound($"Game with ID {id} not found.");
});

await app.RunAsync();