using Microsoft.EntityFrameworkCore;
using ROOMLY.models;
using WebApplication1.models;

namespace ROOMLY.Repositories
{
    public class FavoriteRepo:GenericRepo<Favourite>
    {
        public RoomlyContext Con { get; }

        public FavoriteRepo(RoomlyContext con):base(con) 
        {
            Con = con;
        }
        public List<Favourite> GetFavoritesByUserId(string userId)
        {
            return Con.Favorite
                .Include(f => f.Room) // لو عايز تجيب بيانات الأوض
                .Where(f => f.UserId == userId)
                .ToList();
        }

        public bool IsFavoriteExists(string userId, int roomId)
        {
            return Con.Favorite.Any(f => f.UserId == userId && f.RoomId == roomId);
        }

        // Add و Delete موجودين في GenericRepo
        // بس ممكن تعيد تعريف Add هنا عشان تمنع التكرار
        public new void Add(Favourite favorite)
        {
            if (IsFavoriteExists(favorite.UserId, favorite.RoomId))
            {
                throw new Exception("This room is already in favorites.");
            }
            base.Add(favorite);
        }
    }
}
