using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
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

            var filter = new FilterDictionaryStandartSendList()
            {
                NameExact = Model.Name,
                PositionIDs = new List<int> { Model.PositionId ?? 0 },
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
