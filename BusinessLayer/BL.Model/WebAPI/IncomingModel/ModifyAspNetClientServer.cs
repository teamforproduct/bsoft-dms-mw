using BL.Model.Database;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.WebAPI.IncomingModel
{
    public class ModifyAspNetClientServer
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ServerId { get; set; }
    }
}