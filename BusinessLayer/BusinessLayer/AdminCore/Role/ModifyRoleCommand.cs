using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using BL.Model.AdminCore.FilterModel;
using BL.Model.SystemCore;
using System.Collections.Generic;


namespace BL.Logic.AdminCore
{
    public class ModifyRoleCommand : BaseRoleCommand
    {
        private ModifyAdminRole Model { get { return GetModel<ModifyAdminRole>(); } }

        public override object Execute()
        {
            try
            {
                var model = new InternalAdminRole(Model);
                CommonDocumentUtilities.SetLastChange(_context, model);
                _adminDb.UpdateRole(_context, model);
            }
            catch (AdminRecordWasNotFound)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseError(ex);
            }
            return null;
        }
    }
}