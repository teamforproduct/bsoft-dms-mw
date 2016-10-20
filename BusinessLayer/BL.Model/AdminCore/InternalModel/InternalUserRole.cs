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
            PositionId = model.PositionId;
            StartDate = model.StartDate;
            EndDate = model.EndDate;
            ObjectId = model.ObjectId;
            EntityId = model.EntityId;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        public int UserId { get; set; }

        public int RoleId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// Тип объекта. Например: Конкретному сотруднику на конр должности разрешены действия (роль) над конкретным объектом.
        /// </summary>
        public EnumObjects ObjectId { get; set; }

        /// <summary>
        /// Id сущности. Например отдел...
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// Должность, от которой унаследована роль
        /// </summary>
        public int? PositionId { get; set; }

    }
}