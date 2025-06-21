using Catalog.Application.Common;
using Catalog.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCases.DeleteGameUseCase
{
    public class DeleteGameUseCase
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<DeleteGameUseCase> _logger;
        public DeleteGameUseCase(IGameRepository gameRepository, ILogger<DeleteGameUseCase> logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }
        public async Task<Result<bool>> ExecuteAsync(int id)
        {
            try
            {
                var game = await _gameRepository.GetByIdAsync(id);
                if (game == null)
                {
                    _logger.LogWarning("Game with ID {Id} not found.", id);
                    return Result<bool>.Failure(new List<string> { $"Game with ID {id} not found." });
                }

                await _gameRepository.DeleteAsync(game);

                _logger.LogInformation("Game with ID {Id} deleted successfully.", id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting game with ID {Id}", id);
                throw new InvalidOperationException("An unexpected error occurred while deleting the game.", ex);
            }
        }
    }
}