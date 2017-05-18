﻿using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;


namespace BL.Logic.DictionaryCore
{
    public class BaseDictionaryRegistrationJournalCommand : BaseDictionaryCommand
    {
        private AddRegistrationJournal Model { get { return GetModel<AddRegistrationJournal>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            Model.Name?.Trim();

            // Тонкий момент, проверяю не является ли сотрудник локальным администратором.
            // Если не локальный значит, надеюсь, что глобальный и разрешаю создавать и изменять параметры всех должностей
            var employeeDepartments = _adminService.GetInternalEmployeeDepartments(_context, _context.Employee.Id);

            if (employeeDepartments != null)
            {
                if (!employeeDepartments.Contains(Model.DepartmentId)) throw new AccessIsDenied();
            }

            var filter = new FilterDictionaryRegistrationJournal
            {
                NameExact = Model.Name,
                IndexExact = Model.Index,
                DepartmentIDs = new List<int> { Model.DepartmentId },
            };

            if (TypeModelIs<ModifyRegistrationJournal>())
            { filter.NotContainsIDs = new List<int> { GetModel<ModifyRegistrationJournal>().Id }; }

            // Находим запись с таким-же именем и индексом журнала в этом-же подразделении
            if (_dictDb.ExistsDictionaryRegistrationJournal(_context, filter))
            {
                throw new DictionaryRegistrationJournalNotUnique(Model.Name);
            }

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}