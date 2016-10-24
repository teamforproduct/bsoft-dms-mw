using BL.Model.Common;
using BL.Model.AdminCore.IncomingModel;
using System;
using BL.Model.Enums;

namespace BL.Model.AdminCore.InternalModel
{
    public class InternalAdminUserRole : LastChangeInfo
    {
        public InternalAdminUserRole()
        { }

        public InternalAdminUserRole(ModifyAdminUserRole model)
        {
            Id = model.Id;
            UserId = model.UserId;
            RoleId = model.RoleId;
            PositionExecutorId = model.PositionExecutorId;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int RoleId { get; set; }

        //public DateTime StartDate { get; set; }

        //public DateTime? EndDate { get; set; }
        
        /// <summary>
        /// Должность, от которой унаследована роль
        /// </summary>
        //public int? PositionId { get; set; }

        /// <summary>
        /// Назначение, от которой унаследована роль
        /// </summary>
        public int? PositionExecutorId { get; set; }

    }
}