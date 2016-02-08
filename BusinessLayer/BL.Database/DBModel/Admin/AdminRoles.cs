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

    public partial class AdminRoles
    {
        public AdminRoles()
        {
            this.UserRoles = new HashSet<AdminUserRoles>();
            this.RoleActions = new HashSet<AdminRoleActions>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionId { get; set; }
        public int AccessLevelId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual ICollection<AdminUserRoles> UserRoles { get; set; }
        public virtual ICollection<AdminRoleActions> RoleActions { get; set; }

        [ForeignKey("AccessLevelId")]
        public virtual AdminAccessLevels AccessLevel { get; set; }
        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }
    }
}
