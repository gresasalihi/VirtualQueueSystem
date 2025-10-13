namespace VirtualQueueSystem.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; } // Breakfast, Lunch, Dinner, Snacks
        public decimal Price { get; set; }

        // ✅ Added missing ImageUrl property to support images
        public string? ImageUrl { get; set; }
    }
}
