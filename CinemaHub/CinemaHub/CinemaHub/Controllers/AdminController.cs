using CinemaHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaHub.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Profile()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Edit()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Edit(User user)
        {
            User currentUser = UserController.currentUser;
            bool isChange = false;
            if (user.name != null && currentUser.name != user.name)
            {
                currentUser.name = user.name;
            }
            if (user.surname != null && currentUser.surname != user.surname)
            {
                currentUser.surname = user.surname;
            }
            if (currentUser.gender != user.gender)
            {
                currentUser.gender = user.gender;
            }
            if (user.phoneNumber != null && currentUser.phoneNumber != user.phoneNumber)
            {
                currentUser.phoneNumber = user.phoneNumber;
            }
            if (currentUser.birthday != user.birthday)
            {
                currentUser.birthday = user.birthday;
            }
            if (user.password != null && currentUser.password != user.password)
            {
                currentUser.password = user.password;
                isChange = true;
            }

            UserController userController = new UserController();

            UserController.currentUser = currentUser;
            userController.UpdateCurrentUser();
            if (isChange == true)
            {
                return RedirectToAction("signout", "user");
            }
            else
            {
                return RedirectToAction("index", "home");
            }
        }

        public ActionResult listOfUsers()
        {
            UserController.users.Clear();
            string cmd = "Select * from [dbo].[UserAccount]";
            UserController userController = new UserController();
            List<User> users = userController.FetchUsers(cmd);
            ViewBag.users = users;

            return View();
        }
        public ActionResult listOfMovies()
        {
            string cmd = "Select * from [dbo].[Movie]";
            MovieController movieController = new MovieController();
            List<MovieHub> movies = movieController.FetchMovies(cmd);
            ViewBag.movies = movies;
            return View();
        }

        [HttpGet]
        public ActionResult AddMovie()
        {
            ViewBag.categories = MovieController.getCategories();
            return View();
        }
        [HttpPost]
        public ActionResult AddMovie(MovieHub movie, MovieHub imageModel)
        {
            MovieController movieController = new MovieController();

            movieController.AddMovie(movie, imageModel);
            return RedirectToAction("listOfMovies", "admin");
        }
        [HttpGet]
        public ActionResult UpdateMovie(int movie_ID)
        {
            ViewBag.categories = MovieController.getCategories();
            MovieController movieController = new MovieController();

            String cmd = "Select * from [dbo].[Movie] where movie_ID='" + movie_ID + "';";
            MovieHub movie = movieController.FetchMovies(cmd)[0];

            ViewBag.movie = movie;
            return View();
        }
        [HttpPost]
        public ActionResult UpdateMovie(MovieHub movie, MovieHub imageModel, int movie_ID)
        {
            MovieController movieController = new MovieController();
            string cmd = "Select * from [dbo].[Movie] where movie_ID='" + movie_ID + "';";
            MovieHub tempMovie = movieController.FetchMovies(cmd)[0];

            if (movie.description != null && tempMovie.description != movie.description)
            {
                tempMovie.description = movie.description;
            }
            if (movie.duration != 0 && tempMovie.duration != movie.duration)
            {
                tempMovie.duration = movie.duration;
            }
            if (movie.movieName != null && tempMovie.movieName != movie.movieName)
            {
                tempMovie.movieName = movie.movieName;
            }
            if (movie.viewStatus != null && tempMovie.viewStatus != movie.viewStatus)
            {
                tempMovie.viewStatus = movie.viewStatus;
            }
            if (movie.trailerLink != null && tempMovie.trailerLink != movie.trailerLink)
            {
                tempMovie.trailerLink = movie.trailerLink;
            }
            if (movie.director != null && tempMovie.director != movie.director)
            {
                tempMovie.director = movie.director;
            }
            if (movie.year != 0 && tempMovie.year != movie.year)
            {
                tempMovie.year = movie.year;
            }


            movieController.UpdateMovie(tempMovie, imageModel);
            return RedirectToAction("listOfMovies", "admin");
        }
     
        public ActionResult DeleteMovie(int movie_ID)
        {
            MovieController movieController = new MovieController();
            movieController.DeleteMovie(movie_ID);
            return RedirectToAction("listOfMovies", "admin");
        }

        public ActionResult listOfTickets()
        {
            string cmd = "Select t.*, c.cinemaName,m.movieName, u.name, u.surname, s.seatLetter, s.seatNo from [dbo].[Ticket] t join [dbo].[Cinema] c on t.cinema_ID = c.cinema_ID join [dbo].[Movie] m on t.movie_ID=m.movie_ID join [dbo].[UserAccount] u on t.user_ID = u.user_ID join [dbo].[Seat] s on t.seat_ID = s.seat_ID;";
            TicketController ticketController = new TicketController();
            List<MovieTicket> tickets = ticketController.FetchTickets(cmd);
            ViewBag.tickets = tickets;
            return View();
        }

        public ActionResult DeleteTicket(int ticket_ID)
        {
            TicketController ticketController = new TicketController();
            ticketController.DeleteTicket(ticket_ID);
            return RedirectToAction("listOfTickets", "admin");
        }

    }
}
