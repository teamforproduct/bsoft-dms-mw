using BL.Model.Common;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;

namespace BL.Model.AdminCore.InternalModel
{
    public class InternalAdminRoleAction : LastChangeInfo
    {
        public InternalAdminRoleAction()
        { }

        public InternalAdminRoleAction(ModifyAdminRoleAction model)
        {
            Id = model.Id;
            RoleId = model.RoleId;
            ActionId = model.ActionId;
            RecordId = model.RecordId;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        public int RoleId { get; set; }

        public int ActionId { get; set; }

        public int? RecordId { get; set; }

    }
}