using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDNSBlazorApp.Models
{
    [Table("INSTITUATION")]
    public class Instituation
    {
        [Key]
        [Column("INSTITUATION_ID")]
        [StringLength(5)]
        public string InstituationId { get; set; } = "CDNS";

        [Column("INSTITUATION_NAME")]
        [Required(ErrorMessage = "Instituation Name is required")]
        [StringLength(100)]
        public string InstituationName { get; set; } = string.Empty;

        [Column("CONTACT_EMAIL")]
        [StringLength(255)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? ContactEmail { get; set; }

        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("CREATED_AT_VALUE_DATE")]
        [DataType(DataType.Date)]
        public DateTime CreatedAtValueDate { get; set; } = DateTime.Now.Date;

        // Navigation property
        public virtual ICollection<Instrument>? Instruments { get; set; }
    }
}
