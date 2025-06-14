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
    public class CreateGameUseCaseTests
    {
        private readonly Mock<IGameRepository> _gameRepositoryMock;
        private readonly Mock<IValidator<GameDto>> _validatorMock;
        private readonly Mock<ILogger<CreateGameUseCase>> _loggerMock;
        private readonly CreateGameUseCase _useCase;

        public CreateGameUseCaseTests()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
            _validatorMock = new Mock<IValidator<GameDto>>();
            _loggerMock = new Mock<ILogger<CreateGameUseCase>>();
            _useCase = new CreateGameUseCase(_gameRepositoryMock.Object, _validatorMock.Object, _loggerMock.Object);
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
                new ValidationFailure("Name", "O nome do jogo é obrigatório.")
            });
            _validatorMock.Setup(v => v.ValidateAsync(gameDto, default)).ReturnsAsync(validationResult);

            // Act
            var result = await _useCase.ExecuteAsync(gameDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("O nome do jogo é obrigatório.", result.Errors);
            _gameRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Game>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsSuccess_WhenGameIsValid()
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

            Game? addedGame = null;
            _gameRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Game>()))
                .Callback<Game>(g => addedGame = g)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _useCase.ExecuteAsync(gameDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("Test Game", result.Data.Name);
            _gameRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Game>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenRepositoryThrowsException()
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
            _gameRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Game>())).ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _useCase.ExecuteAsync(gameDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Ocorreu um erro inesperado.", result.Errors);
        }
    }
}