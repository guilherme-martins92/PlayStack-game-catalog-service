using FluentValidation;
using PlayStack_game_catalog_service.Catalog.Application.Common;
using PlayStack_game_catalog_service.Catalog.Application.DTOs;
using PlayStack_game_catalog_service.Catalog.Domain.Entities;
using PlayStack_game_catalog_service.Catalog.Domain.Interfaces;

namespace PlayStack_game_catalog_service.Catalog.Application.UseCases
{
    public class UpdateGameUseCase
    {
        private readonly IGameRepository _gameRepository;
        private readonly IValidator<GameDto> _validator;
        private readonly ILogger<UpdateGameUseCase> _logger;

        public UpdateGameUseCase(IGameRepository gameRepository, IValidator<GameDto> validator, ILogger<UpdateGameUseCase> logger)
        {
            _gameRepository = gameRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<Game>> ExecuteAsync(int id, GameDto gameDto)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(gameDto);
                if (!validationResult.IsValid)
                {
                    _logger.LogInformation("Validation failed for GameDto: {Errors}", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Result<Game>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                }

                var existingGame = await _gameRepository.GetByIdAsync(id);
                if (existingGame == null)
                {
                    _logger.LogWarning("Game with ID {Id} not found.", id);
                    return Result<Game>.Failure(new List<string> { $"Game with ID {id} not found." });
                }

                existingGame.Name = gameDto.Name;
                existingGame.Description = gameDto.Description;
                existingGame.Genre = gameDto.Genre;
                existingGame.ReleaseDate = gameDto.ReleaseDate;
                existingGame.Publisher = gameDto.Publisher;
                existingGame.Developer = gameDto.Developer;
                existingGame.Price = gameDto.Price;
                existingGame.UpdatedAt = DateTime.UtcNow;

                await _gameRepository.UpdateAsync(existingGame);
                _logger.LogInformation("Game with ID {Id} updated successfully.", id);
                return Result<Game>.Success(existingGame);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating game with ID {Id}: {Message}", id, ex.Message);
                throw new InvalidOperationException("An unexpected error occurred while updating the game.", ex);
            }
        }
    }
}