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

        public static InternalAdminRoleAction RoleActionModifyToInternal(IContext context, ModifyAdminRoleAction modifyModel)
        {
            InternalAdminRoleAction internalModel = new InternalAdminRoleAction(modifyModel);

            CommonDocumentUtilities.SetLastChange(context, internalModel);

            return internalModel;
        }

        public static InternalAdminRole RoleModifyToInternal(IContext context, ModifyAdminRole modifyModel)
        {
            InternalAdminRole internalModel = new InternalAdminRole(modifyModel);

            CommonDocumentUtilities.SetLastChange(context, internalModel);

            return internalModel;
        }


    }
}
