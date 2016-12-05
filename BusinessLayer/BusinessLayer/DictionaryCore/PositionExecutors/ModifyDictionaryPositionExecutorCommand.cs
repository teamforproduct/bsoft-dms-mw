using BL.CrossCutting.Helpers;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
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

        public ModifyDictionaryPositionExecutorCommand(IDocumentsDbProcess documentDb)
        {
            _docDb = documentDb;
        }

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            base.PrepareData();

            // Проверка дат
            var oldPositionExecutor = _dictDb.GetInternalPositionExecutors(_context, new FilterDictionaryPositionExecutor { IDs = new List<int> { Model.Id } }).FirstOrDefault();

            if (oldPositionExecutor == null) throw new DictionaryRecordWasNotFound();

            // Если интервал уменьшили
            if (Model.StartDate > oldPositionExecutor.StartDate || (Model.EndDate ?? DateTime.MaxValue) < (oldPositionExecutor.EndDate ?? DateTime.MaxValue))
            {

                // Вычисляю интервал меньше которого нельзя сдвигать даты.
                IEnumerable<InternalDocumentEvent> events = new List<InternalDocumentEvent>();

                events = _docDb.GetEventsNatively(_context, new FilterDocumentEventNatively
                {
                    SourcePositionIDs = new List<int> { oldPositionExecutor.PositionId },
                    SourcePositionExecutorAgentIDs = new List<int> { oldPositionExecutor.AgentId },
                    Date = new BL.Model.Common.Period(oldPositionExecutor.StartDate, oldPositionExecutor.EndDate ?? DateTime.MaxValue)
                });

                // Нельзя изменять период назначения так, чтобы он не включал даты события должности в рамках этого назначения 
                if (events.Count() > 0)
                {
                    DateTime maxD = events.Max(x => x.Date);
                    DateTime minD = events.Min(x => x.Date);

                    if (maxD > Model.EndDate || minD < Model.StartDate) throw new DictionaryPositionExecutorEventExists();
                }

                // Нельзя изменять период назначения так, чтобы он не включал даты событий от других сотрудников по отношению к текущему сотруднику
                events = _docDb.GetEventsNatively(_context, new FilterDocumentEventNatively
                {
                    TargetPositionIDs = new List<int> { oldPositionExecutor.PositionId },
                    TargetPositionExecutorAgentIDs = new List<int> { oldPositionExecutor.AgentId },
                    Date = new BL.Model.Common.Period(oldPositionExecutor.StartDate, oldPositionExecutor.EndDate ?? DateTime.MaxValue)
                });

                if (events.Count() > 0)
                {
                    DateTime maxD = events.Max(x => x.Date);
                    DateTime minD = events.Min(x => x.Date);

                    if (maxD > Model.EndDate || minD < Model.StartDate) throw new DictionaryPositionExecutorEventExists();
                }

                // Нельзя изменять период назначения так, чтобы он не включал даты прочтения сообщений
                events = _docDb.GetEventsNatively(_context, new FilterDocumentEventNatively
                {
                    TargetPositionIDs = new List<int> { oldPositionExecutor.PositionId },
                    ReadAgentIDs = new List<int> { oldPositionExecutor.AgentId },
                    ReadDate = new BL.Model.Common.Period(oldPositionExecutor.StartDate, oldPositionExecutor.EndDate ?? DateTime.MaxValue)
                });

                if (events.Count() > 0)
                {
                    DateTime maxD = events.Max(x => x.Date);
                    DateTime minD = events.Min(x => x.Date);

                    if (maxD > Model.EndDate || minD < Model.StartDate) throw new DictionaryPositionExecutorEventExists();
                }

            }
            return base.CanExecute();
        }

        public override object Execute()
        {
            try
            {
                var dp = CommonDictionaryUtilities.PositionExecutorModifyToInternal(_context, Model);

                using (var transaction = Transactions.GetTransaction())
                {

                    _dictDb.UpdateExecutor(_context, dp);
                    var frontObj = _dictDb.GetPositionExecutors(_context, new FilterDictionaryPositionExecutor { IDs = new List<int> { dp.Id } }).FirstOrDefault();
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryPositionExecutors, (int)CommandType, frontObj.Id, frontObj);

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