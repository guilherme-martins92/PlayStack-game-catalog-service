using PlayStack_game_catalog_service.Catalog.Application.Common;
using PlayStack_game_catalog_service.Catalog.Domain.Entities;
using PlayStack_game_catalog_service.Catalog.Domain.Interfaces;

namespace PlayStack_game_catalog_service.Catalog.Application.UseCases
{
    public class GetGameByIdUseCase
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<GetGameByIdUseCase> _logger;

        public GetGameByIdUseCase(IGameRepository gameRepository, ILogger<GetGameByIdUseCase> logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }
        public async Task<Result<Game?>> ExecuteAsync(int id)
        {
            try
            {
                var game = await _gameRepository.GetByIdAsync(id);

                if (game == null)
                {
                    _logger.LogWarning("Game with ID {Id} not found.", id);
                    return Result<Game?>.Success(null);
                }

                return Result<Game?>.Success(game);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving game with ID {Id}: {Message}", id, ex.Message);
                return Result<Game?>.Failure(new List<string> { "An unexpected error occurred while retrieving the game." });
            }
        }
    }
}