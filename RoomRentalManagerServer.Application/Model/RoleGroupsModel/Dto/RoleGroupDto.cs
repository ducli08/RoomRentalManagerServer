using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomRentalManagerServer.Application.Model.RoleGroupsModel.Dto
{
    public class RoleGroupDto
    {
        [Display(Name = "Tên nhóm quyền", Order = 1)]
        public string Name { get; set; }
        [Display(Name = "Trạng thái", Order = 2)]
        public bool Active { get; set; }
        [Display(Name = "Ngày tạo", Order = 3)]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Ngày cập nhật", Order = 4)]
        public DateTime UpdatedAt { get; set; }
        [Display(Name = "Người tạo", Order = 5)]
        public string CreatorUser { get; set; }
        [Display(Name = "Người cập nhật", Order = 6)]
        public string LastUpdateUser { get; set; }
        [Display(Name = "Mô tả", Order = 7)]
        public string Descriptions { get; set; }
        [Display(Name = "Danh sách quyền", Order = 8)]
        public List<int> RoleIds { get; set; }
    }
}
