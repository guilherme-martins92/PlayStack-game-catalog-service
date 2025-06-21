using Catalog.Application.UseCases.CreateGameUseCase;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.UnitTests.UseCasesTests
{
    public class CreateGameUseCaseTests
    {
        private readonly Mock<IGameRepository> _gameRepositoryMock;
        private readonly Mock<IValidator<CreateGameInput>> _validatorMock;
        private readonly Mock<ILogger<CreateGameUseCase>> _loggerMock;
        private readonly CreateGameUseCase _useCase;

        public CreateGameUseCaseTests()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
            _validatorMock = new Mock<IValidator<CreateGameInput>>();
            _loggerMock = new Mock<ILogger<CreateGameUseCase>>();
            _useCase = new CreateGameUseCase(_gameRepositoryMock.Object, _validatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenValidationFails()
        {
            // Arrange
            var CreateGameInput = new CreateGameInput
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
            _validatorMock.Setup(v => v.ValidateAsync(CreateGameInput, default)).ReturnsAsync(validationResult);

            // Act
            var result = await _useCase.ExecuteAsync(CreateGameInput);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("O nome do jogo é obrigatório.", result.Errors);
            _gameRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Game>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsSuccess_WhenGameIsValid()
        {
            // Arrange
            var CreateGameInput = new CreateGameInput
            {
                Name = "Test Game",
                Description = "desc",
                Genre = "genre",
                ReleaseDate = DateTime.UtcNow,
                Publisher = "pub",
                Developer = "dev",
                Price = 10
            };
            _validatorMock.Setup(v => v.ValidateAsync(CreateGameInput, default)).ReturnsAsync(new ValidationResult());

            Game? addedGame = null;
            _gameRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Game>()))
                .Callback<Game>(g => addedGame = g)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _useCase.ExecuteAsync(CreateGameInput);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("Test Game", result.Data.Name);
            _gameRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Game>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowsException()
        {
            // Arrange
            var CreateGameInput = new CreateGameInput
            {
                Name = "Test Game",
                Description = "desc",
                Genre = "genre",
                ReleaseDate = DateTime.UtcNow,
                Publisher = "pub",
                Developer = "dev",
                Price = 10
            };
            _validatorMock.Setup(v => v.ValidateAsync(CreateGameInput, default)).ReturnsAsync(new ValidationResult());
            _gameRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Game>()))
                .Throws(new Exception("Database error"));
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.ExecuteAsync(CreateGameInput));
        }
    }
}