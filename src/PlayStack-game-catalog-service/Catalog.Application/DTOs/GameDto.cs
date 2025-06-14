namespace PlayStack_game_catalog_service.Catalog.Application.DTOs
{
    public class GameDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Genre { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? Publisher { get; set; }
        public string? Developer { get; set; }
        public decimal Price { get; set; }
    }
}