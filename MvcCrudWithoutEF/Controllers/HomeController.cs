using MvcCrudWithoutEF.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace MvcCrudWithoutEF.Controllers
{
    public class HomeController : Controller
    {
        private string connectionString = @"Server=EXPLORE_NIKKS\SQLEXPRESS;Database=UserDB;Integrated Security=True;";

        // GET: Home
        public ActionResult Index()
        {
            List<User> users = GetUsers();
            return View(users);
        }

        // CREATE: Home/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid) // Check if model is valid
            {
                SaveUserData(user.Name, user.Age);
                TempData["message"] = "Data created";
                return RedirectToAction("Index");
            }
            return View(user); // Return the user to the form if validation fails
        }

        // READ: Method to fetch users
        private List<User> GetUsers()
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Name, Age FROM Users";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    User user = new User
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Age = Convert.ToInt32(reader["Age"])
                    };
                    users.Add(user);
                }
                reader.Close();
            }
            return users;
        }

        // UPDATE: Home/Edit/5
        public ActionResult Edit(int id)
        {
            User user = GetUserById(id);
            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(int id, User user)
        {
            if (ModelState.IsValid) // Check if model is valid
            {
                UpdateUser(id, user.Name, user.Age);
                TempData["message"] = "Data updated";
                return RedirectToAction("Index");
            }
            return View(user); // Return the user to the form if validation fails
        }

        // DELETE: Home/Delete/5
        public ActionResult Delete(int id)
        {
            User user = GetUserById(id);
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            DeleteUser(id);
            TempData["message"] = "Data deleted";
            return RedirectToAction("Index");
        }

        // ADO.NET Operations
        private void SaveUserData(string name, int age)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Users (Name, Age) VALUES (@Name, @Age)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Age", age);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private User GetUserById(int id)
        {
            User user = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Name, Age FROM Users WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    user = new User
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Age = Convert.ToInt32(reader["Age"])
                    };
                }
                reader.Close();
            }
            return user;
        }

        private void UpdateUser(int id, string name, int age)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Users SET Name = @Name, Age = @Age WHERE Id = @Id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Age", age);
               
                    connection.Open();
                   command.ExecuteNonQuery();
                
            }
        }

        private void DeleteUser(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Users WHERE Id = @Id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
