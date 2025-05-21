using Microsoft.EntityFrameworkCore;
using WebApplication1.models;

namespace ROOMLY.Repositories
{
    public class RoomRepo:GenericRepo<Room>
    {
        private readonly RoomlyContext con;

        public RoomRepo(RoomlyContext con):base(con)
        {
            this.con = con;
        }


        public List<Room> GetRoomsByType(int roomtypeId)
        {
            return con.Rooms.Where(r => r.RoomTypeId == roomtypeId).ToList();

        }

        public List<Room> GetAvailableRooms()
        {
            var rooms = con.Rooms
                .Include(r => r.Reservations)
                .ToList();  

            return rooms.Where(r => r.Status == RoomStatus.Available).ToList(); 
        }



    }
}
