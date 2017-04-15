using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class AdminServers
    {
        public AdminServers()
        {
            this.ClientUsers = new HashSet<AspNetUserClientServer>();
        }
        /// <summary>
        /// ID 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// IP or host name 
        /// </summary>
        [MaxLength(2000)]
        public string Address { get; set; }
        /// <summary>
        /// Display name
        /// </summary>
        [MaxLength(2000)]
        public string Name { get; set; }
        /// <summary>
        /// Type of the server (SQL Server / Oracle / MySQL etc.)
        /// </summary>
        [MaxLength(2000)]
        public string ServerType { get; set; }
        /// <summary>
        /// Database name
        /// </summary>
        [MaxLength(2000)]
        public string DefaultDatabase { get; set; }
        /// <summary>
        /// use integrate security or Server security
        /// </summary>
        public bool IntegrateSecurity { get; set; }
        /// <summary>
        /// user name (if required)
        /// </summary>
        [MaxLength(2000)]
        public string UserName { get; set; }
        /// <summary>
        /// user password (if required)
        /// </summary>
        [MaxLength(2000)]
        public string UserPassword { get; set; }
        /// <summary>
        /// Connection string (possible)
        /// </summary>
        [MaxLength(2000)]
        public string ConnectionString { get; set; }
        [MaxLength(2000)]
        public string DefaultSchema { get; set; }


        [ForeignKey("ServerId")]
        public virtual ICollection<AspNetUserClientServer> ClientUsers { get; set; }


    }
}
