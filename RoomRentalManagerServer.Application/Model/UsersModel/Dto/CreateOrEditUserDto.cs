using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomRentalManagerServer.Application.Model.UsersModel.Dto
{
    public class CreateOrEditUserDto
    {
        public long? Id { get; set; }
        public int RoleGroupId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public long ProvinceId { get; set; }
        public long DistrictId { get; set; }
        public long WardId { get; set; }
        public string Address { get; set; }
        public long IDCard { get; set; }
        public string Job { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string BikeId { get; set; }
    }
}
