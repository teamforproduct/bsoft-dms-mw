using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.System
{
    public partial class SystemPermissions
    {
        public int Id { get; set; }

        [Index("IX_ModuleFeatureAccessType", 1, IsUnique = true)]
        public int ModuleFeatureId { get; set; }

        [Index("IX_ModuleFeatureAccessType", 2, IsUnique = true)]
        public int AccessTypeId { get; set; }



        [ForeignKey("ModuleFeatureId")]
        public virtual SystemModuleFetures ModuleFeature { get; set; }

        [ForeignKey("AccessTypeId")]
        public virtual SystemAccessTypes AccessType { get; set; }

    }
}
