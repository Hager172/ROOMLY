using WebApplication1.models;

namespace ROOMLY.Repositories
{
    public class GenericRepo<T> where T : class
    {
        private readonly RoomlyContext con;

        public GenericRepo(RoomlyContext con) 
        {
            this.con = con;
        }

        public List<T> GetAll()
        {
            return con.Set<T>().ToList();
        }

        public T GetbyId(int id) {
        
        return con.Set<T>().Find(id);
        
        }


        public void Add(T item) { 
        
        con.Set<T>().Add(item);
        

        }


        public void edit(T item) { 
        
        con.Entry(item).State=Microsoft.EntityFrameworkCore.EntityState.Modified;
        }


        public void delete(int id ) { 

            var item = GetbyId(id);
            con.Set<T>().Remove(item);

        
            
        }
    }
}
