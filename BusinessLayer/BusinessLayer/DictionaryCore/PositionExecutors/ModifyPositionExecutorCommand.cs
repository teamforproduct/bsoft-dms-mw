using BL.CrossCutting.Helpers;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class ModifyPositionExecutorCommand : BasePositionExecutorCommand
    {
        private readonly IDocumentsDbProcess _docDb;

        private ModifyPositionExecutor Model { get { return GetModel<ModifyPositionExecutor>(); } }

        public ModifyPositionExecutorCommand(IDocumentsDbProcess documentDb)
        {
            _docDb = documentDb;
        }

        public override object Execute()
        {
            var model = new InternalDictionaryPositionExecutor(Model);

            CommonDocumentUtilities.SetLastChange(_context, model);

            using (var transaction = Transactions.GetTransaction())
            {

                _dictDb.UpdateExecutor(_context, model);
                var frontObj = _dictDb.GetPositionExecutors(_context, new FilterDictionaryPositionExecutor { IDs = new List<int> { model.Id } }).FirstOrDefault();
                _logger.Information(_context, null, (int)EnumObjects.DictionaryPositionExecutors, (int)CommandType, frontObj.Id, frontObj);

                // Синхронизация параметров в UserRoles:
                transaction.Complete();
            }

            return Model.Id;
        }
    }
}