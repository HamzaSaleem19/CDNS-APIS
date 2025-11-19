using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDNSBlazorApp.Models
{
    [Table("SUBSCRIPTIONS")]
    public class Subscription
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(15)]
        public string SubscriptionId { get; set; } = string.Empty;

        [Key]
        [Column(Order = 1)]
        public int SubscriptionIdTraNo { get; set; } = 1;

        [Column("TR_DATE")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Transaction date is required")]
        public DateTime TrDate { get; set; }

        [Column("RECEIVED_DATE")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Received date is required")]
        public DateTime ReceivedDate { get; set; }

        [Column("CD_TRANSACTION_TYPE")]
        [StringLength(5)]
        public string? CdTransactionType { get; set; }

        [Column("LOCAL_EXCHANGE_DATE")]
        [DataType(DataType.Date)]
        public DateTime? LocalExchangeDate { get; set; }

        [Column("CU_BASE")]
        [StringLength(3)]
        public string CuBase { get; set; } = "PKR";

        [Column("RECEIPTS_AMOUNT", TypeName = "decimal(21,3)")]
        [Required(ErrorMessage = "Receipts amount is required")]
        [Range(0.001, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal ReceiptsAmount { get; set; }

        [Column("CD_EXEC_MODE")]
        [StringLength(5)]
        public string CdExecMode { get; set; } = "1";

        [ForeignKey("SubscriptionId")]
        public virtual Instrument? Instrument { get; set; }
    }
}
