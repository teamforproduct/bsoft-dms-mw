using BL.Model.Common;
using BL.Model.AdminCore.IncomingModel;

namespace BL.Model.AdminCore.InternalModel
{
    public class InternalAdminPositionRole : LastChangeInfo
    {
        public InternalAdminPositionRole()
        { }

        public InternalAdminPositionRole(ModifyAdminPositionRole model)
        {
            Id = model.Id;
            PositionId = model.PositionId;
            RoleId = model.RoleId;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public int PositionId { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public int RoleId { get; set; }

    }
}