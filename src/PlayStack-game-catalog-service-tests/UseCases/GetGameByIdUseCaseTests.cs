using Microsoft.Extensions.Logging;
using Moq;
using PlayStack_game_catalog_service.Catalog.Application.UseCases;
using PlayStack_game_catalog_service.Catalog.Domain.Entities;
using PlayStack_game_catalog_service.Catalog.Domain.Interfaces;

namespace PlayStack_game_catalog_service_tests.UseCases
{
    public class GetGameByIdUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_ReturnsGame_WhenGameExists()
        {
            // Arrange
            var mockRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<GetGameByIdUseCase>>();
            var game = new Game
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
            };
            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(game);
            var useCase = new GetGameByIdUseCase(mockRepo.Object, mockLogger.Object);

            // Act
            var result = await useCase.ExecuteAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(game.Id, result.Data!.Id);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsNull_WhenGameDoesNotExist()
        {
            // Arrange
            var mockRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<GetGameByIdUseCase>>();
            mockRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Game?)null);
            var useCase = new GetGameByIdUseCase(mockRepo.Object, mockLogger.Object);

            // Act
            var result = await useCase.ExecuteAsync(2);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenExceptionThrown()
        {
            // Arrange
            var mockRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<GetGameByIdUseCase>>();
            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception("DB error"));
            var useCase = new GetGameByIdUseCase(mockRepo.Object, mockLogger.Object);

            // Act
            var result = await useCase.ExecuteAsync(3);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
            Assert.Contains("An unexpected error occurred while retrieving the game.", result.Errors);
        }
    }
}