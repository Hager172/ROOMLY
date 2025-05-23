using System.ComponentModel.DataAnnotations;
using WebApplication1.models;

namespace ROOMLY.DTOs.RoomDTO
{
    public class ROOMdto
    {
        public int RoomId { get; set; }
        [Required]
        [StringLength(10)]
        public string RoomNumber { get; set; }
        [Required]
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }

       

        //public IFormFile MainImage { get; set; }

        public int Status { get; set; }

    }
}
