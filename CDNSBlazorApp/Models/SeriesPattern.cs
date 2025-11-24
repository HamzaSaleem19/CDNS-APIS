using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDNSBlazorApp.Models
{
    [Table("SERIES_PATTERN")]
    public class SeriesPattern
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string SeriePatId { get; set; } = string.Empty;

        [Key]
        [Column(Order = 1)]
        public int SeriePatTraNo { get; set; } = 1;

        [Column("CD_PAT_TYPE")]
        [StringLength(5)]
        public string? CdPatType { get; set; }

        [Column("CD_PERIODICITY")]
        [StringLength(5)]
        public string? CdPeriodicity { get; set; }

        [Column("CG_PERIODICITY")]
        [StringLength(5)]
        public string? CgPeriodicity { get; set; }

        [Column("D_FIRST_PAYMENT")]
        [DataType(DataType.Date)]
        public DateTime? DFirstPayment { get; set; }

        [Column("D_LAST_PAYMENT")]
        [DataType(DataType.Date)]
        public DateTime? DLastPayment { get; set; }

        [Column("AMT", TypeName = "decimal(21,3)")]
        public decimal? Amt { get; set; }

        [Column("PERCENTAGE")]
        public int Percentage { get; set; } = 100;

        [ForeignKey("SeriePatId")]
        public virtual Instrument? Instrument { get; set; }
    }
}
