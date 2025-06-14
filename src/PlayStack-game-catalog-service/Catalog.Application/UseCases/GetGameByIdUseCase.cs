using PlayStack_game_catalog_service.Catalog.Domain.Entities;
using PlayStack_game_catalog_service.Catalog.Domain.Interfaces;

namespace PlayStack_game_catalog_service.Catalog.Application.UseCases
{
    public class GetGameByIdUseCase
    {
        private readonly IGameRepository _gameRepository;
        public GetGameByIdUseCase(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }
        public async Task<Game?> ExecuteAsync(int id)
        {
            return await _gameRepository.GetByIdAsync(id);
        }
    }
}