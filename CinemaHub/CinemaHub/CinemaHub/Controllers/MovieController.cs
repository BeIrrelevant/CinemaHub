using CinemaHub.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web;


namespace CinemaHub.Controllers
{
    public class MovieController : Controller
    {
        public static SqlCommand com = new SqlCommand();
        public static SqlDataReader movieReader;
        public static SqlConnection connection = new SqlConnection(@"Data Source=.\sqlexpress;Initial Catalog=CinemaHubDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        public static MovieHub movie;


        public List<MovieHub> FetchMovies(String commandText)
        {
            string connectionString = @"Data Source=.\sqlexpress;Initial Catalog=CinemaHubDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand com = new SqlCommand();
                    com.Connection = connection;
                    com.CommandText = "SELECT movie_ID, categoryName FROM Category c JOIN CategoriesOfMovies m " +
                                            "on m.category_ID = c.category_ID";
                    SqlDataReader categoryReader = com.ExecuteReader();

                    Hashtable categories = new Hashtable();

            
                    while (categoryReader.Read())
                    {
                        int movieID = (int)categoryReader["movie_ID"];
                        if (!categories.ContainsKey(movieID))
                        {
                           
                            categories[movieID] = new ArrayList();
                        }

                        
                        ArrayList list = (ArrayList)categories[movieID];
                        list.Add(categoryReader["categoryName"]);
                    }
                    connection.Close();

                    connection.Open();

                    com.CommandText = commandText;
                    SqlDataReader movieReader = com.ExecuteReader();

                    List<MovieHub> movies = new List<MovieHub>();
                    while (movieReader.Read())
                    {
                       

                        int movieID = (int)movieReader["movie_ID"];

                        string category = "";

                        
                        if (categories.ContainsKey(movieID))
                        {
                           
                            ArrayList list = (ArrayList)categories[movieID];
                            foreach (String s in list)
                            {
                                category += s + " ";
                            }
                        }

                        movies.Add(new MovieHub()
                        {
                            movie_ID = movieID,
                            description = movieReader["description"].ToString(),
                            duration = (int)movieReader["duration"],
                            movieName = movieReader["movieName"].ToString(),
                            viewStatus = movieReader["viewStatus"].ToString(),
                            trailerLink = movieReader["trailerLink"].ToString(),
                            director = movieReader["director"].ToString(),
                            year = (int)movieReader["year"],
                            movieImg = movieReader["movieImg"].ToString(),
                            categories = category,
                         

                        });
                    }

                    connection.Close();
                    return movies;
                }
                catch (Exception)
                {
                    throw;
                }
            }
         

        }

        [Route("movie/MovieIndex/{category_Name?}")]
        public ActionResult MovieIndex(string category_Name)
        {
            string cmd2;
            List<MovieHub> movies = new List<MovieHub>();

            if (category_Name == null)
            {
                cmd2 = "SELECT * FROM [CinemaHubDB].[dbo].[Movie]";
                movies = FetchMovies(cmd2);
                ViewBag.allMovies = movies;
            }
            else
            {
                cmd2 = "SELECT * FROM [CinemaHubDB].[dbo].[Movie]";

                foreach (MovieHub movie in FetchMovies(cmd2))
                {
                    if (movie.categories.Contains(category_Name))
                    {
                        movies.Add(movie);
                    }
                }

                ViewBag.allMovies = movies;
            }
            cmd2 = "SELECT * FROM[CinemaHubDB].[dbo].[Movie] where viewStatus = 'in theaters'";
            movies = FetchMovies(cmd2);
            ViewBag.inTheaters = movies;
            cmd2 = "SELECT * FROM[CinemaHubDB].[dbo].[Movie] where viewStatus = 'coming soon'";
            movies = FetchMovies(cmd2);
            ViewBag.comingSoon = movies;
            cmd2 = "SELECT * FROM[CinemaHubDB].[dbo].[Movie] where viewStatus = 'old movie'";
            movies = FetchMovies(cmd2);
            ViewBag.oldMovies = movies;



            ViewBag.CategoryNames = getCategories();

            return View();
        }

        public ActionResult MovieDetails(int movie_ID)
        {
            String query = "SELECT * FROM [CinemaHubDB].[dbo].[Movie] WHERE movie_ID = " + movie_ID;
            movie = FetchMovies(query).ToArray().ElementAt(0);

            ViewBag.movie = movie;

            return View();
        }


        public static List<string> getCategories()
        {
            string connectionString = @"Data Source=.\sqlexpress;Initial Catalog=CinemaHubDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                List<string> list = new List<string>();

           
                connection.Open();

              
                SqlCommand com = new SqlCommand();
                com.Connection = connection;
                com.CommandText = "SELECT categoryName FROM Category";

              
                using (SqlDataReader categoryReader = com.ExecuteReader())
                {
                    while (categoryReader.Read())
                    {
                        list.Add(categoryReader["categoryName"].ToString());
                    }
                }

                
                connection.Close();

               
                return list;
            }

        }

        public int getCategoryID(String categoryName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source=.\sqlexpress;Initial Catalog=CinemaHubDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
                {
                    connection.Open();

                    using (SqlCommand com = new SqlCommand())
                    {
                        com.Connection = connection;
                        com.CommandText = "SELECT category_ID FROM Category WHERE LOWER(categoryName) = LOWER(@categoryName)";
                        com.Parameters.AddWithValue("@categoryName", categoryName.Trim());

                        using (SqlDataReader categoryReader = com.ExecuteReader())
                        {
                            if (categoryReader.Read())
                            {
                                return (int)categoryReader["category_ID"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
              
                throw;
            }

            return -1;
        }

        public int getMovieID(String movieName)
        {
            SqlConnection connection = new SqlConnection(@"Data Source=.\sqlexpress;Initial Catalog=CinemaHubDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            SqlCommand com = new SqlCommand();
            com.Connection = connection;
            com.CommandText = "SELECT * FROM Movie";

            connection.Open();

            SqlDataReader categoryReader = com.ExecuteReader();

            while (categoryReader.Read())
            {
                if (categoryReader["movieName"].ToString().Trim().ToLower() == movieName.ToLower().Trim())
                {
                    int value = (int)categoryReader["movie_ID"];
                    connection.Close();
                    return value;
                }
            }
            connection.Close();

            return -1;
         
        }

        public void AddMovie(MovieHub movie, MovieHub imageModel)
        {

            string connectionString = @"Data Source=.\sqlexpress;Initial Catalog=CinemaHubDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO [dbo].[Movie] (description, duration, movieName, viewStatus, trailerLink, director, year, movieImg) " +
                               "VALUES ('" + movie.description.Replace('\'', '"') + "','" + movie.duration + "','" + movie.movieName + "','" +
                               movie.viewStatus + "','" + movie.trailerLink + "','" + movie.director + "','" + movie.year + "','" + movie.movieImg + "');";

                SqlCommand com = new SqlCommand(query, connection);
                SqlDataReader movieReader = com.ExecuteReader();
                connection.Close();

                query = "INSERT INTO [dbo].[CategoriesOfMovies] (category_ID, movie_ID) VALUES ('" + getCategoryID(movie.categories) + "','" + getMovieID(movie.movieName) + "');";

                connection.Open();
                com = new SqlCommand(query, connection);
                movieReader = com.ExecuteReader();
                connection.Close();

            }
            if (imageModel != null && imageModel.ImageFile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(imageModel.ImageFile.FileName);
                string extension = Path.GetExtension(imageModel.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string imagePath = "Movie-img/" + fileName;
                string directoryPath = HostingEnvironment.MapPath("~/Movie-img/"); 
                string filePath = Path.Combine(directoryPath, fileName);
                imageModel.ImageFile.SaveAs(filePath);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string updateQuery = $"UPDATE [dbo].[Movie] SET movieImg = '{imagePath}' WHERE movieName = '{movie.movieName}';";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                    updateCommand.ExecuteNonQuery();

                    connection.Close();
                }
            }
        }
        public void UpdateMovie(MovieHub movie, MovieHub imageModel)
        {
            string connectionString = @"Data Source=.\sqlexpress;Initial Catalog=CinemaHubDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();


                string updateQuery = $"UPDATE [dbo].[Movie] SET description='{movie.description.Replace("'", "''")}', duration='{movie.duration}', movieName='{movie.movieName}', viewStatus='{movie.viewStatus}', trailerLink='{movie.trailerLink}', director='{movie.director}', year='{movie.year}' WHERE movie_ID='{movie.movie_ID}';";
                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.ExecuteNonQuery();
                }

                string deleteQuery = $"DELETE FROM [dbo].[CategoriesOfMovies] WHERE movie_ID='{movie.movie_ID}';";
                using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                {
                    deleteCommand.ExecuteNonQuery();
                }

                // Insert new categories of the movie
                string insertQuery = $"INSERT INTO [dbo].[CategoriesOfMovies] (category_ID, movie_ID) VALUES ('{getCategoryID(movie.categories)}', '{movie.movie_ID}');";
                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.ExecuteNonQuery();
                }

                if (imageModel != null && imageModel.ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(imageModel.ImageFile.FileName);
                    string extension = Path.GetExtension(imageModel.ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string imagePath = "Movie-img/" + fileName;
                    string directoryPath = HostingEnvironment.MapPath("~/Movie-img/");
                    string filePath = Path.Combine(directoryPath, fileName);
                    imageModel.ImageFile.SaveAs(filePath);


                    using (SqlConnection connections = new SqlConnection(connectionString))
                    {
                        connections.Open();


                        string updateQuerys = $"UPDATE [dbo].[Movie] SET movieImg = '{imagePath}' WHERE movieName = '{movie.movieName}';";
                        SqlCommand updateCommand = new SqlCommand(updateQuerys, connections);
                        updateCommand.ExecuteNonQuery();

                        connections.Close();
                    }
                }


            }
        }

        public void DeleteMovie(int movie_ID)
        {
            String query = "Delete from [dbo].[Movie] where movie_ID='" + movie_ID + "';";
            connection.Open();
            com.Connection = connection;
            com.CommandText = query;
            movieReader = com.ExecuteReader();
            connection.Close();

            query = "Delete from [dbo].[CategoriesOfMovies] where movie_ID='" + movie_ID + "';";
            connection.Open();
            com.Connection = connection;
            com.CommandText = query;
            movieReader = com.ExecuteReader();
            connection.Close();
        }
    }
}


