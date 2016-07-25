﻿using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using BL.Model.SystemCore;

namespace BL.Logic.AdminCore
{
    public class DeleteSubordinationCommand : BaseAdminCommand
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
            _admin.VerifyAccess(_context, CommandType, false);
            return true;
        }

        public override object Execute()
        {
            try
            {
                var filter = new InternalAdminSubordination() { Id = Model };
                _adminDb.DeleteSubordination(_context, filter);
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

