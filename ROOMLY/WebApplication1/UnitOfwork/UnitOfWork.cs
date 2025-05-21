using ROOMLY.Repositories;
using WebApplication1.models;

namespace ROOMLY.UnitOfwork
{
    public class UnitOfWork
    {
        private readonly RoomlyContext con;
        RoomRepo RoomRepo;
        RoomTypeRepo RoomTypeRepo;
        ReservationRepo ReservationRepo;
        RoomImageRepository RoomImageRepository;
        FavoriteRepo FavoriteRepo;
        public UnitOfWork(RoomlyContext con)
        {
            this.con = con;
        }
        public FavoriteRepo favoriteRepo
        {
            get
            {

                if (FavoriteRepo == null)
                    FavoriteRepo = new FavoriteRepo(con);
                return FavoriteRepo;




            }
        }


        public RoomRepo roomRepo
        {
            get{

                if (RoomRepo == null) 
                    RoomRepo = new RoomRepo(con);
                    return RoomRepo;
                
                
                
            
            }
        }

        public RoomImageRepository roomImageRepository
        {


            get
            {
                if (RoomImageRepository == null)
                    RoomImageRepository = new RoomImageRepository(con);
                return RoomImageRepository;


            }
        }
        public RoomTypeRepo roomTypeRepo
        {
            get
            {

                if (RoomTypeRepo == null)
                    RoomTypeRepo = new RoomTypeRepo(con);
                return RoomTypeRepo;




            }
        }
        public ReservationRepo reservationRepo
        {
            get
            {

                if (ReservationRepo == null)
                    ReservationRepo = new ReservationRepo(con);
                return ReservationRepo;




            }
        }

        public void Save() { 
        
        con.SaveChanges();
        
        }


    }
}
