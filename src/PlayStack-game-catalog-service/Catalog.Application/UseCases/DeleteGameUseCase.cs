using PlayStack_game_catalog_service.Catalog.Application.Common;
using PlayStack_game_catalog_service.Catalog.Domain.Interfaces;

namespace PlayStack_game_catalog_service.Catalog.Application.UseCases
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
                _logger.LogError(ex, "Unexpected error while deleting game with ID {Id}: {Message}", id, ex.Message);
                return Result<bool>.Failure(new List<string> { "An unexpected error occurred while deleting the game." });
            }
        }
    }
}