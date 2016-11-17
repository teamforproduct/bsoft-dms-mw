using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class SetRJournalPositionCommand : BaseRJournalPositionCommand
    {
        private ModifyAdminRegistrationJournalPosition Model
        {
            get
            {
                if (!(_param is ModifyAdminRegistrationJournalPosition)) throw new WrongParameterTypeError();
                return (ModifyAdminRegistrationJournalPosition)_param;
            }
        }

        public override object Execute()
        {
            try
            {
                var row = new InternalRegistrationJournalPosition()
                {
                    PositionId = Model.PositionId,
                    RegistrationJournalId = Model.RegistrationJournalId,
                    RegJournalAccessTypeId = (int)Model.RegJournalAccessTypeId
                };

                CommonDocumentUtilities.SetLastChange(_context, row);

                var exists = _adminDb.ExistsRegistrationJournalPosition(_context, new FilterAdminRegistrationJournalPosition()
                {
                    PositionIDs = new List<int>() { row.PositionId },
                    RegistrationJournalIDs = new List<int>() { row.RegistrationJournalId },
                    RegistrationJournalAccessTypeIDs = new List<EnumRegistrationJournalAccessTypes>() { (EnumRegistrationJournalAccessTypes)row.RegJournalAccessTypeId }
                });

                if (exists && !Model.IsChecked) _adminDb.DeleteRegistrationJournalPosition(_context, row);
                else if (!exists && Model.IsChecked) _adminDb.AddRegistrationJournalPosition(_context, row);

                return null;
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }
        }
    }
}