using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomRentalManagerServer.Domain.ModelEntities.RoleGroupPermission
{
    [Table("roleGroupPermission")]
    public class RoleGroupPermission
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
