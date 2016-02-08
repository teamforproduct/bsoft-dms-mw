using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Admin
{
    using Dictionary;
    using global::System.ComponentModel.DataAnnotations.Schema;
    using System;

    public class AdminUserRoles
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        //public virtual AdminUsers User { get; set; }
        [ForeignKey("RoleId")]
        public virtual AdminRoles Role { get; set; }
        [ForeignKey("UserId")]
        public virtual DictionaryAgents Agent { get; set; }
    }
}
