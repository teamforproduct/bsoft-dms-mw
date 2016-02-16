using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.System
{
    using Admin;
    using global::System.ComponentModel.DataAnnotations.Schema;


    public partial class SystemActions
    {
        public SystemActions()
        {
            this.RoleActions = new HashSet<AdminRoleActions>();
            this.GarantableActions = new HashSet<SystemActions>();
        }

        public int Id { get; set; }
        public int ObjectId { get; set; }
        public string Code { get; set; }
        public string API { get; set; }
        public string Description { get; set; }
        public bool IsGrantable { get; set; }
        public bool IsGrantableByRecordId { get; set; }
        public Nullable<int> GrantId { get; set; }
        [ForeignKey("ObjectId")]
        public virtual SystemObjects Object { get; set; }

        public virtual ICollection<AdminRoleActions> RoleActions { get; set; }
        [ForeignKey("GrantId")]
        public virtual SystemActions GrantAction { get; set; }

        public virtual ICollection<SystemActions> GarantableActions { get; set; }
    }
}
