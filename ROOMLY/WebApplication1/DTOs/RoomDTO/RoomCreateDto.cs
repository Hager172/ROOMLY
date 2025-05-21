using System.ComponentModel.DataAnnotations;

namespace ROOMLY.DTOs.RoomDTO
{
    public class RoomCreateDto
    {
        [Required]
        [StringLength(10)]
        public string RoomNumber { get; set; }
        [Required]
        public int RoomTypeId { get; set; }

        public IFormFile MainImage { get; set; }

        public List<IFormFile> GalleryImages { get; set; }
    }
}
