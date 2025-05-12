using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.models
{
    public class Room
    {
        // Primary Key
        [Key]
        public int RoomId { get; set; }

        public string RoomNumber { get; set; }

        // Relation with RoomType (Foreign Key)
        [ForeignKey("RoomType")]
        public int RoomTypeId { get; set; }

        public RoomType RoomType { get; set; }

        // Relation with Reservation (Foreign Key)
        public ICollection<Reservation> Reservations { get; set; }
    }
}
