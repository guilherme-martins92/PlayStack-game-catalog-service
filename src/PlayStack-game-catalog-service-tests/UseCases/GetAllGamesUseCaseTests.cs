using Microsoft.Extensions.Logging;
using Moq;
using PlayStack_game_catalog_service.Catalog.Application.UseCases;
using PlayStack_game_catalog_service.Catalog.Domain.Entities;
using PlayStack_game_catalog_service.Catalog.Domain.Interfaces;

namespace PlayStack_game_catalog_service_tests.UseCases
{
    public class GetAllGamesUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_ReturnsSuccessWithGames_WhenGamesExist()
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

            var repoMock = new Mock<IGameRepository>();
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(games);

            var loggerMock = new Mock<ILogger<GetAllGamesUseCase>>();

            var useCase = new GetAllGamesUseCase(repoMock.Object, loggerMock.Object);

            // Act
            var result = await useCase.ExecuteAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Equal("Test Game", result.Data.First().Name);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsSuccessWithEmpty_WhenNoGamesExist()
        {
            // Arrange
            var repoMock = new Mock<IGameRepository>();
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Game>());

            var loggerMock = new Mock<ILogger<GetAllGamesUseCase>>();

            var useCase = new GetAllGamesUseCase(repoMock.Object, loggerMock.Object);

            // Act
            var result = await useCase.ExecuteAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenExceptionThrown()
        {
            // Arrange
            var repoMock = new Mock<IGameRepository>();
            repoMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("DB error"));

            var loggerMock = new Mock<ILogger<GetAllGamesUseCase>>();

            var useCase = new GetAllGamesUseCase(repoMock.Object, loggerMock.Object);

            // Act
            var result = await useCase.ExecuteAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.NotEmpty(result.Errors);
            Assert.Contains("An unexpected error occurred while retrieving games.", result.Errors);
        }
    }
}
