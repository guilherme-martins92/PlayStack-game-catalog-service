using Catalog.Application.UseCases.DeleteGameUseCase;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.UnitTests.UseCasesTests
{
    public class DeleteGameUseCaseTests
    {
        private readonly Mock<IGameRepository> _gameRepositoryMock;
        private readonly Mock<ILogger<DeleteGameUseCase>> _loggerMock;
        private readonly DeleteGameUseCase _useCase;

        public DeleteGameUseCaseTests()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
            _loggerMock = new Mock<ILogger<DeleteGameUseCase>>();
            _useCase = new DeleteGameUseCase(_gameRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_GameExists_DeletesGameAndReturnsSuccess()
        {
            // Arrange
            var game = new Game
            {
                Id = 1,
                Name = "Test Game",
                Description = "Test Description",
                Genre = "Action",
                ReleaseDate = System.DateTime.UtcNow,
                Publisher = "Test Publisher",
                Developer = "Test Developer",
                Price = 10.0m,
                CreatedAt = System.DateTime.UtcNow,
                UpdatedAt = System.DateTime.UtcNow
            };

            _gameRepositoryMock.Setup(r => r.GetByIdAsync(game.Id)).ReturnsAsync(game);
            _gameRepositoryMock.Setup(r => r.DeleteAsync(game)).Returns(Task.CompletedTask);

            // Act
            var result = await _useCase.ExecuteAsync(game.Id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            _gameRepositoryMock.Verify(r => r.DeleteAsync(game), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_GameDoesNotExist_ReturnsFailure()
        {
            // Arrange
            int gameId = 42;
            _gameRepositoryMock.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync((Game?)null);

            // Act
            var result = await _useCase.ExecuteAsync(gameId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains($"Game with ID {gameId} not found.", result.Errors);
            _gameRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Game>()), Times.Never);    
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrownException()
        {
            // Arrange
            int gameId = 1;
            _gameRepositoryMock.Setup(r => r.GetByIdAsync(gameId)).Throws(new Exception("Unexpected error"));
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _useCase.ExecuteAsync(gameId));
        }
    }
}