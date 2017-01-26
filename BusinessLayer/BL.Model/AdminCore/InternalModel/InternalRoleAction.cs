using BL.Model.Common;

namespace BL.Model.AdminCore.InternalModel
{
    public class InternalAdminRoleAction : LastChangeInfo
    {
        public InternalAdminRoleAction()
        { }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        public int RoleId { get; set; }

        public int ActionId { get; set; }

        public int? RecordId { get; set; }

    }
}