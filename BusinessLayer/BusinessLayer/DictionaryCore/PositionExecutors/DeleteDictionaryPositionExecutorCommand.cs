﻿using BL.CrossCutting.Helpers;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryPositionExecutorCommand : BaseDictionaryCommand
    {
        private readonly IDocumentsDbProcess _docDb;

        public DeleteDictionaryPositionExecutorCommand(IDocumentsDbProcess documentDb)
        {
            _docDb = documentDb;
        }

        private int Model { get { return GetModel<int>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            var positionExecutor = _dictDb.GetInternalPositionExecutors(_context, new FilterDictionaryPositionExecutor { IDs = new List<int> { Model } }).FirstOrDefault();

            if (positionExecutor == null) throw new DictionaryRecordWasNotFound();

            // Нельзя удалять назначение если сотрудник на должности в рамках этого назначения породил события
            if (_docDb.ExistsEventsNatively(_context, new FilterDocumentEventNatively
            {
                SourcePositionIDs = new List<int> { positionExecutor.PositionId },
                SourcePositionExecutorAgentIDs = new List<int> { positionExecutor.AgentId },
                Date = new BL.Model.Common.Period(positionExecutor.StartDate, positionExecutor.EndDate ?? DateTime.MaxValue)
            })) throw new DictionaryRecordCouldNotBeDeleted();

            // Нельзя удалять назначение если сотрудник в рамках этого назначения породил события
            if (_docDb.ExistsEventsNatively(_context, new FilterDocumentEventNatively
            {
                SourcePositionIDs = new List<int> { positionExecutor.PositionId },
                SourceAgentIDs = new List<int> { positionExecutor.AgentId },
                Date = new BL.Model.Common.Period(positionExecutor.StartDate, positionExecutor.EndDate ?? DateTime.MaxValue)
            })) throw new DictionaryRecordCouldNotBeDeleted();

            // Нельзя удалять назначение если для сотрудника на должности в рамках этого назначения породили события другие сотрудники
            if (_docDb.ExistsEventsNatively(_context, new FilterDocumentEventNatively
            {
                TargetPositionIDs = new List<int> { positionExecutor.PositionId },
                TargetPositionExecutorAgentIDs = new List<int> { positionExecutor.AgentId },
                Date = new BL.Model.Common.Period(positionExecutor.StartDate, positionExecutor.EndDate ?? DateTime.MaxValue)
            })) throw new DictionaryRecordCouldNotBeDeleted();

            // Нельзя удалять назначение если сотрудник в рамках этого назначения прочел сообщения должности
            if (_docDb.ExistsEventsNatively(_context, new FilterDocumentEventNatively
            {
                TargetPositionIDs = new List<int> { positionExecutor.PositionId },
                ReadAgentIDs = new List<int> { positionExecutor.AgentId },
                ReadDate = new BL.Model.Common.Period(positionExecutor.StartDate, positionExecutor.EndDate ?? DateTime.MaxValue)
            })) throw new DictionaryRecordCouldNotBeDeleted();


            return true;
        }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var frontObj = _dictDb.GetPositionExecutors(_context, new FilterDictionaryPositionExecutor { IDs = new List<int> { Model } }).FirstOrDefault();
                _logger.Information(_context, null, (int)EnumObjects.DictionaryPositionExecutors, (int)CommandType, frontObj.Id, frontObj);

                _adminDb.DeleteUserRoles(_context, new BL.Model.AdminCore.FilterModel.FilterAdminUserRole() { PositionExecutorIDs = new List<int> { Model } });
                _dictDb.DeleteExecutors(_context, new List<int> { Model });

                transaction.Complete();
            }
            return null;
        }
    }

}

