using Catalog.Application.Common;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCases.UpdateGameUseCase
{
    public class UpdateGameUseCase
    {
        private readonly IGameRepository _gameRepository;
        private readonly IValidator<UpdateGameInput> _validator;
        private readonly ILogger<UpdateGameUseCase> _logger;

        public UpdateGameUseCase(IGameRepository gameRepository, IValidator<UpdateGameInput> validator, ILogger<UpdateGameUseCase> logger)
        {
            _gameRepository = gameRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<Game>> ExecuteAsync(int id, UpdateGameInput updateGameInput)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(updateGameInput);
                if (!validationResult.IsValid)
                {
                    _logger.LogInformation("Validation failed for updateGameInput: {Errors}", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Result<Game>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                }

                var existingGame = await _gameRepository.GetByIdAsync(id);
                if (existingGame == null)
                {
                    _logger.LogWarning("Game with ID {Id} not found.", id);
                    return Result<Game>.Failure(new List<string> { $"Game with ID {id} not found." });
                }

                existingGame.Name = updateGameInput.Name;
                existingGame.Description = updateGameInput.Description;
                existingGame.Genre = updateGameInput.Genre;
                existingGame.ReleaseDate = updateGameInput.ReleaseDate;
                existingGame.Publisher = updateGameInput.Publisher;
                existingGame.Developer = updateGameInput.Developer;
                existingGame.Price = updateGameInput.Price;
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