using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ROOMLY.models;

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

        public string? MainImage { get; set; }
        public virtual RoomType RoomType { get; set; }

        public virtual ICollection<RoomImage> RoomImages { get; set; }

        //favourite
        public virtual ICollection<Favourite> Favorites { get; set; } = new List<Favourite>();


        // Relation with Reservation (Foreign Key)
        public virtual ICollection<Reservation> Reservations { get; set; }
        [NotMapped]
        public RoomStatus Status
        {
            get
            {
                var now = DateTime.Now;
                bool isBooked = Reservations?.Any(r => r.CheckInDate<= now && r.CheckOutDate >= now) ?? false;

                return isBooked ? RoomStatus.Booked : RoomStatus.Available;
            }
        }
    }
    public enum RoomStatus
    {
        Available,
        Booked
    }
}
