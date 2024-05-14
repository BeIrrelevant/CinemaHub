using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CinemaHub.Models
{
    public class User
    {
        public int user_ID { get; set; }
        public string name { get; set; }
        public String surname { get; set; }
        public String userName { get; set; }
        public bool gender { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public DateTime birthday { get; set; }
        public string password { get; set; }
        public bool userType { get; set; }
    }
}