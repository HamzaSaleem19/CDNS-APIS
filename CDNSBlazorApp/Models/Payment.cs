using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDNSBlazorApp.Models
{
    [Table("PAYMENTS")]
    public class Payment
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string PaymentId { get; set; } = string.Empty;

        [Key]
        [Column(Order = 1)]
        public int PaymentTraNo { get; set; } = 1;

        [Column("SCH_PAYMENT_DATE")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Scheduled payment date is required")]
        public DateTime SchPaymentDate { get; set; }

        [Column("MADE_DATE")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Made date is required")]
        public DateTime MadeDate { get; set; }

        [Column("RECEIVED_DATE")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Received date is required")]
        public DateTime ReceivedDate { get; set; }

        [Column("LOCAL_EXCH_RATE_DATE")]
        [DataType(DataType.Date)]
        public DateTime? LocalExchRateDate { get; set; }

        [Column("CD_PAYMENT_MODE")]
        [StringLength(5)]
        public string CdPaymentMode { get; set; } = "1";

        [Column("AMOUNT", TypeName = "decimal(21,3)")]
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.001, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Column("CU_BASE")]
        [StringLength(3)]
        public string CuBase { get; set; } = "PKR";

        [Column("CD_TRANSACTION_TYPE")]
        [StringLength(5)]
        public string? CdTransactionType { get; set; }

        [Column("CD_AMOUNT_DIFF")]
        [StringLength(5)]
        public string CdAmountDiff { get; set; } = "1";

        [ForeignKey("PaymentId")]
        public virtual Instrument? Instrument { get; set; }
    }
}
