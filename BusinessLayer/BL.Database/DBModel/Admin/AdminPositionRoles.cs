using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Database.DBModel.Dictionary;

namespace BL.Database.DBModel.Admin
{
    public class AdminPositionRoles
    {
        public int Id { get; set; }
        [Index("IX_PositionRole", 2, IsUnique = true)]
        [Index("IX_RoleId", 1)]
        public int RoleId { get; set; }
        [Index("IX_PositionRole", 1, IsUnique = true)]
        public int PositionId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("RoleId")]
        public virtual AdminRoles Role { get; set; }
        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }
    }
}
