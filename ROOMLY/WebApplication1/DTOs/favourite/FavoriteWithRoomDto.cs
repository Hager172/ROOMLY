using ROOMLY.DTOs.RoomDTO;
using WebApplication1.models;

namespace ROOMLY.DTOs.favourite
{
    public class FavoriteWithRoomDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }

        public string RoomNumber { get; set; }
        public string RoomTypeName { get; set; }
        public int Status { get; set; }
    }
}
