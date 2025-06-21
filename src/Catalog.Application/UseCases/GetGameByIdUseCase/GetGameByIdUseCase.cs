using Catalog.Application.Common;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCases.GetGameByIdUseCase
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
                    return Result<Game?>.Failure(new List<string> { $"Game with ID {id} not found." });
                }

                return Result<Game?>.Success(game);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the game with ID {GameId}.", id);
                throw new InvalidOperationException($"An error occurred while retrieving the game with ID {id}.", ex);
            }
        }
    }
}