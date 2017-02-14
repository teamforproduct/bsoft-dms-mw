using BL.CrossCutting.Helpers;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace BL.Logic.DictionaryCore
{
    public class DeleteUserPositionExecutorCommand : BaseDictionaryCommand
    {
        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }


        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            var delExecution = _dictDb.GetInternalPositionExecutors(_context, new FilterDictionaryPositionExecutor { IDs = new List<int> { Model } }).FirstOrDefault();

            if (delExecution == null) throw new DictionaryRecordWasNotFound();

            // Можно удалять только своих ио или референтов
            if (delExecution.PositionExecutorTypeId == (int)EnumPositionExecutionTypes.Personal) throw new DictionaryRecordCouldNotBeDeleted();

            // Вычисляю кто назначен на должность из delExecution
            var personalExecution = _dictDb.GetInternalPositionExecutors(_context, new FilterDictionaryPositionExecutor
            {
                PositionIDs = new List<int> { delExecution.PositionId },
                PositionExecutorTypeIDs = new List<EnumPositionExecutionTypes> { EnumPositionExecutionTypes.Personal },
                IsActive = true,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow,
            }).FirstOrDefault();

            if (personalExecution == null) throw new DictionaryRecordCouldNotBeDeleted();

            // Если на должность штатно назначен не текущий пользователь
            if (personalExecution.AgentId != _context.CurrentAgentId) throw new DictionaryRecordCouldNotBeDeleted();

            return true;
        }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var frontObj = _dictDb.GetPositionExecutors(_context, new FilterDictionaryPositionExecutor { IDs = new List<int> { Model } }).FirstOrDefault();
                _logger.Information(_context, null, (int)EnumObjects.DictionaryPositionExecutors, (int)CommandType, frontObj.Id, frontObj);

                _adminDb.DeleteUserRoles(_context, new BL.Model.AdminCore.FilterModel.FilterAdminUserRole() { PositionExecutorIDs = new List<int> { Model } });
                _dictDb.DeleteExecutors(_context, new System.Collections.Generic.List<int> { Model });

                transaction.Complete();
            }
            return null;
        }
    }

}

