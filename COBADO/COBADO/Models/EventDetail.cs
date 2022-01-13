using Microsoft.AspNetCore.Mvc;

namespace COBADO.Models
{
    public class eventDetail
    {

        public string EventName { get; set; }
        public string PhotoUrl { get; set; }
        public string EventPlace { get; set; }
        public string EventFull { get; set; }
        public string EventSummary { get; set; }
        public string EventDate { get; set; }
        public string videoUrl { get; set; }
        public int EventID { get; set; }
    }
}
