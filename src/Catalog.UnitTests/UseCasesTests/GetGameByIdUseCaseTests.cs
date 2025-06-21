using Catalog.Application.UseCases.GetGameByIdUseCase;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.UnitTests.UseCasesTests
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
        public async Task ExecuteAsync_ReturnsFailure_WhenGameDoesNotExist()
        {
            // Arrange
            var mockRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<GetGameByIdUseCase>>();
            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Game?)null);
            var useCase = new GetGameByIdUseCase(mockRepo.Object, mockLogger.Object);
            // Act
            var result = await useCase.ExecuteAsync(1);
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.Single(result.Errors);
            Assert.Equal("Game with ID 1 not found.", result.Errors[0]);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowsException()
        {
            // Arrange
            var mockRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<GetGameByIdUseCase>>();
            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));
            var useCase = new GetGameByIdUseCase(mockRepo.Object, mockLogger.Object);
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(1));
        }
    }
}