using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualQueueSystem.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; } // Primary Key

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; } // Link to User Table

        [Required]
        public string Items { get; set; } = string.Empty; // Store ordered items as JSON or string list

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QueueNumber { get; set; } // Auto-increment queue number (FCFS)

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Status: Pending → Preparing → Ready → Completed

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Store in UTC for consistency
    }
}
