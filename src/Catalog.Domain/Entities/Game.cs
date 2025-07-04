﻿namespace Catalog.Domain.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Genre { get; set; }
        public DateTime ReleaseDate { get; set; }
        public required string Publisher { get; set; }
        public required string Developer { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}