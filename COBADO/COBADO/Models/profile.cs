using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace COBADO.Models
{
    public class profile
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string role { get; set; }
        public string email { get; set; }
    }
}
