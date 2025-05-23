﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.models
{
    public class Reservation
    {
        // Primary Key
        [Key]
        public int ResevationId { get; set; }
        

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public decimal TotalPrice { get; set; }

        // Relation with Room 
        [ForeignKey("Room")]
        public int RoomId { get; set; }

        public virtual Room Room { get; set; }

        // Relation with Customer 
        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
