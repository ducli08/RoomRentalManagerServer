using RoomRentalManagerServer.Application.Model.UsersModel.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomRentalManagerServer.Application.Model.Login.Dto
{
    public class LoginResponseDto
    {
        public string Message { get; set; }
        public string AccessToken { get; set; }
        public UserDto User { get; set; }
    }
}
