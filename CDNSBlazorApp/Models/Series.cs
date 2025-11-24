using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDNSBlazorApp.Models
{
    [Table("Series")]
    public class Series
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string SerieId { get; set; } = string.Empty;

        [Key]
        [Column(Order = 1)]
        public int SerieTraNo { get; set; } = 1;

        [Column("SERIE_AMT", TypeName = "decimal(21,3)")]
        public decimal SerieAmt { get; set; }

        [Column("SERIE_CURRENCY")]
        [StringLength(3)]
        public string SerieCurrency { get; set; } = "PKR";

        [Column("CD_REORG_GRP")]
        [StringLength(5)]
        public string CdReorgGrp { get; set; } = "4";

        [ForeignKey("SerieId")]
        public virtual Instrument? Instrument { get; set; }
    }
}
