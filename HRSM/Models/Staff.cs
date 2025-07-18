using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRSM.Models
{
    [NotMapped] // This class doesn't create a new table
    public class Staff : User
    {
        // Staff-specific properties
        [NotMapped]
        public string Department { get; set; } = string.Empty;
        
        [NotMapped]
        public DateTime HireDate { get; set; } = DateTime.Now;

        public Staff()
        {
            // Override the default role to Staff
            Role = "Staff";
        }
    }
} 