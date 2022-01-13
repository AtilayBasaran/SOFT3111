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
            return View("all_event", eventModel);
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
            SqlConnection connection2 = new SqlConnection(connectionString);

            String sql = "select e.EventName,e.EventPhoto,e.EventDate,e.EventSummary,e.EventFull,e.EventUrl,p.place_address from fasticket.events e inner join fasticket.place p on e.place_id = p.place_id where e.EventID ='" + eventID + "'";
            String sql_category = "select category_id,category_type from fasticket.categories where event_id ='" + eventID + "'";

            SqlCommand cmd = new SqlCommand(sql, connection);
            SqlCommand categ_cmd = new SqlCommand(sql_category, connection2);

            connection.Open();
            connection2.Open();

            List<eventDetail> event_detail_Model = new List<eventDetail>();
            List<eventCategory> event_category = new List<eventCategory>();

            SqlDataReader rdr = cmd.ExecuteReader();
            SqlDataReader categ_rdr = categ_cmd.ExecuteReader();

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

            while (categ_rdr.Read())
            {
                var categ_detail = new eventCategory();
                categ_detail.category_id = Int16.Parse(categ_rdr[0].ToString());
                categ_detail.category_type = categ_rdr[1].ToString();

                event_category.Add(categ_detail);
            }

            EventViewModel event_categ_Model = new EventViewModel();

            event_categ_Model.eventCategories = event_category;
            event_categ_Model.eventDetails = event_detail_Model;

            Console.WriteLine(eventID);
            return View("events", event_categ_Model);
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
            string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
            SqlConnection connection = new SqlConnection(connectionString);

            String sql = "select EventId,EventName,EventPhoto from fasticket.events where EventCategory='" + "müzik" + "'";

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
            return View("music", eventModel);
        }
        public IActionResult ownerEvent()
        {
            return View();
        }
        
        [Route("/Home/payment/{eventID:int}")]
        public IActionResult payment(int eventID, categ_chooice cc)
        {
            Console.WriteLine("Chooice buranın içinde : " + cc.placeTicket);
            List<paymentid> paymentModel = new List<paymentid>();

            var pym = new paymentid();
            pym.id = eventID;
            pym.category_id = cc.placeTicket;
            //pym.category_id = cc.category_id;
            paymentModel.Add(pym);
            return View("payment", paymentModel);
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
                //             -----------------------------------------------------------------------------------------------

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
        public IActionResult ownerPanel()
        {
            string[] splitting = (Request.Cookies["name"]).Split(" ");
            string splittingmail = (Request.Cookies["name"]).Split(" ")[0];
            Console.WriteLine(splitting[1]);
            if (Request.Cookies["name"] != null && splitting[1] == "owner")
            {
                string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
                SqlConnection connection = new SqlConnection(connectionString);

                string id_str = (Request.Cookies["name"]).Split(" ")[2];
                int user_id = Int16.Parse(id_str);
                String queryevent = "select * from fasticket.events e inner join fasticket.users u on e.User_id = u.id where e.User_id='" + user_id + "'";

                SqlCommand cmd = new SqlCommand(queryevent, connection);
                connection.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                List<Event> eventm = new List<Event>();

                while (rdr.Read())
                {
                    var evnt = new Event();
                    evnt.EventID = rdr["EventID"].ToString();
                    evnt.EventName = rdr["EventName"].ToString();
                    evnt.EventPhoto = rdr["EventPhoto"].ToString();
                    evnt.EventFull = rdr["EventFull"].ToString();
                    evnt.EventSummary = rdr["EventSummary"].ToString();
                    evnt.place_id = rdr["place_id"].ToString();
                    evnt.city = rdr["city"].ToString();
                    evnt.EventDate = rdr["EventDate"].ToString();
                   
                    eventm.Add(evnt);
                }
                ViewModel eventmodel = new ViewModel();
                eventmodel.eventtts = eventm;

                return View("ownerPanel", eventmodel);


            }
            else
            {
                return RedirectToAction("index", "Home");
            }
        }

        public IActionResult adminPanel()
        {
            string[] splitting = (Request.Cookies["name"]).Split(" ");
            Console.WriteLine(splitting[1]);
            if (splitting[1] == "admin")
            {
                string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
                SqlConnection connection = new SqlConnection(connectionString);
                SqlConnection connection2 = new SqlConnection(connectionString);

                String queryevent = "Select * From fasticket.events ";
                String queryuser = "Select * From fasticket.users ";


                SqlCommand cmd = new SqlCommand(queryevent, connection);
                SqlCommand cmd2 = new SqlCommand(queryuser, connection2);
                connection.Open();
                connection2.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                SqlDataReader rdr2 = cmd2.ExecuteReader();

                List<Event> eventm = new List<Event>();
                List<profile> userm = new List<profile>();
                while (rdr.Read())
                {
                    var evnt = new Event();
                    evnt.EventID = rdr["EventID"].ToString(); ;
                    evnt.EventName = rdr["EventName"].ToString();
                    evnt.EventPhoto = rdr["EventPhoto"].ToString();
                    evnt.EventFull = rdr["EventFull"].ToString();
                    evnt.EventSummary = rdr["EventSummary"].ToString();
                    //evnt.place_id = rdr["place_id"].ToString();
                    evnt.city = rdr["city"].ToString();
                    evnt.EventDate = rdr["EventDate"].ToString();
                    eventm.Add(evnt);
                }

                while (rdr2.Read())
                {
                    var users = new profile();
                    users.id = (int)rdr2["id"];
                    users.role = rdr2["role"].ToString();
                    users.firstname = rdr2["firstname"].ToString(); ;
                    users.lastname = rdr2["lastname"].ToString();


                    userm.Add(users);
                }
                ViewModel eventmodel = new ViewModel();

                eventmodel.eventtts = eventm;
                eventmodel.userss = userm;


                return View("adminPanel", eventmodel);


            }
            else
            {
                return RedirectToAction("index", "Home");
            }

        }
        public IActionResult sport()
        {
            string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
            SqlConnection connection = new SqlConnection(connectionString);

            String sql = "select EventId,EventName,EventPhoto from fasticket.events where EventCategory='" + "spor" + "'";

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
            return View("sport", eventModel);
        }
        public IActionResult succcessPage()
        {
            return View("succcesPage");
        }
        public IActionResult talk()
        {
            string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
            SqlConnection connection = new SqlConnection(connectionString);

            String sql = "select EventId,EventName,EventPhoto from fasticket.events where EventCategory='" + "konuşmalar" + "'";

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
            return View("talk", eventModel);
        }
        public IActionResult theatre()
        {
            string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
            SqlConnection connection = new SqlConnection(connectionString);

            String sql = "select EventId,EventName,EventPhoto from fasticket.events where EventCategory='" + "tiyatro" + "'";

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
            return View("theatre", eventModel);
        }
        public IActionResult notFound()
        {
            return View("404Page");
        }

        public IActionResult succesLogin()
        {
            return View("successLogin");
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
            return RedirectToAction("index", "Home");
        }

        [HttpPost]
        public ActionResult Verify(Account acc)
        {
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
                Response.Cookies.Append("name", acc.email + " " + desc + " " + userID, options);


                return RedirectToAction("index", "Home");
            }
            else
            {
                con.Close();
                return RedirectToAction("login", "Home");
            }


        }
        public ActionResult logout()
        {
            if (Request.Cookies["name"] != null)
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
                Console.WriteLine("deneme");
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

        [Route("/Home/paymentSuccessfull/{eventID:int}/{category_id:int}")]
        public ActionResult paymentSuccessfull(int eventID, int category_id)
        {
            string firstVariable = string.Empty;
            int secondVariable = 0;
            int userID = Int16.Parse((Request.Cookies["name"]).Split(" ")[2]);
            Console.WriteLine(userID);
            Console.WriteLine(category_id);

                string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";

                con.ConnectionString = connectionString;
                con.Open();
                com.Connection = con;
                com.CommandText = "select category_type,category_price from fasticket.categories where category_id='" + category_id + "'";
                dr = com.ExecuteReader();

                while (dr.Read())
                {
                    firstVariable = dr[0].ToString();
                    secondVariable = Int16.Parse(dr[1].ToString());
                }


            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();

            String query = "INSERT INTO fasticket.ticket(user_id, event_id, category_id, ticket_type, ticket_price) VALUES (@userID,@eventID,@category_id,@ticket_type,@ticket_price)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userID", userID);
                command.Parameters.AddWithValue("@eventID", eventID);
                command.Parameters.AddWithValue("@category_id", category_id);
                command.Parameters.AddWithValue("@ticket_type", firstVariable);
                command.Parameters.AddWithValue("@ticket_price", secondVariable);
                int result = command.ExecuteNonQuery();
                
            }

            return RedirectToAction("succcessPage");
        }
        [HttpPost]
        public ActionResult bar(search_Bar SB)
        {
            List<AllEvent> s = new List<AllEvent>();
            string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            var searchB = new AllEvent();
            Console.WriteLine(SB.EventName);
            if (SB.EventName != null)
            {
                String query = "select EventName, EventPhoto, EventID from fasticket.events where eventName like'" + SB.EventName + "%'";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {

                    searchB.EventName = (rdr[0].ToString());
                    searchB.EventPhoto = rdr[1].ToString();
                    searchB.EventID = Int16.Parse(rdr[2].ToString());
                    s.Add(searchB);
                }
                return View("filterEvent", s);
            }
            else
            {
                return View("404Page");
            }
        }
        [HttpPost]
        public ActionResult filterr(filterEvent FE)
        {
            List<AllEvent> allModel = new List<AllEvent>();
            string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();


            if (FE.eventName != null && FE.eventCategory != null && FE.eventDate != null && FE.eventCity != null)
            {
                String query1 = "select EventName,EventPhoto,EventId from fasticket.events  where eventName like'" + FE.eventName + "%'and eventDate='" + FE.eventDate + "'and eventCategory='" + FE.eventCategory + "'and city='" + FE.eventCity + "'";
                SqlCommand cmd1 = new SqlCommand(query1, connection);
                SqlDataReader rdr1 = cmd1.ExecuteReader();
                while (rdr1.Read())
                {
                    var allE = new AllEvent();
                    allE.EventName = (rdr1[0].ToString());
                    allE.EventPhoto = rdr1[1].ToString();
                    allE.EventID = Int16.Parse(rdr1[2].ToString());
                    allModel.Add(allE);
                }
                return View("filterEvent", allModel);
            }
            else if (FE.eventName == null && FE.eventDate != null && FE.eventCategory != null && FE.eventCity != null)
            {
                String query2 = "select EventName,EventPhoto,EventId from fasticket.events  where eventDate='" + FE.eventDate + "'and eventCategory='" + FE.eventCategory + "'and city='" + FE.eventCity + "'";
                SqlCommand cmd2 = new SqlCommand(query2, connection);
                SqlDataReader rdr2 = cmd2.ExecuteReader();
                while (rdr2.Read())
                {
                    var allE = new AllEvent();
                    allE.EventName = (rdr2[0].ToString());
                    allE.EventPhoto = rdr2[1].ToString();
                    allE.EventID = Int16.Parse(rdr2[2].ToString());
                    allModel.Add(allE);
                }
                return View("filterEvent", allModel);
            }
            else if (FE.eventName != null && FE.eventDate != null && FE.eventCategory == null && FE.eventCity != null)
            {
                String query3 = "select EventName,EventPhoto,EventId from fasticket.events  where eventName like'" + FE.eventName + "%'and eventDate='" + FE.eventDate + "'and city='" + FE.eventCity + "'";
                SqlCommand cmd3 = new SqlCommand(query3, connection);
                SqlDataReader rdr3 = cmd3.ExecuteReader();
                while (rdr3.Read())
                {
                    var allE = new AllEvent();
                    allE.EventName = (rdr3[0].ToString());
                    allE.EventPhoto = rdr3[1].ToString();
                    allE.EventID = Int16.Parse(rdr3[2].ToString());
                    allModel.Add(allE);
                }
                return View("filterEvent", allModel);
            }
            else if (FE.eventName != null && FE.eventDate != null && FE.eventCategory != null && FE.eventCity == null)
            {
                String query4 = "select EventName,EventPhoto,EventId from fasticket.events  where eventName like'" + FE.eventName + "%'and eventDate='" + FE.eventDate + "'and eventCategory='" + FE.eventCategory + "'";
                SqlCommand cmd4 = new SqlCommand(query4, connection);
                SqlDataReader rdr4 = cmd4.ExecuteReader();

                while (rdr4.Read())
                {
                    var allE = new AllEvent();
                    allE.EventName = (rdr4[0].ToString());
                    allE.EventPhoto = rdr4[1].ToString();
                    allE.EventID = Int16.Parse(rdr4[2].ToString());
                    allModel.Add(allE);
                }
                return View("filterEvent", allModel);

            }
            else if (FE.eventName != null && FE.eventDate == null && FE.eventCategory != null && FE.eventCity != null)
            {
                String query5 = "select EventName,EventPhoto,EventId from fasticket.events  where eventName like'" + FE.eventName + "%'and eventCategory='" + FE.eventCategory + "'and city='" + FE.eventCity + "'";
                SqlCommand cmd5 = new SqlCommand(query5, connection);
                SqlDataReader rdr5 = cmd5.ExecuteReader();
                while (rdr5.Read())
                {
                    var allE = new AllEvent();
                    allE.EventName = (rdr5[0].ToString());
                    allE.EventPhoto = rdr5[1].ToString();
                    allE.EventID = Int16.Parse(rdr5[2].ToString());
                    allModel.Add(allE);
                }
                return View("filterEvent", allModel);

            }
            else if (FE.eventName == null && FE.eventDate == null && FE.eventCategory != null && FE.eventCity != null)
            {
                String query6 = "select EventName,EventPhoto,EventId from fasticket.events  where eventCategory='" + FE.eventCategory + "'and city='" + FE.eventCity + "'";
                SqlCommand cmd6 = new SqlCommand(query6, connection);
                SqlDataReader rdr6 = cmd6.ExecuteReader();
                while (rdr6.Read())
                {
                    var allE = new AllEvent();
                    allE.EventName = (rdr6[0].ToString());
                    allE.EventPhoto = rdr6[1].ToString();
                    allE.EventID = Int16.Parse(rdr6[2].ToString());
                    allModel.Add(allE);
                }
                return View("filterEvent", allModel);

            }
            else if (FE.eventName == null && FE.eventDate != null && FE.eventCategory == null && FE.eventCity != null)
            {
                String query7 = "select EventName,EventPhoto,EventId from fasticket.events  where eventDate='" + FE.eventDate + "'and city='" + FE.eventCity + "'";
                SqlCommand cmd7 = new SqlCommand(query7, connection);
                SqlDataReader rdr7 = cmd7.ExecuteReader();
                while (rdr7.Read())
                {
                    var allE = new AllEvent();
                    allE.EventName = (rdr7[0].ToString());
                    allE.EventPhoto = rdr7[1].ToString();
                    allE.EventID = Int16.Parse(rdr7[2].ToString());
                    allModel.Add(allE);
                }
                return View("filterEvent", allModel);
            }
            else if (FE.eventName == null && FE.eventDate != null && FE.eventCategory != null && FE.eventCity == null)
            {
                String query8 = "select EventName,EventPhoto,EventId from fasticket.events  where eventDate='" + FE.eventDate + "'and eventCategory='" + FE.eventCategory + "'";
                SqlCommand cmd8 = new SqlCommand(query8, connection);
                SqlDataReader rdr8 = cmd8.ExecuteReader();
                while (rdr8.Read())
                {
                    var allE = new AllEvent();
                    allE.EventName = (rdr8[0].ToString());
                    allE.EventPhoto = rdr8[1].ToString();
                    allE.EventID = Int16.Parse(rdr8[2].ToString());
                    allModel.Add(allE);
                }
                return View("filterEvent", allModel);

            }
            else if (FE.eventName != null && FE.eventDate == null && FE.eventCategory == null && FE.eventCity != null)
            {
                String query9 = "select EventName,EventPhoto,EventId from fasticket.events  where eventName like'" + FE.eventName + "%'and city='" + FE.eventCity + "'";
                SqlCommand cmd9 = new SqlCommand(query9, connection);
                SqlDataReader rdr9 = cmd9.ExecuteReader();
                while (rdr9.Read())
                {
                    var allE = new AllEvent();
                    allE.EventName = (rdr9[0].ToString());
                    allE.EventPhoto = rdr9[1].ToString();
                    allE.EventID = Int16.Parse(rdr9[2].ToString());
                    allModel.Add(allE);
                }
                return View("filterEvent", allModel);
            }
            else if (FE.eventName != null && FE.eventDate == null && FE.eventCategory != null && FE.eventCity == null)
            {
                String query10 = "select EventName,EventPhoto,EventId from fasticket.events  where eventName like'" + FE.eventName + "%'and eventCategory='" + FE.eventCategory + "'";
                SqlCommand cmd10 = new SqlCommand(query10, connection);
                SqlDataReader rdr10 = cmd10.ExecuteReader();
                while (rdr10.Read())
                {
                    var allE = new AllEvent();
                    allE.EventName = (rdr10[0].ToString());
                    allE.EventPhoto = rdr10[1].ToString();
                    allE.EventID = Int16.Parse(rdr10[2].ToString());
                    allModel.Add(allE);
                }
                return View("filterEvent", allModel);
            }
            else if (FE.eventName != null && FE.eventDate != null && FE.eventCategory == null && FE.eventCity == null)
            {
                String query11 = "select EventName,EventPhoto,EventId from fasticket.events  where eventName like'" + FE.eventName + "%'and eventDate='" + FE.eventDate + "'";
                SqlCommand cmd11 = new SqlCommand(query11, connection);
                SqlDataReader rdr11 = cmd11.ExecuteReader();
                while (rdr11.Read())
                {
                    var allE = new AllEvent();
                    allE.EventName = (rdr11[0].ToString());
                    allE.EventPhoto = rdr11[1].ToString();
                    allE.EventID = Int16.Parse(rdr11[2].ToString());
                    allModel.Add(allE);
                }
                return View("filterEvent", allModel);
            }
            else if (FE.eventName == null && FE.eventDate == null && FE.eventCategory == null && FE.eventCity != null)
            {
                String query12 = "select EventName,EventPhoto,EventId from fasticket.events  where city='" + FE.eventCity + "'";
                SqlCommand cmd12 = new SqlCommand(query12, connection);
                SqlDataReader rdr12 = cmd12.ExecuteReader();
                while (rdr12.Read())
                {
                    var allE = new AllEvent();
                    allE.EventName = (rdr12[0].ToString());
                    allE.EventPhoto = rdr12[1].ToString();
                    allE.EventID = Int16.Parse(rdr12[2].ToString());
                    allModel.Add(allE);
                }
                return View("filterEvent", allModel);
            }
            else if (FE.eventName == null && FE.eventDate == null && FE.eventCategory != null && FE.eventCity == null)
            {
                String query13 = "select EventName,EventPhoto,EventId from fasticket.events  where eventCategory='" + FE.eventCategory + "'";
                SqlCommand cmd13 = new SqlCommand(query13, connection);
                SqlDataReader rdr13 = cmd13.ExecuteReader();
                while (rdr13.Read())
                {
                    var allE = new AllEvent();
                    allE.EventName = (rdr13[0].ToString());
                    allE.EventPhoto = rdr13[1].ToString();
                    allE.EventID = Int16.Parse(rdr13[2].ToString());
                    allModel.Add(allE);
                }
                return View("filterEvent", allModel);
            }
            else if (FE.eventName == null && FE.eventDate != null && FE.eventCategory == null && FE.eventCity == null)
            {
                String query14 = "select EventName,EventPhoto,EventId from fasticket.events  where eventDate='" + FE.eventDate + "'";
                SqlCommand cmd14 = new SqlCommand(query14, connection);
                SqlDataReader rdr14 = cmd14.ExecuteReader();
                while (rdr14.Read())
                {
                    var allE = new AllEvent();
                    allE.EventName = (rdr14[0].ToString());
                    allE.EventPhoto = rdr14[1].ToString();
                    allE.EventID = Int16.Parse(rdr14[2].ToString());
                    allModel.Add(allE);
                }
                return View("filterEvent", allModel);
            }
            else
            {
                String query15 = "select EventName,EventPhoto,EventId from fasticket.events  where eventName like'" + FE.eventName + "%'";
                SqlCommand cmd15 = new SqlCommand(query15, connection);
                SqlDataReader rdr15 = cmd15.ExecuteReader();
                while (rdr15.Read())
                {
                    var allE = new AllEvent();
                    allE.EventName = (rdr15[0].ToString());
                    allE.EventPhoto = rdr15[1].ToString();
                    allE.EventID = Int16.Parse(rdr15[2].ToString());
                    allModel.Add(allE);
                }
                return View("filterEvent", allModel);
            }
        }

        [HttpPost]
        public ActionResult EventAdd(Event eventaddp)
        {
            string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
            SqlConnection connection = new SqlConnection(connectionString);

            String query = "INSERT INTO fasticket.events(EventName, EventPhoto, EventFull, EventSummary,place_id, city,EventDate) VALUES (@EventName,@EventPhoto,@EventFull,@EventSummary,@place_id,@city,@EventDate)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                //.Parameters.AddWithValue("@EventID", eventaddp.EventID);
                command.Parameters.AddWithValue("@EventName", eventaddp.EventName);
                command.Parameters.AddWithValue("@EventPhoto", eventaddp.EventPhoto);
                command.Parameters.AddWithValue("@EventFull", eventaddp.EventFull);
                command.Parameters.AddWithValue("@EventSummary", eventaddp.EventSummary);
                command.Parameters.AddWithValue("@place_id", eventaddp.place_id);
                command.Parameters.AddWithValue("@city", eventaddp.city);
                command.Parameters.AddWithValue("@EventDate", "20120618 10:34:09 AM");
                connection.Open();
                try
                {
                    int result = command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    ViewBag.Result = "error" + ex.Message;
                }
                connection.Close();

            }
            return RedirectToAction("adminPanel", "Home");
        }
        [HttpPost]
        public ActionResult EventAddOwner(Event eventaddp)
        {
            string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
            SqlConnection connection = new SqlConnection(connectionString);
            string user_id = (Request.Cookies["name"]).Split(" ")[2];
            String query = $"INSERT INTO fasticket.events(EventName, EventPhoto, EventFull, EventSummary,place_id, city,EventDate,user_id) VALUES (@EventName,@EventPhoto,@EventFull,@EventSummary,@place_id,@city,@EventDate,'{user_id}')";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                //.Parameters.AddWithValue("@EventID", eventaddp.EventID);
                command.Parameters.AddWithValue("@EventName", eventaddp.EventName);
                command.Parameters.AddWithValue("@EventPhoto", eventaddp.EventPhoto);
                command.Parameters.AddWithValue("@EventFull", eventaddp.EventFull);
                command.Parameters.AddWithValue("@EventSummary", eventaddp.EventSummary);
                command.Parameters.AddWithValue("@place_id", eventaddp.place_id);
                command.Parameters.AddWithValue("@city", eventaddp.city);
                command.Parameters.AddWithValue("@EventDate", "20120618 10:34:09 AM");

                connection.Open();
                try
                {
                    int result = command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    ViewBag.Result = "error" + ex.Message;
                }
                connection.Close();

            }
            return RedirectToAction("ownerPanel", "Home");
        }
        [HttpPost]
        [Route("/Home/UpdateEvent/{eventID:int}")]
        public ActionResult UpdateEvent(Event e, int eventID)
        {
            Console.WriteLine("Allahım bize yardım et : " + eventID);
            string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE fasticket.events SET EventName='" + e.EventName + "',EventPhoto='" + e.EventPhoto + "',EventFull='" + e.EventFull + "',EventSummary='" + e.EventSummary + "',city='" + e.city + "',EventDate='" + e.EventDate + "' Where EventID = '" + eventID + "'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return RedirectToAction("adminPanel", "Home");
        }
        [HttpPost]
        public ActionResult DeleteEvent(int id)
        {
            string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
            using (var connection = new SqlConnection(connectionString))
            {
                string query = $"DELETE FROM fasticket.events Where EventID = '{id}'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return RedirectToAction("adminPanel", "Home");
        }
        [HttpPost]
        public ActionResult DeleteEventOwner(int id)
        {
            string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
            using (var connection = new SqlConnection(connectionString))
            {
                string query = $"DELETE FROM fasticket.events Where EventID = '{id}'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return RedirectToAction("ownerPanel", "Home");
        }
        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
            using (var connection = new SqlConnection(connectionString))
            {
                string query = $"DELETE FROM fasticket.users Where id = '{id}'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return RedirectToAction("adminPanel", "Home");
        }
        [HttpPost]
        public ActionResult UpdateAdmin(int id)
        {
            string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
            using (var connection = new SqlConnection(connectionString))
            {
                string query = $"UPDATE fasticket.users SET role='admin'  Where id = '{id}'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return RedirectToAction("adminPanel", "Home");
        }
        [HttpPost]
        public ActionResult UpdateOwner(int id)
        {
            string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
            using (var connection = new SqlConnection(connectionString))
            {
                string query = $"UPDATE fasticket.users SET role='owner'  Where id = '{id}'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return RedirectToAction("adminPanel", "Home");
        }
        [HttpPost]
        public ActionResult UpdateReg(int id)
        {
            string connectionString = "data source=DESKTOP-GNF0LHR; database=ticketsell; integrated security=SSPI;";
            using (var connection = new SqlConnection(connectionString))
            {
                string query = $"UPDATE fasticket.users SET role='reguser'  Where id = '{id}'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return RedirectToAction("adminPanel", "Home");
        }


    }
}
