namespace ROOMLY.DTOs.RoomDTO
{
    public class RoomDetailsDto
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public int RoomTypeId { get; set; }
        public string MainImage { get; set; }
        public List<string> GalleryImages { get; set; }
    }
}
