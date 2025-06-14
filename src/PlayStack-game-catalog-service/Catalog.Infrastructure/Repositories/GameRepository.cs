using PlayStack_game_catalog_service.Catalog.Domain.Entities;
using PlayStack_game_catalog_service.Catalog.Domain.Interfaces;
using System.Collections.Concurrent;

namespace PlayStack_game_catalog_service.Catalog.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private static readonly ConcurrentDictionary<int, Game> _games = new ConcurrentDictionary<int, Game>(
            new[]
            {
                    new KeyValuePair<int, Game>(1, new Game
                    {
                        Id = 1,
                        Name = "Galactic Conquest",
                        Description = "A space strategy game where you conquer planets.",
                        Genre = "Strategy",
                        ReleaseDate = new DateTime(2022, 5, 10, 0, 0, 0, DateTimeKind.Utc),
                        Publisher = "Star Games",
                        Developer = "Nova Studios",
                        Price = 49.99m
                    }),
                    new KeyValuePair<int, Game>(2, new Game
                    {
                        Id = 2,
                        Name = "Dungeon Explorer",
                        Description = "Explore dungeons and defeat monsters.",
                        Genre = "RPG",
                        ReleaseDate = new DateTime(2023, 2, 15,0,0,0,DateTimeKind.Utc),
                        Publisher = "Adventure Works",
                        Developer = "CaveSoft",
                        Price = 39.99m
                    }),
                    new KeyValuePair<int, Game>(3, new Game
                    {
                        Id = 3,
                        Name = "Speed Racer X",
                        Description = "High-octane racing with futuristic cars.",
                        Genre = "Racing",
                        ReleaseDate = new DateTime(2021, 11, 20, 0, 0, 0, DateTimeKind.Utc),
                        Publisher = "Fast Lane",
                        Developer = "Velocity Inc.",
                        Price = 29.99m
                    }),
                    new KeyValuePair<int, Game>(4, new Game
                    {
                        Id = 4,
                        Name = "Mystic Quest",
                        Description = "Embark on a magical journey in a fantasy world.",
                        Genre = "Adventure",
                        ReleaseDate = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                        Publisher = "Magic Realm",
                        Developer = "DreamForge",
                        Price = 59.99m
                    }),
                    new KeyValuePair<int, Game>(5, new Game
                    {
                        Id = 5,
                        Name = "Battlefield Heroes",
                        Description = "Multiplayer shooter with tactical gameplay.",
                        Genre = "Shooter",
                        ReleaseDate = new DateTime(2020, 8, 30, 0, 0, 0, DateTimeKind.Utc),
                        Publisher = "Combat Studios",
                        Developer = "WarZone Devs",
                        Price = 44.99m
                    })
            }
        );

        public Task AddAsync(Game game)
        {
            _games[game.Id] = game;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            _games.TryRemove(id, out _);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Game>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Game>>(_games.Values);
        }

        public Task<Game?> GetByIdAsync(int id)
        {
            _games.TryGetValue(id, out var game);
            return Task.FromResult(game);
        }

        public Task UpdateAsync(Game game)
        {
            if (_games.ContainsKey(game.Id))
            {
                _games[game.Id] = game;
            }
            else
            {
                throw new KeyNotFoundException($"Game with ID {game.Id} not found.");
            }
            return Task.CompletedTask;
        }
    }
}