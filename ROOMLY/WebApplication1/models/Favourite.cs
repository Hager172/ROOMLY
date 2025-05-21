using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebApplication1.models;

namespace ROOMLY.models
{
    public class Favourite
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Required]
        [ForeignKey(nameof(Room))]
        public int RoomId { get; set; }

        public virtual Room Room { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
