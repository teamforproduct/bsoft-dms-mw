using BL.CrossCutting.Helpers;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class DuplicateJournalAccess_JournalCommand : BaseJournalAccessCommand
    {
        private DuplicateJournalAccess Model { get { return GetModel<DuplicateJournalAccess>(); } }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {
                if (Model.CopyMode == EnumCopyMode.Сoverage)
                {
                    // ощищаю настроки для Model.TargetJournalId
                    _adminDb.DeleteRegistrationJournalPositions(_context, new FilterAdminRegistrationJournalPosition() { RegistrationJournalIDs = new List<int> { Model.TargetJournalId } });
                }

                // выгребаю все настройки для Model.SourceJournalId
                var items = _adminDb.GetInternalRegistrationJournalPositions(_context, new FilterAdminRegistrationJournalPosition() { RegistrationJournalIDs = new List<int> { Model.SourceJournalId } });

                // добавляю 
                foreach (var item in items)
                {
                    // подменил SourceJournal
                    var model = new SetJournalAccess()
                    {
                        PositionId = item.PositionId,
                        RegistrationJournalId = Model.TargetJournalId,
                        RegJournalAccessTypeId = (EnumRegistrationJournalAccessTypes)item.RegJournalAccessTypeId,
                        IsChecked = true,
                    };

                    SetJournalAccess(model);
                }

                transaction.Complete();
            }

            return null;

        }
    }
}