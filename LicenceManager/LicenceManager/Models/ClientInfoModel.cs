using System;

namespace LicenceManager.Models
{
    public class ClientInfoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LicenceName { get; set; }
        public DateTime DateStart { get; set; }
        public bool IsValid { get; set; }
        public string LicenceDescription { get; set; }
        public int? NNC { get; set; }
        public int? CNC { get; set; }
        public string Functionality { get; set; }
        public int? DayLimit { get; set; }
        public string ClientCode { get; set; }
        public string LicenceCode { get; set; }
    }
}