using Catalog.Application.UseCases.CreateGameUseCase;
using Catalog.Application.UseCases.DeleteGameUseCase;
using Catalog.Application.UseCases.GetAllGamesUseCase;
using Catalog.Application.UseCases.GetGameByIdUseCase;
using Catalog.Application.UseCases.UpdateGameUseCase;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.DependencyInjection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<GetGameByIdUseCase>();
builder.Services.AddScoped<CreateGameUseCase>();
builder.Services.AddScoped<GetAllGamesUseCase>();
builder.Services.AddScoped<DeleteGameUseCase>();
builder.Services.AddScoped<UpdateGameUseCase>();

builder.Services.AddScoped<IValidator<CreateGameInput>, CreateGameValidator>();
builder.Services.AddScoped<IValidator<UpdateGameInput>, UpdateGameValidator>();

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

var app = builder.Build();

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