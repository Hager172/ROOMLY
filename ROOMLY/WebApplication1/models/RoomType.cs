using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.models
{
    public class RoomType
    {
        // Primary Key
        [Key]
        public int RoomTypeId { get; set; }

        public string Name { get; set; }
       public string Description { get; set; }

        public decimal Price { get; set; }

        // Relation with Room (One-to-Many relationship)
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
