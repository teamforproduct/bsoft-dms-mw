using BL.Model.Database;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.WebAPI.IncomingModel
{
    public class SetClientLicenceKey
    {
        public int ClientLicenceId { get; set; }
        [MaxLength(2000)]
        public string LicenceKey { get; set; }
    }
}