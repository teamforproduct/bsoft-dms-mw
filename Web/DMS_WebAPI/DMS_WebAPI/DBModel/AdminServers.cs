using BL.Database.DBModel.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class AdminServers
    {
        /// <summary>
        /// ID 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// IP or host name 
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Display name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Type of the server (SQL Server / Oracle / MySQL etc.)
        /// </summary>
        public string ServerType { get; set; }
        /// <summary>
        /// Database name
        /// </summary>
        public string DefaultDatabase { get; set; }
        /// <summary>
        /// use integrate security or Server security
        /// </summary>
        public bool IntegrateSecurity { get; set; }
        /// <summary>
        /// user name (if required)
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// user password (if required)
        /// </summary>
        public string UserPassword { get; set; }
        /// <summary>
        /// Connection string (possible)
        /// </summary>
        public string ConnectionString { get; set; }
        public string DefaultSchema { get; set; }
    }
}
