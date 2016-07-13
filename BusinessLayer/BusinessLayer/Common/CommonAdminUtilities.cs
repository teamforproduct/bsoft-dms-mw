using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.CrossCutting.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BL.Logic.Common
{
    /// <summary>
    /// Класс содержит общие приемы работы с админкой
    /// </summary>
    public static class CommonAdminUtilities
    {
        public static InternalAdminPositionRole PositionRoleModifyToInternal(IContext context, ModifyAdminPositionRole modifyModel)
        {
            InternalAdminPositionRole internalModel = new InternalAdminPositionRole(modifyModel);

            CommonDocumentUtilities.SetLastChange(context, internalModel);

            return internalModel;
        }
    }
}
