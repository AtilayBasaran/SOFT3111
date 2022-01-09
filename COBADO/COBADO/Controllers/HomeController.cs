using COBADO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace COBADO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult index()
        {
            if (Request.Cookies["name"] != null)
            {
                ViewBag.message = Request.Cookies["name"];
            }
            else
            {
                ViewBag.message = "You are not registered";
            }
            return View();
        }
        public IActionResult about_us()
        {
            return View();
        }
        public IActionResult adminPanel()
        {
            string[] splitting = (Request.Cookies["name"]).Split(" ");
            Console.WriteLine(splitting[1]);
            if (splitting[1] == "admin")
            {
                 return View("adminPanel");
            }
            else
            {
                return RedirectToAction("index", "Home");
            }
            
        }
        public IActionResult all_event()
        {
            string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
            SqlConnection connection = new SqlConnection(connectionString);

            String sql = "select EventId,EventName,EventPhoto from fasticket.events ";

            SqlCommand cmd = new SqlCommand(sql, connection);

            connection.Open();

            List<AllEvent> eventModel = new List<AllEvent>();
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                var allevent = new AllEvent();
                allevent.EventName = rdr[1].ToString();
                allevent.EventPhoto = rdr[2].ToString();
                allevent.EventID = Int16.Parse(rdr[0].ToString());
                eventModel.Add(allevent);
            }
            return View("all_event",eventModel);
        }
        public IActionResult buy_page()
        {
            return View();
        }
        public IActionResult categories()
        {
            return View();
        }
        public IActionResult contact_us()
        {
            string denemeee = (Request.Cookies["name"]).Split(" ")[2];
            Console.WriteLine(denemeee);
            return View();
        }

        [Route("/Home/events/{eventID:int}")]
        public IActionResult events(int eventID)
        {
             string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
             SqlConnection connection = new SqlConnection(connectionString);

             String sql = "select e.EventName,e.EventPhoto,e.EventDate,e.EventSummary,e.EventFull,e.EventUrl,p.place_address from fasticket.events e inner join fasticket.place p on e.place_id = p.place_id where e.EventID ='" + eventID + "'";

             SqlCommand cmd = new SqlCommand(sql, connection);

             connection.Open();

             List<eventDetail> event_detail_Model = new List<eventDetail>();
             SqlDataReader rdr = cmd.ExecuteReader();

             while (rdr.Read())
             {
                 var event_detail = new eventDetail();
                event_detail.EventName = rdr[0].ToString();
                event_detail.PhotoUrl = rdr[1].ToString();
                event_detail.EventDate = rdr[2].ToString();
                event_detail.EventSummary = rdr[3].ToString();
                event_detail.EventFull = rdr[4].ToString();
                event_detail.videoUrl = rdr[5].ToString();
                event_detail.EventPlace = rdr[6].ToString();
                event_detail.EventID = eventID;
                event_detail_Model.Add(event_detail);
             }

            Console.WriteLine(eventID);
            return View("events", event_detail_Model);
        }
        public IActionResult FAQ()
        {
            return View();
        }
        public IActionResult login()
        {
            if (Request.Cookies["name"] != null)
            {
                return RedirectToAction("index", "Home");
            }
            else
            {
                return View("login");
            }
        }
        public IActionResult music()
        {
            return View();
        }
        public IActionResult ownerEvent()
        {
            return View();
        }
        public IActionResult ownerPanel()
        {
            string[] splitting = (Request.Cookies["name"]).Split(" ");
            Console.WriteLine(splitting[1]);
            if (splitting[1] == "owner")
            {
                return View("ownerPanel");
            }
            else
            {
                return RedirectToAction("index", "Home");
            }
        }
        public IActionResult payment()
        {
            return View();
        }
        public IActionResult profile()
        {
            string usermail = (Request.Cookies["name"]).Split(" ")[0];
            if (Request.Cookies["name"] != null)
            {
                string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
                SqlConnection connection = new SqlConnection(connectionString);
                SqlConnection connection2 = new SqlConnection(connectionString);


                string id_str = (Request.Cookies["name"]).Split(" ")[2];
                int user_id = Int16.Parse(id_str);

                //             --------------------------------------- SQL Queries -------------------------------------------
                String sql = "select role,firstname,lastname,email from fasticket.users where email='" + usermail + "'";
                String sql_ticket = "select t.ticket_id, e.EventName from fasticket.ticket t inner join fasticket.events e on t.event_id = EventID where t.user_id ='" + user_id + "'";
                // -----------------------------------------------------------------------------------------------------------

                SqlCommand cmd = new SqlCommand(sql, connection);
                SqlCommand ticket_cmd = new SqlCommand(sql_ticket, connection2);

                connection.Open();
                connection2.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                SqlDataReader ticket_rdr = ticket_cmd.ExecuteReader();

                

                

                List<ticket> ticketModel = new List<ticket>();
                List<profile> model = new List<profile>();

                while (ticket_rdr.Read())
                {
                    var tck = new ticket();
                    tck.ticket_id = Int16.Parse(ticket_rdr[0].ToString());
                    tck.eventname = ticket_rdr[1].ToString();
                    ticketModel.Add(tck);
                }



                while (rdr.Read())
                {
                    var prof = new profile();
                    prof.role = rdr["role"].ToString();
                    prof.firstname = rdr["firstname"].ToString();
                    prof.lastname = rdr["lastname"].ToString();
                    prof.email = rdr["email"].ToString();
                    model.Add(prof);
                }
                ViewModel profileModel = new ViewModel();

                profileModel.profiles = model;
                profileModel.tickets = ticketModel;

                return View("Profile", profileModel);
            }
            else
            {
                return RedirectToAction("index", "Home");
            }
            
        }
        public IActionResult register()
        {
            return View();
        }
        public IActionResult security()
        {
            return View();
        }
        public IActionResult sport()
        {
            return View();
        }
        public IActionResult successPage()
        {
            return View();
        }
        public IActionResult talk()
        {
            return View();
        }
        public IActionResult theatre()
        {
            return View();
        }
        public IActionResult notFound()
        {
            return View("404Page");
        }

        public IActionResult successLogin()
        {
            return View();
        }

        public IActionResult filterEvent()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
                int result = command.ExecuteNonQuery();

            }
            return RedirectToAction("index","Home");
        }

        [HttpPost]
        public ActionResult Verify(Account acc)
        {
            Boolean isAdmin;
            string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
            con.ConnectionString = connectionString;
            con.Open();
            com.Connection = con;
            com.CommandText = "select role,id from fasticket.users where email='" + acc.email + "'and password='" + acc.Password + "'";
            dr = com.ExecuteReader();
            
            
            /*if (desc == "admin")
            {
                isAdmin = true;
            }
            else
            {
                isAdmin = false;
            }*/
            if (dr.Read())
            {
                string desc = (string)dr["role"];
                int userID = (int)dr["id"];
                Console.WriteLine(desc);
                con.Close();
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Append("name", acc.email+" "+desc+" "+userID, options);

               
                return RedirectToAction("index", "Home");
            }
            else
            {
                con.Close();
                return View("Error");
            }


        }
        public ActionResult logout()
        {
            if(Request.Cookies["name"] != null)
            {
                Response.Cookies.Delete("name");
            }

            return RedirectToAction("index", "Home");
        }

        public ActionResult sendMail(MailContext context)
        {
            MailSender(context.message, context.email, context.subject);
            return RedirectToAction("contact_us");
        }
        public static void MailSender(string body, string sendEmail, string subject)
        {
            var fromAddress = new MailAddress("snolldestek@gmail.com");
            var toAddress = new MailAddress(sendEmail);
            using (var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, "snoll123")
            })
            {
                using (var message = new MailMessage(fromAddress, toAddress) { Subject = subject, Body = body })
                {
                    smtp.Send(message);
                }
            }
        }

        public ActionResult UserDetails(profile prof)
        {
            string usermail = (Request.Cookies["name"]).Split(" ")[0];
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                String sql = "select role,firstname,lastname,email from fasticket.users where email='" + usermail + "'";
                SqlCommand cmd = new SqlCommand(sql, cn);
                cn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                List<profile> model = new List<profile>();
                while (rdr.Read())
                {
                    var details = new profile();
                    prof.role = rdr["role"].ToString();
                    prof.firstname = rdr["firstname"].ToString();
                    prof.lastname = rdr["lastname"].ToString();
                    prof.email = rdr["email"].ToString();
                    model.Add(details);
                }
                return View("Profile", model);
            }
        }


     }
}
