using BL.Model.Database;
using BL.Model.SystemCore;
using System;

namespace BL.Model.WebAPI.FrontModel
{
    public class FrontAspNetClientServer
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ServerId { get; set; }

        public string ClientName { get; set; }
        public string ServerName { get; set; }

    }
}