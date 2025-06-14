using FluentValidation;
using PlayStack_game_catalog_service.Catalog.Application.DTOs;

namespace PlayStack_game_catalog_service.Catalog.Application.Validators
{
    public class GameValidator : AbstractValidator<GameDto>
    {
        public GameValidator()
        {
            RuleFor(game => game.Name)
                .NotEmpty().WithMessage("O nome do jogo é obrigatório.")
                .Length(2, 100).WithMessage("O nome do jogo deve ter entre 2 e 100 caracteres.");
            RuleFor(game => game.Description)
                .NotEmpty().WithMessage("A Descrição do jogo é obrigatória.")
                .Length(10, 500).WithMessage("A descrição do jogo deve ter entre 10 e 500 caracteres.");
            RuleFor(game => game.Genre)
                .NotEmpty().WithMessage("O gênero do jogo é obrigatório.")
                .Length(2, 50).WithMessage("O gênero do jogo deve ter entre 2 e 50 caracteres.");
            RuleFor(game => game.ReleaseDate)
                .NotEmpty().WithMessage("A Data de lançamento é obrigatória.");
            RuleFor(game => game.Publisher)
                .NotEmpty().WithMessage("O nome da publisher é obrigatório.")
                .Length(2, 100).WithMessage("O nome da publisher deve ter entre 2 e 100 caracteres.");
            RuleFor(game => game.Developer)
                .NotEmpty().WithMessage("O nome da desenvolvedora é obrigatório.")
                .Length(2, 100).WithMessage("O nome da desenvolvedora deve ter entre 2 e 100 caracteres.");
            RuleFor(game => game.Price)
                .GreaterThanOrEqualTo(0).WithMessage("O preço não pode ser negativo.");
        }
    }
}