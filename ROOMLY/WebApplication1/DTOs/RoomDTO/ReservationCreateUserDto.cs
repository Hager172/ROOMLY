namespace ROOMLY.DTOs.RoomDTO
{
    public class ReservationCreateUserDto
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int RoomId { get; set; }
    }
}
