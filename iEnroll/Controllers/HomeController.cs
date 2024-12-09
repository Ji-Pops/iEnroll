using iEnroll.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace iEnroll.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Enroll()
        {
            return View();
        }

        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult SavePerson(Person person)
        {
            if (!ModelState.IsValid || person == null)
            {
                return Json(new { success = false, message = "Invalid input data." });
            }

            try
            {
                // Retrieve connection string from appsettings.json
                string connectionString = _configuration.GetConnectionString("dbConn");

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL query to insert data into the "persons" table
                    string query = "INSERT INTO person (FirstName, MiddleName, LastName) VALUES (@FirstName, @MiddleName, @LastName)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@FirstName", person.FirstName);
                        command.Parameters.AddWithValue("@MiddleName", string.IsNullOrEmpty(person.MiddleName) ? DBNull.Value : person.MiddleName);
                        command.Parameters.AddWithValue("@LastName", person.LastName);

                        // Execute the query
                        command.ExecuteNonQuery();
                    }
                }

                //return Json(new { success = true, message = "Person saved successfully!" });
                return View("Index");
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine("Error: " + ex.Message);

                return Json(new { success = false, message = "An error occurred while saving the person." });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


