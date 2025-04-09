using AutoMapper;
using RoomRentalManagerServer.Application.Model.RoleGroupsModel.Dto;
using RoomRentalManagerServer.Application.Model.UsersModel.Dto;
using RoomRentalManagerServer.Domain.ModelEntities.RoleGroups;
using RoomRentalManagerServer.Domain.ModelEntities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomRentalManagerServer.Application.Model.RoleGroupsModel.RoleGroupMapper
{
    public class RoleGroupMappingProfile : Profile
    {
        public RoleGroupMappingProfile()
        {
            CreateMap<RoleGroup, CreateOrEditRoleGroupDto>().ReverseMap();
            CreateMap<RoleGroup, RoleGroupDto>().ReverseMap();
        }
    }
}
