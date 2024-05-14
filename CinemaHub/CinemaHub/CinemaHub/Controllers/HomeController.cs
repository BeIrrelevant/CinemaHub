using CinemaHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CinemaHub.Controllers
{
    public class HomeController : Controller
    {
        MovieController movieController = new MovieController();

        [AllowAnonymous]
        public ActionResult Index()
        {
            String cmd = "SELECT * FROM[CinemaHubDB].[dbo].[Movie] where viewStatus = 'in theaters'";
            List<MovieHub> movies = movieController.FetchMovies(cmd);
            ViewBag.inTheaters = movies;
            cmd = "SELECT * FROM[CinemaHubDB].[dbo].[Movie] where viewStatus = 'coming soon'";
            movies = movieController.FetchMovies(cmd);
            ViewBag.comingSoon = movies;
            cmd = "SELECT * FROM[CinemaHubDB].[dbo].[Movie] where viewStatus = 'old movie'";
            movies = movieController.FetchMovies(cmd);
            ViewBag.oldMovies = movies;

        

            return View();
        }

        [AllowAnonymous]
        public ActionResult Search(String searchWord)
        {
            String cmd = "SELECT * FROM [CinemaHubDB].[dbo].[Movie] m JOIN [CinemaHubDB].[dbo].[CategoriesOfMovies] com ON m.movie_ID = com.movie_ID " +
                "JOIN [CinemaHubDB].[dbo].[Category] c ON com.category_ID = c.category_ID where movieName LIKE '%" + searchWord + "%' OR categoryName LIKE '%" +
                searchWord + "%' OR director LIKE '%" + searchWord + "%' OR description LIKE '%" + searchWord + "%';";
            List<MovieHub> movies = movieController.FetchMovies(cmd);
            ViewBag.movies = movies;

            return View();
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            return View();
        }
        
        public ActionResult Error()
        {
            return View("Not Found Error 404");
        }


    }
}