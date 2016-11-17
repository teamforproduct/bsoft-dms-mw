using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace BL.Logic.AdminCore
{
    public class DuplicateRJournalPositionCommand : BaseRJournalPositionCommand
    {
        private CopyAdminSettingsByPosition Model
        {
            get
            {
                if (!(_param is CopyAdminSettingsByPosition)) throw new WrongParameterTypeError();
                return (CopyAdminSettingsByPosition)_param;
            }
        }

        public override object Execute()
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
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
                        var model = new ModifyAdminRegistrationJournalPosition()
                        {
                            PositionId = Model.TargetPositionId,
                            RegistrationJournalId = item.RegistrationJournalId,
                            RegJournalAccessTypeId = (EnumRegistrationJournalAccessTypes)item.RegJournalAccessTypeId,
                            IsChecked = true,
                        };

                        SetRegistrationJournalPosition(model);
                    }

                    transaction.Complete();
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }

        }
    }
}