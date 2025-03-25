using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomRentalManagerServer.Domain.ModelEntities.PaymentAmount
{
    [Table("payment")]
    public class Payment
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("invoiceId")]
        public long InvoiceId { get; set; }

        [Column("AmountPaid")]
        [Display(Name = "Số tiền đã thanh toán")]
        public decimal AmountPaid { get; set; }

        [Column("paymentDate")]
        [Display(Name = "Ngày thanh toán")]
        public decimal PaymentDate { get; set; }

        [Column("paymentMethod")]
        [Display(Name = "Phương thức thanh toán")]
        public PaymentMethod PaymentMethod { get; set; }

        [Column("statusPayment")]
        [Display(Name = "Trạng thái thanh toán")]
        public PaymentStatus StatusPayment { get; set; }

        [Column("note")]
        [Display(Name = "Ghi chú")]
        public string Note { get; set; }

        [Column("createdAt")]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; }

        [Column("lastUpdateUser")]
        [Display(Name = "Người cập nhật")]
        public DateTime LastUpdateUser { get; set; }
    }

    public enum PaymentMethod
    {
        [Display(Name = "Tiền mặt")]
        Cash = 1,
        [Display(Name = "Chuyển khoản")]
        BankTransfer = 2,
        [Display(Name = "Thẻ")]
        Card = 3
    }
    public enum PaymentStatus {
        [Display(Name = "Chưa thanh toán")]
        Unpaid = 1,
        [Display(Name = "Đã thanh toán")]
        Paid = 2,
        [Display(Name = "Quá hạn")]
        Overdue = 3
    }
}
