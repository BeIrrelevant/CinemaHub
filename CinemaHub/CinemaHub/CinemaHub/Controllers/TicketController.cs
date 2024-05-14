using CinemaHub.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaHub.Controllers
{
    public class TicketController : Controller
    {
        public static List<MovieTicket> tickets = new List<MovieTicket>();

        public static MovieTicket tempTicket = new MovieTicket();
        public static List<int> takenSeats = new List<int>();
        public static List<int> choosenSeats = new List<int>();

        SqlCommand com = new SqlCommand();
        SqlDataReader dataReader;

        SqlConnection connection = new SqlConnection(@"Data Source=.\sqlexpress;Initial Catalog=CinemaHubDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        public List<MovieTicket> FetchTickets(String commandText)
        {
            List<MovieTicket> tickets = new List<MovieTicket>();

            try
            {
                connection.Open();
                com.Connection = connection;
                com.CommandText = commandText;
                dataReader = com.ExecuteReader();

                while (dataReader.Read())
                {
                    tickets.Add(new MovieTicket()
                    {
                        ticket_ID = int.Parse(dataReader["ticket_ID"].ToString()),
                        movie_ID = int.Parse(dataReader["movie_ID"].ToString()),
                        cinema_ID = int.Parse(dataReader["cinema_ID"].ToString()),
                        seat_ID = int.Parse(dataReader["seat_ID"].ToString()),
                        seatNo = int.Parse(dataReader["seatNo"].ToString()),
                        seatLetter = dataReader["seatLetter"].ToString(),
                        user_ID = int.Parse(dataReader["user_ID"].ToString()),
                        date = DateTime.Parse(dataReader["date"].ToString()),
                        session = dataReader["session"].ToString(),
                        cardOwnerName = dataReader["cardOwnerName"].ToString(),
                        cardNumber = dataReader["cardNumber"].ToString(),
                        cardValidDate = dataReader["cardValidDate"].ToString(),
                        cardCVV = dataReader["cardCVV"].ToString(),
                        price = decimal.Parse(dataReader["price"].ToString()),
                        cinemaName = dataReader["cinemaName"].ToString(),
                        movieName = dataReader["movieName"].ToString(),
                        name = dataReader["name"].ToString(),
                        surname = dataReader["surname"].ToString()
                    });
                }

                connection.Close();
            }
            catch (Exception e)
            {
                throw e;
            }


            return tickets;
        }

        public List<CinemaMax> FetchCinemas(String commandText)
        {
            List<CinemaMax> cinemas = new List<CinemaMax>();

            try
            {
                connection.Open();
                com.Connection = connection;
                com.CommandText = commandText;

                dataReader = com.ExecuteReader();

                while (dataReader.Read())
                {
                    cinemas.Add(new CinemaMax()
                    {
                        cinema_ID = (int)dataReader["cinema_ID"],
                        cinemaName = dataReader["cinemaName"].ToString(),

                    });
                }

                connection.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

            return cinemas;
        }
     
        public ActionResult BuyTicket(int movie_ID)
        {
            if (UserController.currentUser == null)
                return RedirectToAction("login", "user");

            tempTicket.movie_ID = movie_ID;

            String commandText = "SELECT * FROM [CinemaHubDB].[dbo].[Movie] where movie_ID = " + movie_ID;
            MovieController movieController = new MovieController();

            MovieHub movie = movieController.FetchMovies(commandText).ToArray().ElementAt(0);
            ViewBag.movieName = movie.movieName;

            commandText = "SELECT cinema_ID, cinemaName FROM [dbo].[Cinema]";


            List<CinemaMax> cinemas = FetchCinemas(commandText);
            ViewBag.cinemas = cinemas;

            return View();


        }


        [HttpGet]
        public ActionResult ChooseSeat()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult ChooseSeat(MovieTicket ticket, int cinema, string session, DateTime date)
        {
            tempTicket.date = date;
            tempTicket.cinema_ID = cinema;
            tempTicket.session = session;

            string commandText = "SELECT * FROM [CinemaHubDB].[dbo].[Ticket] t " +
                "join [CinemaHubDB].[dbo].[Seat] s on t.seat_ID = s.seat_ID JOIN [CinemaHubDB].[dbo].[Cinema] c " +
                "on t.cinema_ID = c.cinema_ID JOIN [CinemaHubDB].[dbo].[UserAccount] u on t.user_ID = u.user_ID " +
                "JOIN [CinemaHubDB].[dbo].[Movie] m ON t.movie_ID = m.movie_ID WHERE t.movie_ID = " + tempTicket.movie_ID +
                " and t.cinema_ID = " + tempTicket.cinema_ID + " and t.date = '" + ticket.date.ToString("yyyy'/'MM'/'dd") +
                "' and t.session = '" + ticket.session + "'";

            List<MovieTicket> tickets = FetchTickets(commandText);

            takenSeats.Clear();
            foreach (MovieTicket t in tickets)
            {
                takenSeats.Add(t.seat_ID);
            }

            return View(tempTicket.cinema_ID);
        }


        public ActionResult addSeat(int seat_ID)
        {
            choosenSeats.Add(seat_ID);

            return RedirectToAction("ChooseSeat", "Ticket", new { cinema = tempTicket.cinema_ID });
        }

        public ActionResult deleteSeat(int seat_ID)
        {
            choosenSeats.Remove(seat_ID);

            return RedirectToAction("chooseSeat", "ticket");
        }

        public ActionResult PaymentTicket()
        {
            if (choosenSeats.Count() == 0)
            {
                ViewBag.errorMessage = "Please choose a seat";
                return RedirectToAction("chooseseat", "ticket");
            }

            return View();
        }

        [HttpPost]
        public ActionResult saveTicket(MovieTicket ticket)
        {


            foreach (int seatID in choosenSeats)
            {
                string cmd = "Insert Into [dbo].[Ticket](cinema_ID, movie_ID, user_ID, seat_ID, date, session, cardOwnerName, " +
                    "cardNumber, cardValidDate, cardCVV, price) values ('" + tempTicket.cinema_ID + "','" + tempTicket.movie_ID + "','" + UserController.currentUser.user_ID +
                    "','" + seatID + "','" + tempTicket.date.ToString("yyyy-MM-dd") + "','" + tempTicket.session + "','" + ticket.cardOwnerName +
                    "','" + ticket.cardNumber + "','" + ticket.cardValidDate + "','" + ticket.cardCVV + "','" + 450 + "');";
                connection.Open();
                com = connection.CreateCommand();
                com.CommandType = CommandType.Text;
                com.CommandText = cmd;
                com.ExecuteNonQuery();
                connection.Close();
            }

            choosenSeats.Clear();

            return RedirectToAction("index", "home");
        }

        public List<MovieTicket> getAllTickets()
        {
            string cmd = "SELECT * FROM [CinemaHubDB].[dbo].[Ticket] t " +
                "join [CinemaHubDB].[dbo].[Seat] s on t.seat_ID = s.seat_ID JOIN [CinemaHubDB].[dbo].[Cinema] c" +
                "on t.cinema_ID = c.cinema_ID JOIN [CinemaHubDB].[dbo].[UserAccount] u on t.user_ID = u.user_ID";
            List<MovieTicket> tickets = new TicketController().FetchTickets(cmd);

            return tickets;
        }

        public void DeleteTicket(int ticket_ID)
        {
            String query = "Delete from [dbo].[Ticket] where ticket_ID='" + ticket_ID + "';";
            connection.Open();
            com.Connection = connection;
            com.CommandText = query;
            dataReader = com.ExecuteReader();
            connection.Close();
        }
    }
}


