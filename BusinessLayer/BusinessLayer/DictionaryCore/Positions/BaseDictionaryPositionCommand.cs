﻿using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Exception;
using System;

namespace BL.Logic.DictionaryCore
{
    public class BaseDictionaryPositionCommand : BaseDictionaryCommand
    {
        private AddPosition Model { get { return GetModel<AddPosition>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            if (Model.Order < 1) Model.Order = int.MaxValue;

            //var fdd = new FilterDictionaryPosition { Name = Model.Name, NotContainsIDs = new List<int> { Model.Id } };

            //if (Model.ParentId != null)
            //{
            //    fdd.ParentIDs = new List<int> { Model.ParentId.Value };
            //}

            //// Находим запись с таким-же именем в этой-же папке
            //if (_dictDb.ExistsPosition(_context, fdd))
            //{
            //    throw new DictionaryRecordNotUnique();
            //}

            // Тонкий момент, проверяю не является ли сотрудник локальным администратором.
            // Если не локальный значит, надеюсь, что глобальный и разрешаю создавать и изменять параметры всех должностей
            var employeeDepartments = _adminService.GetInternalEmployeeDepartments(_context, _context.Employee.Id);

            if (employeeDepartments != null)
            {
                if (!employeeDepartments.Contains(Model.DepartmentId)) throw new AccessIsDenied();
            }

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}