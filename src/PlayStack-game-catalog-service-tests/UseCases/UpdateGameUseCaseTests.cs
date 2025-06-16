using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using PlayStack_game_catalog_service.Catalog.Application.DTOs;
using PlayStack_game_catalog_service.Catalog.Application.UseCases;
using PlayStack_game_catalog_service.Catalog.Domain.Entities;
using PlayStack_game_catalog_service.Catalog.Domain.Interfaces;

namespace PlayStack_game_catalog_service_tests.UseCases
{
    public class UpdateGameUseCaseTests
    {
        private readonly Mock<IGameRepository> _gameRepositoryMock;
        private readonly Mock<IValidator<GameDto>> _validatorMock;
        private readonly Mock<ILogger<UpdateGameUseCase>> _loggerMock;
        private readonly UpdateGameUseCase _useCase;

        public UpdateGameUseCaseTests()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
            _validatorMock = new Mock<IValidator<GameDto>>();
            _loggerMock = new Mock<ILogger<UpdateGameUseCase>>();
            _useCase = new UpdateGameUseCase(_gameRepositoryMock.Object, _validatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenValidationFails()
        {
            // Arrange
            var gameDto = new GameDto
            {
                Name = "",
                Description = "desc",
                Genre = "genre",
                ReleaseDate = DateTime.UtcNow,
                Publisher = "pub",
                Developer = "dev",
                Price = 10
            };
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required.")
            });
            _validatorMock.Setup(v => v.ValidateAsync(gameDto, default)).ReturnsAsync(validationResult);

            // Act
            var result = await _useCase.ExecuteAsync(1, gameDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Name is required.", result.Errors);
            _gameRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _gameRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Game>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenGameNotFound()
        {
            // Arrange
            var gameDto = new GameDto
            {
                Name = "Test Game",
                Description = "desc",
                Genre = "genre",
                ReleaseDate = DateTime.UtcNow,
                Publisher = "pub",
                Developer = "dev",
                Price = 10
            };
            _validatorMock.Setup(v => v.ValidateAsync(gameDto, default)).ReturnsAsync(new ValidationResult());
            _gameRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Game?)null);

            // Act
            var result = await _useCase.ExecuteAsync(1, gameDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Game with ID 1 not found.", result.Errors);
            _gameRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Game>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsSuccess_WhenGameIsUpdated()
        {
            // Arrange
            var gameDto = new GameDto
            {
                Name = "Updated Game",
                Description = "Updated desc",
                Genre = "Updated genre",
                ReleaseDate = DateTime.UtcNow,
                Publisher = "Updated pub",
                Developer = "Updated dev",
                Price = 20
            };
            var existingGame = new Game
            {
                Id = 1,
                Name = "Old Game",
                Description = "Old desc",
                Genre = "Old genre",
                ReleaseDate = DateTime.UtcNow.AddYears(-1),
                Publisher = "Old pub",
                Developer = "Old dev",
                Price = 10,
                CreatedAt = DateTime.UtcNow.AddYears(-1),
                UpdatedAt = DateTime.UtcNow.AddYears(-1)
            };
            _validatorMock.Setup(v => v.ValidateAsync(gameDto, default)).ReturnsAsync(new ValidationResult());
            _gameRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingGame);
            _gameRepositoryMock.Setup(r => r.UpdateAsync(existingGame)).Returns(Task.CompletedTask);

            // Act
            var result = await _useCase.ExecuteAsync(1, gameDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("Updated Game", result.Data.Name);
            Assert.Equal("Updated desc", result.Data.Description);
            Assert.Equal("Updated genre", result.Data.Genre);
            Assert.Equal("Updated pub", result.Data.Publisher);
            Assert.Equal("Updated dev", result.Data.Developer);
            Assert.Equal(20, result.Data.Price);
            _gameRepositoryMock.Verify(r => r.UpdateAsync(existingGame), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ThrowsException_WhenRepositoryThrows()
        {
            // Arrange
            var gameDto = new GameDto
            {
                Name = "Game",
                Description = "desc",
                Genre = "genre",
                ReleaseDate = DateTime.UtcNow,
                Publisher = "pub",
                Developer = "dev",
                Price = 10
            };
            var existingGame = new Game
            {
                Id = 1,
                Name = "Game",
                Description = "desc",
                Genre = "genre",
                ReleaseDate = DateTime.UtcNow,
                Publisher = "pub",
                Developer = "dev",
                Price = 10,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _validatorMock.Setup(v => v.ValidateAsync(gameDto, default)).ReturnsAsync(new ValidationResult());
            _gameRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingGame);
            _gameRepositoryMock.Setup(r => r.UpdateAsync(existingGame)).Throws(new Exception("DB error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.ExecuteAsync(1, gameDto));
        }
    }
}