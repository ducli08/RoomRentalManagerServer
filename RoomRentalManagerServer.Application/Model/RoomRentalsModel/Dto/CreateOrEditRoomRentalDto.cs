using RoomRentalManagerServer.Domain.ModelEntities.RoomRentals;

namespace RoomRentalManagerServer.Application.Model.RoomRentalsModel.Dto
{
    public class CreateOrEditRoomRentalDto
    {
        public int RoomNumber { get; set; }
        public RoomType RoomType { get; set; }
        public double Price { get; set; }
        public RoomStatus StatusRoom { get; set; }
        public string Note { get; set; }
        public double Area { get; set; }
        public List<int>? ImagesDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatorUser { get; set; }
        public string LastUpdateUser { get; set; }
    }
}
