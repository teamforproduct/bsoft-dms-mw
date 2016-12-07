using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore;
using BL.Logic.Common;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryPositionCommand : BaseDictionaryPositionCommand
    {

        public override object Execute()
        {
            try
            {
                var dp = CommonDictionaryUtilities.PositionModifyToInternal(_context, Model);
                int positionId;
                using (var transaction = Transactions.GetTransaction())
                {
                    // добавляю должность
                    positionId = _dictDb.AddPosition(_context, dp);

                    // устанавливаю порядок отображения
                    _dictService.SetPositionOrder(_context, positionId, Model.Order);

                    // включаю доступ к журналам в своем отделе
                    SetDefaultRJournalPositions(new ModifyAdminDefaultByPosition { PositionId = positionId });

                    // всегда устанавливаю рассылку по умолчанию 
                    SetDefaultSubordinations(new ModifyAdminDefaultByPosition { PositionId = positionId });

                    // рассылка для исполнения на всех
                    if (GetSubordinationsSendAllForExecution())
                    { SetAllSubordinations( new ModifyAdminSubordinations {  IsChecked = true, PositionId = positionId, SubordinationTypeId = EnumSubordinationTypes.Execution }); }

                    // рассылка для сведения на всех
                    if (GetSubordinationsSendAllForInforming())
                    { SetAllSubordinations(new ModifyAdminSubordinations { IsChecked = true, PositionId = positionId, SubordinationTypeId = EnumSubordinationTypes.Informing }); }

                    var frontObj = _dictDb.GetPositions(_context, new FilterDictionaryPosition { IDs = new List<int> { dp.Id } }).FirstOrDefault();
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryPositions, (int)CommandType, frontObj.Id, frontObj);


                    transaction.Complete();
                }

                return positionId;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }

        private void SetDefaultRJournalPositions(ModifyAdminDefaultByPosition model)
        {
            _adminService.ExecuteAction(EnumAdminActions.SetDefaultRegistrationJournalPosition, _context, model);
        }

        private void SetDefaultSubordinations(ModifyAdminDefaultByPosition model)
        {
            _adminService.ExecuteAction(EnumAdminActions.SetDefaultSubordination, _context, model);
        }

        private void SetAllSubordinations(ModifyAdminSubordinations model)
        {
            _adminService.ExecuteAction(EnumAdminActions.SetAllSubordination, _context, model);
        }

        private bool GetSubordinationsSendAllForExecution()
        {
            var tmpService = DmsResolver.Current.Get<ISettings>();
            return  tmpService.GetSubordinationsSendAllForExecution(_context);
        }

        private bool GetSubordinationsSendAllForInforming()
        {
            var tmpService = DmsResolver.Current.Get<ISettings>();
            return tmpService.GetSubordinationsSendAllForInforming(_context);
        }
    }
}