using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS_WebAPI.DBModel
{
    public class AspNetClients
    {
        public AspNetClients()
        {
            this.Servers = new HashSet<AdminServers>();
        }
        public int Id { get; set; }
        [MaxLength(2000)]
        public string Name { get; set; }
        public virtual ICollection<AdminServers> Servers { get; set; }
    }
}
