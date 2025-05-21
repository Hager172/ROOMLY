using Microsoft.EntityFrameworkCore;
using ROOMLY.models;
using WebApplication1.models;

namespace ROOMLY.Repositories
{
    public class RoomImageRepository:GenericRepo<RoomImage>
    {
        private readonly RoomlyContext con;

        public RoomImageRepository(RoomlyContext con) : base(con)
        {
            
        }

    
    }
}
