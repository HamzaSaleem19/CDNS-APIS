using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDNSBlazorApp.Models
{
    [Table("Instrument")]
    public class Instrument
    {
        [Key]
        [Column("INSTRUMENT_ID")]
        [StringLength(15)]
        public string InstrumentId { get; set; } = string.Empty;

        [Column("NAME")]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(30)]
        public string Name { get; set; } = string.Empty;

        [Column("LONG_NAME")]
        [StringLength(300)]
        public string? LongName { get; set; }

        [Column("ORIGINAL_INST_ID")]
        [StringLength(30)]
        public string? OriginalInstId { get; set; }

        [Column("ISIN_CODE")]
        [StringLength(12)]
        public string? IsinCode { get; set; }

        [Column("CD_DUR_CAT")]
        [StringLength(5)]
        public string? CdDurCat { get; set; }

        [Column("CD_LO_STATUS")]
        [StringLength(5)]
        public string? CdLoStatus { get; set; }

        [Column("D_SIGN")]
        [DataType(DataType.Date)]
        public DateTime? DSign { get; set; }

        [Column("D_FINAL_MATURITY")]
        [DataType(DataType.Date)]
        public DateTime? DFinalMaturity { get; set; }

        [Column("MATURITY_PERIOD")]
        [StringLength(5)]
        public string? MaturityPeriod { get; set; }

        [Column("AMT", TypeName = "decimal(21,3)")]
        public decimal Amt { get; set; }

        [Column("AMT_DISCOUNTED", TypeName = "decimal(21,3)")]
        public decimal AmtDiscounted { get; set; } = 0;

        [Column("CU_BASE")]
        [StringLength(3)]
        public string CuBase { get; set; } = "PKR";

        [Column("AMT_NET", TypeName = "decimal(21,3)")]
        public decimal AmtNet { get; set; }

        [Column("CD_DEBT_SOURCE")]
        [StringLength(5)]
        public string? CdDebtSource { get; set; }

        [Column("CD_DEBT_TYPE")]
        [StringLength(5)]
        public string? CdDebtType { get; set; }

        [Column("CD_INSTRUMENT_TYPE")]
        [StringLength(5)]
        public string? CdInstrumentType { get; set; }

        [Column("CD_DEBT_SEC_INTR_TYPE")]
        [StringLength(5)]
        public string? CdDebtSecIntrType { get; set; }

        [Column("CD_REORG_GRP")]
        [StringLength(5)]
        public string CdReorgGrp { get; set; } = "4";

        [Column("CD_LO_PRP")]
        [StringLength(5)]
        public string? CdLoPrp { get; set; }

        [Column("CD_ECON_SECT")]
        [StringLength(5)]
        public string? CdEconSect { get; set; }

        [Column("CD_SOURCE_MODULE")]
        [StringLength(10)]
        public string CdSourceModule { get; set; } = "1";

        [Column("CD_USER_CODE_1")]
        [StringLength(10)]
        public string CdUserCode1 { get; set; } = "1";

        [Column("INSTITUATION_ID")]
        [StringLength(5)]
        [ForeignKey("Instituation")]
        public string InstituationId { get; set; } = "CDNS";

        [Column("CREDITOR_ID")]
        public int? CreditorId { get; set; }

        [Column("DEBTOR_ID")]
        public int? DebtorId { get; set; }

        // Navigation properties
        public virtual Instituation? Instituation { get; set; }
        public virtual ICollection<Series>? Series { get; set; }
        public virtual ICollection<SeriesPattern>? SeriesPatterns { get; set; }
        public virtual ICollection<Subscription>? Subscriptions { get; set; }
        public virtual ICollection<Payment>? Payments { get; set; }
    }
}
