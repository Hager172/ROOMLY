namespace ROOMLY.DTOs.RoomDTO
{
    public class ReservationDto
    {

        public int ResevationId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
