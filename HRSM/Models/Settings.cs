using System.ComponentModel.DataAnnotations;

namespace HRSM.Models
{
    public class Settings
    {
        [Key]
        public int SettingId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Key { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Value { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? Description { get; set; }
        
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
} 