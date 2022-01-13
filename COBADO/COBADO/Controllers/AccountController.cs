using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using COBADO.Models;

namespace COBADO.Controllers
{
    public class AccountController : Controller
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        void connectionString()
        {
            con.ConnectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
        }

        [HttpPost]
        public ActionResult Verify(Account acc)
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "select * from login where username='" + acc.email + "'and password='" + acc.Password + "'";
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                con.Close();
                return View("Create");
            }
            else
            {
                con.Close();
                return View("Error");
            }


        }

        [HttpPost]
        public ActionResult reg(Register register)
        {
            string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
            //con.Open();
            //com.Connection = con;
            // com.CommandText = "INSERT INTO users(firstname, lastname, email, password, role, createDate) VALUES'" + register.firstname + "','" + register.lastname + "','" + register.email + "','" + register.Password + "', regUser'" + "', '20120618 10:34:09 AM' '" + "'";

            SqlConnection connection = new SqlConnection(connectionString);

            String query = "INSERT INTO fasticket.users(firstname, lastname, email, password, role, createDate) VALUES (@firstname,@lastname,@email,@password,@role,@createDate)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@firstname", register.firstname);
                command.Parameters.AddWithValue("@lastname", register.lastname);
                command.Parameters.AddWithValue("@email", register.email);
                command.Parameters.AddWithValue("@password", register.Password);
                command.Parameters.AddWithValue("@role", "regUser");
                command.Parameters.AddWithValue("@createDate", "20120618 10:34:09 AM");
                connection.Open();
                command.ExecuteNonQuery();

                return View("Create");
            }
        }
    }
}


