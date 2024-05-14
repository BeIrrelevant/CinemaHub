using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace CinemaHub.Models
{
    public class MovieHub
    {
        public int movie_ID { get; set; }
        public String description { get; set; }
        public int duration { get; set; }
        public String movieName { get; set; }
        public String viewStatus { get; set; }
        public String trailerLink { get; set; }
        public String director { get; set; }
        public int year { get; set; }
        public string categories { get; set; }
        public String movieImg { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }



    }
}