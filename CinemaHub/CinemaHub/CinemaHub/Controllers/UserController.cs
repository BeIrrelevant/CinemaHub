using CinemaHub.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CinemaHub.Controllers
{
    public class UserController : Controller
    {
        SqlCommand com = new SqlCommand();
        SqlDataReader userReader;
        SqlConnection con = new SqlConnection(@"Data Source=.\sqlexpress;Initial Catalog=CinemaHubDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        public static List<User> users = new List<User>();

        public static User currentUser;

        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult LogIn(User user)
        {
            try
            {
                String cmd = "Select * from [dbo].[UserAccount] where email = '" + user.email + "';";
                User temp = FetchUsers(cmd)[0];

                if (user.password == temp.password)
                {
                    currentUser = temp;

                    if (MovieController.movie != null)
                        return RedirectToAction("MovieDetails", "Movie", new { movie_ID = MovieController.movie.movie_ID });

                    return RedirectToAction("index", "home");
                }
                else
                {
                    ViewBag.errorMessage = "Wrong password!";

                    return View();
                }
            }
            catch
            {
                ViewBag.errorMessage = "Wrong email!";
                return View();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(User user)
        {
          
            String cmd = "Select * from [dbo].[UserAccount] where email='" + user.email + "' or userName='" + user.userName + "';";
            List<User> findUser = FetchUsers(cmd);

            if (findUser.Count != 0)
            {
                ViewBag.errorMessage = "ERROR";

                return View();
            }
            else
            {
                string gender;
                if (user.gender) gender = "true";
                else gender = "false";
                string birthday = user.birthday != null ? user.birthday.ToString("yyyy-MM-dd") : string.Empty;

                cmd = "Insert Into [dbo].[UserAccount](name,surname,userName,gender,email,phoneNumber,birthday,password) values ('" + user.name + "','" + user.surname + "','" + user.userName + "','" + gender + "','" + user.email + "','" + user.phoneNumber + "','" + birthday + "','" + user.password + "');";
                con.Open();
                com = con.CreateCommand();
                com.CommandType = CommandType.Text;
                com.CommandText = cmd;
                com.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("index", "home");
            }

        }
        public List<User> FetchUsers(String commandText)
        {

            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = commandText;
                userReader = com.ExecuteReader();

                while (userReader.Read())
                {
                    bool? userType = userReader["userType"] as bool?;
                    users.Add(new User()
                    {
                        user_ID = (int)userReader["user_ID"],
                        name = userReader["name"].ToString(),
                        surname = userReader["surname"].ToString(),
                        userName = userReader["userName"].ToString(),
                        gender = (bool)userReader["gender"],
                        email = userReader["email"].ToString(),
                        phoneNumber = userReader["phoneNumber"].ToString(),
                
                        birthday = DateTime.Parse(userReader["birthday"].ToString()),
                        password = userReader["password"].ToString(),
                    
                        userType = userType.HasValue ? userType.Value : false
                    });
                }

                con.Close();

                return users;
            }
            catch (Exception)
            {
                throw;
            }
        }
        [AllowAnonymous]
        public ActionResult SignOut()
        {
            CinemaHub.Controllers.UserController.currentUser = null;
            CinemaHub.Controllers.UserController.users.Clear();

            return RedirectToAction("index", "home");
        }

 
        public ActionResult Profile()
        {
            return View();
        }


     
        public void UpdateCurrentUser()
        {
            User user = currentUser;
            string gender;
            if (user.gender) gender = "true";
            else gender = "false";
            String cmd = "Update [dbo].[UserAccount] set name='" + user.name + "', surname='" + user.surname + "', gender='" + gender + "', phoneNumber='" + user.phoneNumber + "', birthday='" + user.birthday.ToString("yyyy-MM-dd") + "', password='" + user.password + "' where user_ID=" + user.user_ID + ";";
            con.Open();
            com.Connection = con;

            com.CommandText = cmd;

            userReader = com.ExecuteReader();
            con.Close();
        }


      
        public ActionResult DeleteUser(int user_ID)
        {

            String cmd = "Delete from[dbo].[UserAccount] where user_ID='" + user_ID + "';";
            con.Open();
            com.Connection = con;

            com.CommandText = cmd;

            userReader = com.ExecuteReader();
            con.Close();

            return RedirectToAction("listOfUsers", "admin");

        }

        public ActionResult SetAdmin(int user_ID)
        {
            String cmd = "Update [dbo].[UserAccount] set userType='True' where user_ID='" + user_ID + "';";

            con.Open();
            com.Connection = con;

            com.CommandText = cmd;

            userReader = com.ExecuteReader();
            con.Close();

            return RedirectToAction("listOfUsers", "admin");
        }

        public ActionResult SetUser(int user_ID)
        {
            String cmd = "Update [dbo].[UserAccount] set userType='False' where user_ID='" + user_ID + "';";


            con.Open();
            com.Connection = con;

            com.CommandText = cmd;

            userReader = com.ExecuteReader();
            con.Close();

            return RedirectToAction("listOfUsers", "admin");
        }

        public ActionResult UserTickets()
        {
            string cmd = "Select t.*, c.cinemaName,m.movieName, u.name, u.surname, s.seatLetter, s.seatNo " +
                "from [dbo].[Ticket] t join [dbo].[Cinema] c on t.cinema_ID = c.cinema_ID join [dbo].[Movie] m " +
                "on t.movie_ID=m.movie_ID join [dbo].[UserAccount] u on t.user_ID = u.user_ID join [dbo].[Seat] s on " +
                "t.seat_ID = s.seat_ID WHERE t.user_ID = '" + UserController.currentUser.user_ID + "';";
            TicketController ticketController = new TicketController();
            List<MovieTicket> tickets = ticketController.FetchTickets(cmd);
            ViewBag.tickets = tickets;
            return View();
        }

        public ActionResult DeleteTicket(int ticket_ID)
        {
            TicketController ticketController = new TicketController();
            ticketController.DeleteTicket(ticket_ID);
            return RedirectToAction("userTickets", "user");
        }


    }
}
