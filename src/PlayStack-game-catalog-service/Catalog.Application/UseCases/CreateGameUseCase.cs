using FluentValidation;
using PlayStack_game_catalog_service.Catalog.Application.Common;
using PlayStack_game_catalog_service.Catalog.Application.DTOs;
using PlayStack_game_catalog_service.Catalog.Domain.Entities;
using PlayStack_game_catalog_service.Catalog.Domain.Interfaces;

namespace PlayStack_game_catalog_service.Catalog.Application.UseCases
{
    public class CreateGameUseCase
    {
        private readonly IGameRepository _gameRepository;
        private readonly IValidator<GameDto> _validator;
        private readonly ILogger<CreateGameUseCase> _logger;

        public CreateGameUseCase(IGameRepository gameRepository, IValidator<GameDto> validator, ILogger<CreateGameUseCase> logger)
        {
            _gameRepository = gameRepository;
            _validator = validator;
            _logger = logger;
        }
        public async Task<Result<Game>> ExecuteAsync(GameDto gameDto)
        {
            try
            {
                if (gameDto == null)
                {
                    _logger.LogInformation("GameDto is null.");
                    return Result<Game>.Failure(new List<string> { "Game data cannot be null." });
                }

                var result = await _validator.ValidateAsync(gameDto);

                if (!result.IsValid)
                {
                    _logger.LogInformation("Validation failed for GameDto: {Errors}", result.Errors.Select(e => e.ErrorMessage));
                    return Result<Game>.Failure(result.Errors.Select(e => e.ErrorMessage).ToList());
                }

                var newGame = new Game
                {
                    Name = gameDto.Name ?? throw new ArgumentException("O nome do jogo não pode ser nulo."),
                    Description = gameDto.Description ?? throw new ArgumentException("A descrição do jogo não pode nula."),
                    Genre = gameDto.Genre ?? throw new ArgumentException("O gênero do jogo não pode ser nulo."),
                    ReleaseDate = gameDto.ReleaseDate,
                    Publisher = gameDto.Publisher ?? throw new ArgumentException("A publisher do jogo não pode ser nula."),
                    Developer = gameDto.Developer ?? throw new ArgumentException("A desenvolvedora do jogo não pode ser nula."),
                    Price = gameDto.Price
                };

                await _gameRepository.AddAsync(newGame);

                _logger.LogInformation("Game created successfully: {GameName}", newGame.Name);

                return Result<Game>.Success(newGame);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Validation error while creating game: {Message}", ex.Message);
                return Result<Game>.Failure(new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating game: {Message}", ex.Message);
                return Result<Game>.Failure(new List<string> { "Ocorreu um erro inesperado." });
            }
        }
    }
}