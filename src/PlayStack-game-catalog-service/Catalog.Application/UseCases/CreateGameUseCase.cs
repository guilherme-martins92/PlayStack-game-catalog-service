using FluentValidation;
using PlayStack_game_catalog_service.Catalog.Application.Common;
using PlayStack_game_catalog_service.Catalog.Application.DTOs;
using PlayStack_game_catalog_service.Catalog.Domain.Entities;
using PlayStack_game_catalog_service.Catalog.Domain.Interfaces;

namespace PlayStack_game_catalog_service.Catalog.Application.UseCases
{
    public class CreateGameUseCase
    {
        private readonly IGameRepository _gameRepository;
        private readonly IValidator<GameDto> _validator;
        private readonly ILogger<CreateGameUseCase> _logger;

        public CreateGameUseCase(IGameRepository gameRepository, IValidator<GameDto> validator, ILogger<CreateGameUseCase> logger)
        {
            _gameRepository = gameRepository;
            _validator = validator;
            _logger = logger;
        }
        public async Task<Result<Game>> ExecuteAsync(GameDto gameDto)
        {
            try
            {
                var result = await _validator.ValidateAsync(gameDto);

                if (!result.IsValid)
                {
                    _logger.LogInformation("Validation failed for GameDto: {Errors}", result.Errors.Select(e => e.ErrorMessage));
                    return Result<Game>.Failure(result.Errors.Select(e => e.ErrorMessage).ToList());
                }

                var newGame = new Game
                {
                    Name = gameDto.Name,
                    Description = gameDto.Description,
                    Genre = gameDto.Genre,
                    ReleaseDate = DateTime.SpecifyKind(gameDto.ReleaseDate, DateTimeKind.Utc),
                    Publisher = gameDto.Publisher,
                    Developer = gameDto.Developer,
                    Price = gameDto.Price
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