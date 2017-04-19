using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.DictionaryCore.Positions
{
    public class ModifyDictionaryPositionCommand : BaseDictionaryPositionCommand
    {
        private ModifyPosition Model { get { return GetModel<ModifyPosition>(); } }

        public override object Execute()
        {
            var model = new InternalDictionaryPosition(Model);

            CommonDocumentUtilities.SetLastChange(_context, model);

            using (var transaction = Transactions.GetTransaction())
            {
                _dictDb.UpdatePosition(_context, model);
                var frontObj = _dictDb.GetPositions(_context, new FilterDictionaryPosition { IDs = new List<int> { model.Id } }).FirstOrDefault();
                _logger.Information(_context, null, (int)EnumObjects.DictionaryPositions, (int)CommandType, frontObj.Id, frontObj);

                _dictService.SetPositionOrder(_context, new ModifyPositionOrder { PositionId = Model.Id, Order = Model.Order });

                transaction.Complete();
            }
            return null;
        }
    }
}