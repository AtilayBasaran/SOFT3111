using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace COBADO.Models
{
    public class EventViewModel
    {
        public IEnumerable<eventCategory> eventCategories { get; set; }
        public IEnumerable<eventDetail> eventDetails { get; set; }
    }
}