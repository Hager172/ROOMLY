using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ROOMLY.models;

namespace WebApplication1.models
{
    public class RoomlyContext: IdentityDbContext<ApplicationUser>
    {

     public   RoomlyContext(DbContextOptions<RoomlyContext>options):base(options) {
        
        
        
        }


        public DbSet<Room> Rooms { get; set; }

        public DbSet<RoomType> roomTypes { get; set; }

        public DbSet<Reservation> reservations { get; set; }
        public DbSet<RoomImage> RoomImages { get; set; }

        public DbSet<Favourite> Favorite { get; set; }




    }

}
