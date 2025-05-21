using Microsoft.AspNetCore.Identity;
using ROOMLY.models;

namespace WebApplication1.models
{
    public class ApplicationUser: IdentityUser
    {



        public string FullName { get; set; }

        public string? Address { get; set; }

        public  virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<Favourite> Favorites { get; set; } = new List<Favourite>();

    }
}
