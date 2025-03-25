using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomRentalManagerServer.Domain.ModelEntities.Invoices
{
    [Table("invoice")]
    public class Invoice
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("contractId")]
        public long ContractId { get; set; }

        [Column("invoiceDate")]
        public DateTime InvoiceDate { get; set; }

        [Column("dueDate")]
        public DateTime DueDate { get; set; }

        [Column("totalAmount")]
        public decimal TotalAmount { get; set; }

        [Column("invoiceStatus")]
        public InvoiceStatus Status { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [Column("creatorUser")]
        public string CreatorUser { get; set; }

        [Column("lastUpdateUser")]
        public string LastUpdateUser { get; set; }

    }
    public enum InvoiceStatus
    {
        [Display(Name = "Chưa thanh toán")]
        Unpaid = 1,
        [Display(Name = "Đã thanh toán")]
        Paid = 2,
        [Display(Name = "Quá hạn")]
        Overdue = 3
    }
}
