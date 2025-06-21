namespace Catalog.Application.UseCases.CreateGameUseCase
{
    public class CreateGameInput
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Genre { get; set; }
        public DateTime ReleaseDate { get; set; }
        public required string Publisher { get; set; }
        public required string Developer { get; set; }
        public decimal Price { get; set; }
    }
}