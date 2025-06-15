using PlayStack_game_catalog_service.Catalog.Domain.Entities;

namespace PlayStack_game_catalog_service.Catalog.Domain.Interfaces
{
    public interface IGameRepository
    {
        Task<Game?> GetByIdAsync(int id);
        Task<IEnumerable<Game>> GetAllAsync();
        Task AddAsync(Game game);
        Task UpdateAsync(Game game);
        Task DeleteAsync(Game game);
    }
}