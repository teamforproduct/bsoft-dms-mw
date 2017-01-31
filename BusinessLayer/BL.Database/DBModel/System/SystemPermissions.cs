using BL.Database.DBModel.Admin;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.System
{
    public partial class SystemPermissions
    {
        public SystemPermissions()
        {
            this.RolePermissions = new HashSet<AdminRolePermissions>();
            this.Actions = new HashSet<SystemActions>();
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Index("IX_ModuleFeatureAccessType", 1, IsUnique = true)]
        public int ModuleId { get; set; }

        [Index("IX_ModuleFeatureAccessType", 2, IsUnique = true)]
        public int FeatureId { get; set; }

        [Index("IX_ModuleFeatureAccessType", 3, IsUnique = true)]
        public int AccessTypeId { get; set; }



        [ForeignKey("ModuleId")]
        public virtual SystemModules Module { get; set; }

        [ForeignKey("FeatureId")]
        public virtual SystemFeatures Feature { get; set; }

        [ForeignKey("AccessTypeId")]
        public virtual SystemAccessTypes AccessType { get; set; }

        [ForeignKey("PermissionId")]
        public virtual ICollection<AdminRolePermissions> RolePermissions { get; set; }

        [ForeignKey("PermissionId")]
        public virtual ICollection<SystemActions> Actions { get; set; }


    }
}
