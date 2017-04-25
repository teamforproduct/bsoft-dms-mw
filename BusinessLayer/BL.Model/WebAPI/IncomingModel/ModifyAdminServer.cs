﻿using BL.Model.Enums;
using System.Runtime.Serialization;

namespace BL.Model.WebAPI.IncomingModel
{
    public class ModifyAdminServer
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public EnumDatabaseType ServerType { get; set; }
        public string DefaultDatabase { get; set; }
        public bool IntegrateSecurity { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string ConnectionString { get; set; }
        public string DefaultSchema { get; set; }
        public int ClientId { get; set; }

    }
}