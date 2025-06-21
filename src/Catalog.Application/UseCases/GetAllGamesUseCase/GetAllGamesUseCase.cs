using Catalog.Application.Common;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCases.GetAllGamesUseCase
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
                    return Result<IEnumerable<Game>>.Failure(new List<string> { "No games found." });

                }

                _logger.LogInformation("Successfully retrieved {Count} games from the repository.", games.Count());
                return Result<IEnumerable<Game>>.Success(games);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving games from the repository.");
                throw new InvalidOperationException("An error occurred while retrieving games from the repository.", ex);
            }
        }
    }
}