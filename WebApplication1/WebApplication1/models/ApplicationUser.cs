using Microsoft.AspNetCore.Identity;

namespace WebApplication1.models
{
    public class ApplicationUser: IdentityUser
    {



        public string FullName { get; set; }

        public string Address { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
