using BL.Model.Database;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.WebAPI.IncomingModel
{
    public class AddFirstAdminClient
    {
        public ModifyAspNetUser Admin { get; set; }
        public string ClientCode { get; set; }
        public string VerificationCode { get; set; }
    }
}