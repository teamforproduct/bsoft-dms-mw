using BL.CrossCutting.Helpers;
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

namespace BL.Logic.AdminCore
{
    public class BaseJournalAccessCommand : BaseAdminCommand
    {
        public override bool CanBeDisplayed(int Id) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);
            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }

        protected void SetJournalAccess(SetJournalAccess model)
        {
            _adminService.ExecuteAction(EnumAdminActions.SetJournalAccess, _context, model);
        }

        public void SetAll_Position(int positionId, bool IsChecked, bool IsViewing, bool IsRegistration)
        {
            // Добавляю рассылку во все направления для сведения и исполнения для указанной должности
            try
            {
                using (var transaction = Transactions.GetTransaction())
                {
                    if (IsChecked)
                    {
                        // все журналы к которым есть доступ у positionId
                        var existsJournals = _adminDb.GetInternalRegistrationJournalPositions(_context, new FilterAdminRegistrationJournalPosition() { PositionIDs = new List<int> { positionId } });

                        // все журналы
                        var allJournals = _dictDb.GetRegistrationJournals(_context, new FilterDictionaryRegistrationJournal()
                        { IsActive = true }, null);

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

        public void SetAll_Journal(int journalId, bool isChecked, bool isViewing, bool isRegistration)
        {
            // Добавляю рассылку во все направления для сведения и исполнения для указанной должности
            try
            {
                using (var transaction = Transactions.GetTransaction())
                {
                    if (isChecked)
                    {
                        // все должности у которых есть доступ
                        var existsJournals = _adminDb.GetInternalRegistrationJournalPositions(_context, new FilterAdminRegistrationJournalPosition() { RegistrationJournalIDs = new List<int> { journalId } });

                        // все журналы
                        var allPositions = _dictDb.GetInternalPositions(_context, new FilterDictionaryPosition()
                        { IsActive = true });

                        var newJournals = new List<InternalRegistrationJournalPosition>();

                        if (isViewing)
                        {
                            // разрешаю просматривать документы 
                            newJournals.AddRange(allPositions.Select(x => new InternalRegistrationJournalPosition()
                            {
                                PositionId = x.Id,
                                RegistrationJournalId = journalId,
                                RegJournalAccessTypeId = (int)EnumRegistrationJournalAccessTypes.View
                            }));
                        }

                        if (isRegistration)
                        {
                            // разрешаю выполнять рыссылку от указанной должности для сведения
                            newJournals.AddRange(allPositions.Select(x => new InternalRegistrationJournalPosition()
                            {
                                PositionId = x.Id,
                                RegistrationJournalId = journalId,
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

                        f.RegistrationJournalIDs = new List<int> { journalId };

                        if (isViewing & !isRegistration)
                        {
                            f.RegistrationJournalAccessTypeIDs = new List<EnumRegistrationJournalAccessTypes> { EnumRegistrationJournalAccessTypes.View };
                        }
                        else if (isRegistration & !isViewing)
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

        public void SetByDepartment_Position(int departmentId, int positionId, bool isChecked, EnumRegistrationJournalAccessTypes type)
        {
            var journals = _dictDb.GetRegistrationJournals(_context, new FilterDictionaryRegistrationJournal() { DepartmentIDs = new List<int> { departmentId }, IsActive = true }, null);

            if (journals.Count() > 0)
            {
                foreach (var journal in journals)
                {
                    SetJournalAccess(new SetJournalAccess()
                    {
                        IsChecked = isChecked,
                        PositionId = positionId,
                        RegistrationJournalId = journal.Id,
                        RegJournalAccessTypeId = type
                    });
                }
            }
        }


        public void SetByDepartment_Journal(int departmentId, int journalId, bool isChecked, EnumRegistrationJournalAccessTypes type)
        {
            var positions = _dictDb.GetInternalPositions(_context, new FilterDictionaryPosition() { DepartmentIDs = new List<int> { departmentId }, IsActive = true });

            if (positions.Count() > 0)
            {
                foreach (var position in positions)
                {
                    SetJournalAccess(new SetJournalAccess()
                    {
                        IsChecked = isChecked,
                        PositionId = position.Id,
                        RegistrationJournalId = journalId,
                        RegJournalAccessTypeId = type
                    });
                }
            }
        }

        public void SetForChildDepartments_Journal(int departmentId, int journalId, bool isChecked, EnumRegistrationJournalAccessTypes type)
        {
            var departments = _dictDb.GetDepartments(_context, new FilterDictionaryDepartment() { ParentIDs = new List<int> { departmentId } });

            if (departments.Count() > 0)
            {
                foreach (var department in departments)
                {
                    SetByDepartment_Journal(department.Id, journalId, isChecked, type);
                    SetForChildDepartments_Journal(department.Id, journalId, isChecked, type);
                }
            }
        }

        public void SetForChildDepartments_Position(int departmentId, int positionId, bool isChecked, EnumRegistrationJournalAccessTypes type)
        {
            var departments = _dictDb.GetDepartments(_context, new FilterDictionaryDepartment() { ParentIDs = new List<int> { departmentId } });

            if (departments.Count() > 0)
            {
                foreach (var department in departments)
                {
                    SetByDepartment_Position(department.Id, positionId , isChecked, type);
                    SetForChildDepartments_Position(department.Id, positionId, isChecked, type);
                }
            }
        }

    }
}