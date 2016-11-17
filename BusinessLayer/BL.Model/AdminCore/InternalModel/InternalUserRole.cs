using BL.Model.Common;
using BL.Model.AdminCore.IncomingModel;
using System;
using BL.Model.Enums;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Model.AdminCore.InternalModel
{
    public class InternalAdminUserRole : LastChangeInfo
    {
        public InternalAdminUserRole()
        { }

        public InternalAdminUserRole(ModifyAdminUserRole model)
        {
            Id = model.Id;
            RoleId = model.RoleId;
            PositionExecutorId = model.PositionExecutorId;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public int RoleId { get; set; }
        
        /// <summary>
        /// Назначение, от которой унаследована роль
        /// </summary>
        public int PositionExecutorId { get; set; }





        #region [+] Выноска параметров ...
        public int AgentId { get; set; }

        /// <summary>
        /// Должность, от которой унаследована роль
        /// </summary>
        public int PositionId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        #endregion

    }
}