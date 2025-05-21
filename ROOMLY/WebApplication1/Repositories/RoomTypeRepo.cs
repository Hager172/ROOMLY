using WebApplication1.models;

namespace ROOMLY.Repositories
{
    public class RoomTypeRepo:GenericRepo<RoomType>
    {
        private readonly RoomlyContext con;

        public RoomTypeRepo(RoomlyContext con) : base(con)
        {
            this.con = con;
        }

    }
}
