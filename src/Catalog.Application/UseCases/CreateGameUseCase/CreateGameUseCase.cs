using Catalog.Application.Common;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCases.CreateGameUseCase
{
    public class CreateGameUseCase
    {
        private readonly IGameRepository _gameRepository;
        private readonly IValidator<CreateGameInput> _validator;
        private readonly ILogger<CreateGameUseCase> _logger;

        public CreateGameUseCase(IGameRepository gameRepository, IValidator<CreateGameInput> validator, ILogger<CreateGameUseCase> logger)
        {
            _gameRepository = gameRepository;
            _validator = validator;
            _logger = logger;
        }
        public async Task<Result<Game>> ExecuteAsync(CreateGameInput createGameInput)
        {
            try
            {
                var result = await _validator.ValidateAsync(createGameInput);

                if (!result.IsValid)
                {
                    _logger.LogInformation("Validation failed for createGameInput: {Errors}", result.Errors.Select(e => e.ErrorMessage));
                    return Result<Game>.Failure(result.Errors.Select(e => e.ErrorMessage).ToList());
                }

                var newGame = new Game
                {
                    Name = createGameInput.Name,
                    Description = createGameInput.Description,
                    Genre = createGameInput.Genre,
                    ReleaseDate = DateTime.SpecifyKind(createGameInput.ReleaseDate, DateTimeKind.Utc),
                    Publisher = createGameInput.Publisher,
                    Developer = createGameInput.Developer,
                    Price = createGameInput.Price
                };

                await _gameRepository.AddAsync(newGame);

                _logger.LogInformation("Game created successfully: {GameName}", newGame.Name);

                return Result<Game>.Success(newGame);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating game: {Message}", ex.Message);
                throw new InvalidOperationException("An unexpected error occurred while creating the game.", ex);
            }
        }
    }
}