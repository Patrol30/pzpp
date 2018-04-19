using System.Data.Entity;

namespace DataBase
{
    public class PingDataContext: DbContext
    {
        public DbSet<Devices> Devices { get; set; }
        public DbSet<Responses> Responses { get; set; }
    }
}
