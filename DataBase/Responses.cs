using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase
{
    public class Responses : IEntity
    {
        public int Id { get; set; }
        public bool Success { get; set; }
        public long PingTime { get; set; }
        public DateTime Time { get; set; }

        [ForeignKey("Device")]
        public int Device_Id { get; set; }
        public virtual Devices Device { get; set; }
    }
}
