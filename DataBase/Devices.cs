using System.Collections.Generic;

namespace DataBase
{

    public class Devices : IEntity
    {
        public int Id { get; set; }
        public string IP { get; set; }
        //public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Responses> Responses { get; set; }
        public Devices()
        {
            Responses = new HashSet<Responses>();
        }
    }
}
