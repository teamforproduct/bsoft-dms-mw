using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryPositionCommand : BaseDictionaryCommand
    {

        private int Model { get { return GetModel<int>(); } }

        public override bool CanBeDisplayed(int positionId) => true;


        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            // PSS Добавить проверку. Существуют ли события от этой должности

            return true;
        }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var frontObj = _dictDb.GetPositions(_context, new FilterDictionaryPosition { IDs = new List<int> { Model } }).FirstOrDefault();
                if (frontObj != null) _logger.Information(_context, null, (int)EnumObjects.DictionaryPositions, (int)CommandType, frontObj.Id, frontObj);

                _dictService.DeletePositions(_context, new List<int> { Model });
                transaction.Complete();
            }
            return null;
        }
    }

}

