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
    public class SetJournalAccessCommand : BaseJournalAccessCommand
    {
        private SetJournalAccess Model { get { return GetModel<SetJournalAccess>(); } }

        public override object Execute()
        {
            var row = new InternalRegistrationJournalPosition()
            {
                PositionId = Model.PositionId,
                RegistrationJournalId = Model.RegistrationJournalId,
                RegJournalAccessTypeId = (int)Model.RegJournalAccessTypeId
            };

            CommonDocumentUtilities.SetLastChange(_context, row);

            var filter = new FilterAdminRegistrationJournalPosition()
            {
                PositionIDs = new List<int>() { row.PositionId },
                RegistrationJournalIDs = new List<int>() { row.RegistrationJournalId },
                RegistrationJournalAccessTypeIDs = new List<EnumRegistrationJournalAccessTypes>() { (EnumRegistrationJournalAccessTypes)row.RegJournalAccessTypeId }
            };

            var exists = _adminDb.ExistsRegistrationJournalPosition(_context, filter);

            if (exists && !Model.IsChecked) _adminDb.DeleteRegistrationJournalPositions(_context, filter);
            else if (!exists && Model.IsChecked) _adminDb.AddRegistrationJournalPosition(_context, row);

            return null;
        }
    }
}