using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BL.Logic.DictionaryCore
{
    public class BaseDictionaryStandartSendListCommand :BaseDictionaryCommand
    {
        private AddStandartSendList Model { get { return GetModel<AddStandartSendList>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            Model.Name?.Trim();

            // Тонкий момент, проверяю не является ли сотрудник локальным администратором.
            // Если не локальный значит, надеюсь, что глобальный и разрешаю 
            var employeeDepartments = _adminService.GetInternalEmployeeDepartments(_context, _context.Employee.Id);

            if (employeeDepartments != null)
            {
                var position = _dictDb.GetInternalPositions(_context, new FilterDictionaryPosition { IDs = new List<int> { Model.PositionId } }).FirstOrDefault();

                if (position != null)
                {
                    if (!employeeDepartments.Contains(position.DepartmentId)) throw new AccessIsDenied();
                }

            }


            var filter = new FilterDictionaryStandartSendList()
            {
                NameExact = Model.Name,
                PositionIDs = new List<int> { Model.PositionId },
            };

            if (TypeModelIs<ModifyStandartSendList>())
            { filter.NotContainsIDs = new List<int> { GetModel<ModifyStandartSendList>().Id }; }

            var contents = _dictDb.GetStandartSendLists(_context, filter, null);

            if (contents.Any())
            {
                throw new DictionaryStandartSendListNotUnique(Model.Name);
            }

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}
