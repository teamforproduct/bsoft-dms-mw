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
    public class BaseDictionaryStandartSendListContentCommand :BaseDictionaryCommand
    {
        private AddStandartSendListContent Model { get { return GetModel<AddStandartSendListContent>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false, true);

            var filter = new FilterDictionaryStandartSendListContent()
            {
                StandartSendListId = new List<int> { Model.StandartSendListId },
                TargetAgentId = Model.TargetAgentId,
                TargetPositionId = Model.TargetPositionId,
                Task = Model.Task,
                SendTypeId = new List<EnumSendTypes> { Model.SendTypeId },
            };

            if (TypeModelIs<ModifyStandartSendListContent>())
            { filter.NotContainsIDs = new List<int> { GetModel<ModifyStandartSendListContent>().Id }; }

            var contents = _dictDb.GetStandartSendListContents(_context, filter);

            //StandartSendListId = new List<int> { Model.StandartSendListId },
            //    TargetAgentId = Model.TargetAgentId,
            //    TargetPositionId = Model.TargetPositionId,
            //    SendTypeId = new List<EnumSendTypes> { Model.SendTypeId }

            if (contents.Any()) throw new DictionaryStandartSendListContentNotUnique(Model.Task);

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}
