using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Application.Common.CommonDto;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.RoleGroupsModel.Dto;
using RoomRentalManagerServer.Application.Model.RoomRentalsModel.Dto;
using RoomRentalManagerServer.Application.Model.UsersModel.Dto;
using RoomRentalManagerServer.Domain.Interfaces.RoleGroupInterfaces;
using RoomRentalManagerServer.Domain.Interfaces.UserInterfaces;
using RoomRentalManagerServer.Domain.ModelEntities.RoleGroups;
using RoomRentalManagerServer.Domain.ModelEntities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomRentalManagerServer.Application.Services
{
    public class RoleGroupAppsService : IRoleGroupAppService
    {
        private readonly ILogger<RoleGroupAppsService> _logger;
        private readonly IMapper _mapper;
        private readonly IRoleGroupRepository _roleGroupRepository;
        public RoleGroupAppsService(ILogger<RoleGroupAppsService> logger, IRoleGroupRepository roleGroupRepository, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _roleGroupRepository = roleGroupRepository;
        }

        public async Task<PagedResultDto<RoleGroupDto>> GetAllRoleGroupsAsync(PagedRequestDto<RoleGroupFilterDto> pagedRequestRoleGroupDto)
        {
            try
            {
                var queryGetAllRoleGroup = await _roleGroupRepository.GetAllRoleGroupAsync();

                // Filter by CreatedDate
                if (pagedRequestRoleGroupDto.Filter?.CreatedAt.HasValue == true)
                {
                    var filterDate = pagedRequestRoleGroupDto.Filter.CreatedAt.Value.Date;
                    queryGetAllRoleGroup = queryGetAllRoleGroup.Where(x => x.CreatedAt.Date == filterDate);
                }

                // Filter by UpdatedDate
                if (pagedRequestRoleGroupDto.Filter?.UpdatedAt.HasValue == true)
                {
                    var filterDate = pagedRequestRoleGroupDto.Filter.UpdatedAt.Value.Date;
                    queryGetAllRoleGroup = queryGetAllRoleGroup.Where(x => x.UpdatedAt.Date == filterDate);
                }

                // Filter by CreatorUser (contains search)
                if (!string.IsNullOrEmpty(pagedRequestRoleGroupDto.Filter?.CreatorUser))
                {
                    queryGetAllRoleGroup = queryGetAllRoleGroup.Where(x => x.CreatorUser != null && x.CreatorUser.Contains(pagedRequestRoleGroupDto.Filter.CreatorUser));
                }

                // Filter by LastUpdateUser (contains search)
                if (!string.IsNullOrEmpty(pagedRequestRoleGroupDto.Filter?.LastUpdateUser))
                {
                    queryGetAllRoleGroup = queryGetAllRoleGroup.Where(x => x.LastUpdateUser != null && x.LastUpdateUser.Contains(pagedRequestRoleGroupDto.Filter.LastUpdateUser));
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(pagedRequestRoleGroupDto.SortOrder) && !string.IsNullOrEmpty(pagedRequestRoleGroupDto.SortBy))
                {
                    queryGetAllRoleGroup = pagedRequestRoleGroupDto.SortOrder == "desc"
                    ? queryGetAllRoleGroup.OrderByDescending(x => EF.Property<object>(x, pagedRequestRoleGroupDto.SortBy))
                    : queryGetAllRoleGroup.OrderBy(x => EF.Property<object>(x, pagedRequestRoleGroupDto.SortBy));
                }

                var total = await queryGetAllRoleGroup.CountAsync();
                var roleGroups = await queryGetAllRoleGroup.Skip((pagedRequestRoleGroupDto.Page - 1) * pagedRequestRoleGroupDto.PageSize).Take(pagedRequestRoleGroupDto.PageSize).ToListAsync();
                var roleGroupDtos = roleGroups.Select(r => _mapper.Map<RoleGroupDto>(r)).ToList();
                return new PagedResultDto<RoleGroupDto>(roleGroupDtos, total);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get all RoleGroups");
                throw;
            }
        }

        public async Task<RoleGroup> GetRoleGroupByIdAsync(long id)
        {
            try
            {
                var roleGroup = await _roleGroupRepository.GetByIdAsync(id);
                return roleGroup;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CreateOrEditRoleGroup(CreateOrEditRoleGroupDto input)
        {
            var action = input.Id != null ? "Edit" : "Create";
            var res = true;
            try
            {
                var roleGroup = _mapper.Map<RoleGroup>(input);
                if (input.Id != null)
                {
                    await UpdateAsync(roleGroup);
                }
                else
                {
                    await AddAsync(roleGroup);
                }
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to {action}: {ex.Message}");
                throw;
            }

        }

        public async Task<bool> UpdateAsync(RoleGroup roleGroup)
        {
            try
            {
                var res = await _roleGroupRepository.UpdateAsync(roleGroup);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update role group: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> AddAsync(RoleGroup roleGroup)
        {
            try
            {
                var res = await _roleGroupRepository.AddAsync(roleGroup);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to add role group: {ex.Message}");
                throw;
            }
        }
    }
}
