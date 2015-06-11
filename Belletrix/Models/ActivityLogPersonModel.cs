using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Belletrix.Models
{
    public class ActivityLogPersonModel
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Description { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }
    }
}
