﻿using BL.Model.Database;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.WebAPI.IncomingModel
{
    public class AddAspNetClient
    {
        public ModifyAspNetUser Admin { get; set; }
        public ModifyAspNetClient Client { get; set; }
        public ModifyAdminServer Server { get; set; }
        public ModifyAspNetClientLicence Licence { get; set; }
    }
}