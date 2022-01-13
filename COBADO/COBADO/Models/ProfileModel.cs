using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace COBADO.Models
{
    public class ViewModel
    {
        public IEnumerable<ticket> tickets { get; set; }
        public IEnumerable<profile> profiles { get; set; }

        public IEnumerable<Event> eventtts { get; set; }
        public IEnumerable<profile> userss { get; set; }
    }
}
