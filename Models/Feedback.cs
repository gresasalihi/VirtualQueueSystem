using System;
using System.ComponentModel.DataAnnotations;

namespace VirtualQueueSystem.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
        public string Comment { get; set; } = string.Empty; // ✅ Fixed Null Issue

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now; // ✅ Default Value

        public virtual User? User { get; set; } // ✅ Nullable Fix
    }
}
