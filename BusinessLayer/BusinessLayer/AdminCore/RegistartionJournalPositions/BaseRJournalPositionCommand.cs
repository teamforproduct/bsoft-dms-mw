using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace BL.Logic.AdminCore
{
    public class BaseRJournalPositionCommand : BaseAdminCommand
    {
        public override bool CanBeDisplayed(int Id) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);
            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }

        protected void SetRegistrationJournalPosition(ModifyAdminRegistrationJournalPosition model)
        {
            _adminService.ExecuteAction(EnumAdminActions.SetRegistrationJournalPosition, _context, model);
        }

        public void SetAll(int positionId, bool IsChecked, bool IsViewing, bool IsRegistration)
        {
            // Добавляю рассылку во все направления для сведения и исполнения для указанной должности
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    if (IsChecked)
                    {
                        // все журналы к которым есть доступ у positionId
                        var existsJournals = _adminDb.GetInternalRegistrationJournalPositions(_context, new FilterAdminRegistrationJournalPosition() { PositionIDs = new List<int> { positionId } });

                        // все журналы
                        var allJournals = _dictDb.GetRegistrationJournals(_context, new FilterDictionaryRegistrationJournal()
                        { IsActive = true });

                        var newJournals = new List<InternalRegistrationJournalPosition>();

                        if (IsViewing)
                        {
                            // разрешаю просматривать документы 
                            newJournals.AddRange(allJournals.Select(x => new InternalRegistrationJournalPosition()
                            {
                                PositionId = positionId,
                                RegistrationJournalId = x.Id,
                                RegJournalAccessTypeId = (int)EnumRegistrationJournalAccessTypes.View
                            }));
                        }

                        if (IsRegistration)
                        {
                            // разрешаю выполнять рыссылку от указанной должности для сведения
                            newJournals.AddRange(allJournals.Select(x => new InternalRegistrationJournalPosition()
                            {
                                PositionId = positionId,
                                RegistrationJournalId = x.Id,
                                RegJournalAccessTypeId = (int)EnumRegistrationJournalAccessTypes.Registration
                            }));
                        }

                        // нахожу в subordinations существующие записи и исключаю их
                        foreach (var item in existsJournals)
                        {
                            var i = newJournals.Where(x =>
                            x.PositionId == item.PositionId &
                            x.RegistrationJournalId == item.RegistrationJournalId &
                            x.RegJournalAccessTypeId == (int)item.RegJournalAccessTypeId
                            ).FirstOrDefault();

                            newJournals.Remove(i);
                        }

                        CommonDocumentUtilities.SetLastChange(_context, newJournals);

                        _adminDb.AddRegistrationJournalPositions(_context, newJournals);
                    }
                    else
                    {
                        var f = new FilterAdminRegistrationJournalPosition();

                        f.PositionIDs = new List<int> { positionId };

                        if (IsViewing & !IsRegistration)
                        {
                            f.RegistrationJournalAccessTypeIDs = new List<EnumRegistrationJournalAccessTypes> { EnumRegistrationJournalAccessTypes.View };
                        }
                        else if (IsRegistration & !IsViewing)
                        {
                            f.RegistrationJournalAccessTypeIDs = new List<EnumRegistrationJournalAccessTypes> { EnumRegistrationJournalAccessTypes.Registration };
                        }

                        _adminDb.DeleteRegistrationJournalPositions(_context, f);

                    }

                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }
        }

        public void SetDefault(int positionId, bool IsViewing, bool IsRegistration)
        {
            // Добавляю рассылку во все направления для сведения и исполнения для указанной должности
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    // все журналы к которым есть доступ у positionId
                    var existsJournals = _adminDb.GetInternalRegistrationJournalPositions(_context, new FilterAdminRegistrationJournalPosition() { PositionIDs = new List<int> { positionId } });

                    // все журналы отдела, к кот относится должность
                    var allJournals = _dictDb.GetRegistrationJournals(_context, new FilterDictionaryRegistrationJournal()
                    { IsActive = true, DepartmentByPositionIDs = new List<int> { positionId } });

                    var newJournals = new List<InternalRegistrationJournalPosition>();

                    if (IsViewing)
                    {
                        // разрешаю просматривать документы 
                        newJournals.AddRange(allJournals.Select(x => new InternalRegistrationJournalPosition()
                        {
                            PositionId = positionId,
                            RegistrationJournalId = x.Id,
                            RegJournalAccessTypeId = (int)EnumRegistrationJournalAccessTypes.View
                        }));
                    }

                    if (IsRegistration)
                    {
                        // разрешаю выполнять рыссылку от указанной должности для сведения
                        newJournals.AddRange(allJournals.Select(x => new InternalRegistrationJournalPosition()
                        {
                            PositionId = positionId,
                            RegistrationJournalId = x.Id,
                            RegJournalAccessTypeId = (int)EnumRegistrationJournalAccessTypes.Registration
                        }));
                    }

                    // нахожу в subordinations существующие записи и исключаю их
                    foreach (var item in existsJournals)
                    {
                        var i = newJournals.Where(x =>
                        x.PositionId == item.PositionId &
                        x.RegistrationJournalId == item.RegistrationJournalId &
                        x.RegJournalAccessTypeId == (int)item.RegJournalAccessTypeId
                        ).FirstOrDefault();

                        newJournals.Remove(i);
                    }

                    CommonDocumentUtilities.SetLastChange(_context, newJournals);

                    _adminDb.AddRegistrationJournalPositions(_context, newJournals);

                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }
        }

    }
}