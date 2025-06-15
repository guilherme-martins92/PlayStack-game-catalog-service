using Microsoft.Extensions.Logging;
using Moq;
using PlayStack_game_catalog_service.Catalog.Application.UseCases;
using PlayStack_game_catalog_service.Catalog.Domain.Entities;
using PlayStack_game_catalog_service.Catalog.Domain.Interfaces;

namespace PlayStack_game_catalog_service_tests.UseCases
{
    public class GetAllGamesUseCaseTests
    {
        private readonly Mock<IGameRepository> _gameRepositoryMock;
        private readonly Mock<ILogger<GetAllGamesUseCase>> _loggerMock;
        private readonly GetAllGamesUseCase _useCase;

        public GetAllGamesUseCaseTests()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
            _loggerMock = new Mock<ILogger<GetAllGamesUseCase>>();
            _useCase = new GetAllGamesUseCase(_gameRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsSuccessResult_WhenGamesExist()
        {
            // Arrange
            var games = new List<Game>
            {
                new Game
                {
                    Id = 1,
                    Name = "Test Game",
                    Description = "Test Description",
                    Genre = "Action",
                    ReleaseDate = DateTime.UtcNow,
                    Publisher = "Test Publisher",
                    Developer = "Test Developer",
                    Price = 59.99m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _gameRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(games);

            // Act
            var result = await _useCase.ExecuteAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailureResult_WhenNoGamesExist()
        {
            // Arrange     
            _gameRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Game>());

            // Act
            var result = await _useCase.ExecuteAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.Contains("No games found.", result.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowsException()
        {
            // Arrange
            _gameRepositoryMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _useCase.ExecuteAsync());
        }
    }
}