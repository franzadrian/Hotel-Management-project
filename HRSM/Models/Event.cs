using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRSM.Models
{
    [Table("EVENT")]
    public class Event
    {
        // Static property for the events page title
        public static string EventsPageTitle { get; set; } = "Events";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("EVENT_ID")]
        public int EventId { get; set; }

        [Required]
        [Column("TITLE")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column("LOCATION")]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Column("DESCRIPTION")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column("EVENT_DATE")]
        public string EventDate { get; set; } = string.Empty;

        [Required]
        [Column("PARTICIPANTS")]
        public string Participants { get; set; } = string.Empty;

        [Column("NOTE")]
        public string? Note { get; set; }

        [Required]
        [Column("IMAGE_PATH")]
        public string ImagePath { get; set; } = string.Empty;

        [Required]
        [Column("DISPLAY_ORDER")]
        public int DisplayOrder { get; set; }

        [Required]
        [Column("IS_ACTIVE")]
        public bool IsActive { get; set; } = true;
    }
} 