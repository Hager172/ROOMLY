using WebApplication1.models;

namespace ROOMLY.Repositories
{
    public class ReservationRepo:GenericRepo<Reservation>

    {
        private readonly RoomlyContext con;

        public ReservationRepo(RoomlyContext con) : base(con)
        {
            this.con = con;
        }

        public List<Reservation> GetUpcomingReservations()
        {
            return con.reservations.Where(r => r.CheckInDate > DateTime.Now).ToList();
        }
        public bool IsRoomAvailable(int roomId, DateTime checkInDate, DateTime checkOutDate, int? reservationId = null)
        {
            return !con.reservations.Any(r =>
                r.RoomId == roomId &&
                (reservationId == null || r.ResevationId != reservationId) && 
                checkInDate < r.CheckOutDate &&
                checkOutDate > r.CheckInDate);
        }

    }
}
