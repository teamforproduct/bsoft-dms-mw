using BL.CrossCutting.Helpers;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryPositionExecutorCommand : BaseDictionaryPositionExecutorCommand
    {
        private readonly IDocumentsDbProcess _docDb;

        private ModifyPositionExecutor Model { get { return GetModel<ModifyPositionExecutor>(); } }

        public ModifyDictionaryPositionExecutorCommand(IDocumentsDbProcess documentDb)
        {
            _docDb = documentDb;
        }

        public override object Execute()
        {
            try
            {
                var model = new InternalDictionaryPositionExecutor(Model);

                CommonDocumentUtilities.SetLastChange(_context, model);

                using (var transaction = Transactions.GetTransaction())
                {

                    _dictDb.UpdateExecutor(_context, model);
                    var frontObj = _dictDb.GetPositionExecutors(_context, new FilterDictionaryPositionExecutor { IDs = new List<int> { model.Id } }).FirstOrDefault();
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryPositionExecutors, (int)CommandType, frontObj.AssignmentId, frontObj);

                    // Синхронизация параметров в UserRoles:
                    transaction.Complete();
                }

                return Model.Id;
            }
            catch (DictionaryRecordWasNotFound)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseError(ex);
            }
        }
    }
}