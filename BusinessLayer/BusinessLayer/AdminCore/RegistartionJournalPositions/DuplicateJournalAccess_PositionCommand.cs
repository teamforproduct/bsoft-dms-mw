using BL.CrossCutting.Helpers;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class DuplicateJournalAccess_PositionCommand : BaseJournalAccessCommand
    {
        private CopyAdminSettingsByPosition Model { get { return GetModel<CopyAdminSettingsByPosition>(); } }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {
                if (Model.CopyMode == BL.Model.Enums.EnumCopyMode.Сoverage)
                {
                    // ощищаю настроки для Model.TargetPositionId
                    _adminDb.DeleteRegistrationJournalPositions(_context, new FilterAdminRegistrationJournalPosition() { PositionIDs = new List<int> { Model.TargetPositionId } });
                }

                // выгребаю все настройки для Model.SourcePosition
                var items = _adminDb.GetInternalRegistrationJournalPositions(_context, new FilterAdminRegistrationJournalPosition() { PositionIDs = new List<int> { Model.SourcePositionId } });

                // добавляю 
                foreach (var item in items)
                {
                    // подменил SourcePosition
                    var model = new SetJournalAccess()
                    {
                        PositionId = Model.TargetPositionId,
                        RegistrationJournalId = item.RegistrationJournalId,
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