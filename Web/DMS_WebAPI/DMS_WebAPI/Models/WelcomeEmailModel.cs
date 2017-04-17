using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_WebAPI.Models
{
    public class WelcomeEmailModel
    {
        public string UserName { get; set; }

        public string ClientUrl { get; set; }

        public string CabinetUrl { get; set; }

        public string SpamUrl { get; set; }
        public string UserEmail { get; set; }

        public string OstreanEmail { get; set; }
    }
}