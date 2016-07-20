using BL.Model.Common;
using BL.Model.AdminCore.IncomingModel;
using System;

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
            StartDate = model.StartDate;
            EndDate = model.EndDate;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        public int UserId { get; set; }

        public int RoleId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

    }
}