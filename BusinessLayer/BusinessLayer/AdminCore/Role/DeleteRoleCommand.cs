using BL.Logic.Common;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;

namespace BL.Logic.AdminCore
{
    public class DeleteRoleCommand : BaseAdminCommand
    {

        private int Model
        {
            get
            {
                if (!(_param is int)) throw new WrongParameterTypeError();
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int Id)
        {
            return true;
        }


        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            string roleCode = _adminDb.GetRoleTypeCode(_context, Model);

            if (!string.IsNullOrEmpty(roleCode))
            {
                throw new DictionarySystemRecordCouldNotBeDeleted();
            }

            return true;
        }

        public override object Execute()
        {
            try
            {
                var model = new InternalAdminRole() { Id = Model };
                _adminDb.DeleteRole(_context, model);
                return null;
            }
            catch (ArgumentNullException ex)
            {
                throw new AdminRecordWasNotFound(ex);
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeDeleted(ex);
            }
        }
    }

}

