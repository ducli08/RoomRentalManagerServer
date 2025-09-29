using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomRentalManagerServer.Domain.ModelEntities.RoleGroupRole
{
    [Table("roleGroupRole")]
    public class RoleGroupRole
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("roleGroupId")]
        public long RoleGroupId { get; set; }

        [Column("roleId")]
        public long RoleId { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Column("createdBy")]
        public string? CreatedBy { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("updatedBy")]
        public string? UpdatedBy { get; set; }
    }
}
