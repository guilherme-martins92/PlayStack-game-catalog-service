using PlayStack_game_catalog_service.Catalog.Application.Common;
using PlayStack_game_catalog_service.Catalog.Domain.Entities;
using PlayStack_game_catalog_service.Catalog.Domain.Interfaces;

namespace PlayStack_game_catalog_service.Catalog.Application.UseCases
{
    public class GetAllGamesUseCase
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<GetAllGamesUseCase> _logger;

        public GetAllGamesUseCase(IGameRepository gameRepository, ILogger<GetAllGamesUseCase> logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }
        public async Task<Result<IEnumerable<Game>>> ExecuteAsync()
        {
            try
            {
                var games = await _gameRepository.GetAllAsync();

                if (games == null || !games.Any())
                {
                    _logger.LogInformation("No games found in the repository.");
                    return Result<IEnumerable<Game>>.Success(Enumerable.Empty<Game>());
                }

                _logger.LogInformation("Successfully retrieved {Count} games from the repository.", games.Count());
                return Result<IEnumerable<Game>>.Success(games);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving all games: {Message}", ex.Message);
                return Result<IEnumerable<Game>>.Failure(new List<string> { "An unexpected error occurred while retrieving games." });
            }
        }
    }
}