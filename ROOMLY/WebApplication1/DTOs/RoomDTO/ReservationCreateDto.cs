namespace ROOMLY.DTOs.RoomDTO
{
    public class ReservationCreateDto
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int RoomId { get; set; }
        public string UserId { get; set; }
    }
}
