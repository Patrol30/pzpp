using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PZPP_Web.Models
{
    public class ResponseModel
    {
        public int Id { get; set; }
        public int Device_Id { get; set; }
        public bool Success { get; set; }
        public long PingTime { get; set; }
        public DateTime Time { get; set; }
        
    }
}