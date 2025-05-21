using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebApplication1.models;

namespace ROOMLY.models
{
    public class RoomImage
    {
        [Key]
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        // Foreign Key to Room
        [ForeignKey("Room")]
        public int RoomId { get; set; }

        public virtual Room Room { get; set; }
    }
}
