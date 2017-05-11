using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryDepartmentCommand : BaseDictionaryCommand
    {

        private int Model { get { return GetModel<int>(); } }

        public override bool CanBeDisplayed(int positionId) => true;


        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);
            return true;
        }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var frontObj = _dictDb.GetDepartments(_context, new FilterDictionaryDepartment { IDs = new List<int> { Model } }).FirstOrDefault();
                if (frontObj != null) _logger.Information(_context, null, (int)EnumObjects.DictionaryDepartments, (int)CommandType, frontObj.Id, frontObj);

                // Удаляю отдел и дочерние отделы с должностями и всем барахлом
                _dictService.DeleteDepartments(_context, new List<int>() { Model });
                transaction.Complete();
            }
            return null;
        }
    }

}

