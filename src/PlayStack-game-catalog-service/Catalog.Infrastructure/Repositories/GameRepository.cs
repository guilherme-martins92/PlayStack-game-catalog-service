using Microsoft.EntityFrameworkCore;
using PlayStack_game_catalog_service.Catalog.Domain.Entities;
using PlayStack_game_catalog_service.Catalog.Domain.Interfaces;

namespace PlayStack_game_catalog_service.Catalog.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly CatalogDbContext _context;

        public GameRepository(CatalogDbContext catalogDbContext)
        {
            _context = catalogDbContext ?? throw new ArgumentNullException(nameof(catalogDbContext));
        }

        public async Task AddAsync(Game game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Game game)
        {
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Game>> GetAllAsync()
        {
            return await _context.Games.AsNoTracking().ToListAsync();
        }

        public async Task<Game?> GetByIdAsync(int id)
        {
            return await _context.Games.FindAsync(id);
        }

        public async Task UpdateAsync(Game game)
        {
            _context.Games.Update(game);
            await _context.SaveChangesAsync();
        }
    }
}