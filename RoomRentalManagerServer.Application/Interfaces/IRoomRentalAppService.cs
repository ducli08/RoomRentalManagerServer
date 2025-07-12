using RoomRentalManagerServer.Application.Common.CommonDto;
using RoomRentalManagerServer.Application.Model.RoomRentalsModel.Dto;
using RoomRentalManagerServer.Domain.ModelEntities.RoomRentals;

namespace RoomRentalManagerServer.Application.Interfaces
{
    public interface IRoomRentalAppService
    {
        Task<PagedResultDto<RoomRentalDto>> GetAllRoomRentalAsync(PagedRequestDto<RoomRentalFilterDto> pagedRequestDto);
        Task<RoomRentalDto> GetRoomRentalByIdAsync(long id);
        Task<bool> CreateOrEditRoomRentalAsync(CreateOrEditRoomRentalDto input);
        Task DeleteRoomRentalAsync(long id);
        Task<List<RoomRental>> GetAllRoomRentalForSelectListItem();
    }
}
