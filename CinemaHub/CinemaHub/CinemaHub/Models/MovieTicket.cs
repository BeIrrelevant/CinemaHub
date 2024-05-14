using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CinemaHub.Models
{
    public class MovieTicket
    {
        public int ticket_ID { get; set; }
        public int cinema_ID { get; set; }
        public int movie_ID { get; set; }
        public string cinemaName { get; set; }
        public string movieName { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public int user_ID { get; set; }
        public int seat_ID { get; set; }
        public string seatLetter { get; set; }
        public int seatNo { get; set; }
        public DateTime date { get; set; }
        public String session { get; set; }
        public String cardOwnerName { get; set; }
        public String cardNumber { get; set; }
        public String cardValidDate { get; set; }
        public String cardCVV { get; set; }
        public decimal price { get; set; }
    }
}