﻿using BL.CrossCutting.DependencyInjection;
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
        private AddPosition Model { get { return GetModel<AddPosition>(); } }

        public override object Execute()
        {
            try
            {
                var model = new InternalDictionaryPosition(Model);

                CommonDocumentUtilities.SetLastChange(_context, model);

                using (var transaction = Transactions.GetTransaction())
                {
                    // добавляю должность
                    model.Id = _dictDb.AddPosition(_context, model);

                    // устанавливаю порядок отображения
                    _dictService.SetPositionOrder(_context, new ModifyPositionOrder { PositionId = model.Id, Order = Model.Order });

                    // включаю доступ к журналам в своем отделе
                    SetDefaultRJournalPositions(new ModifyAdminDefaultByPosition { PositionId = model.Id });

                    // всегда устанавливаю рассылку по умолчанию 
                    SetDefaultSubordinations(new ModifyAdminDefaultByPosition { PositionId = model.Id });

                    // рассылка для исполнения на всех
                    if (GetSubordinationsSendAllForExecution())
                    { SetAllSubordinations(new ModifyAdminSubordinations { IsChecked = true, PositionId = model.Id, SubordinationTypeId = EnumSubordinationTypes.Execution }); }

                    // рассылка для сведения на всех
                    if (GetSubordinationsSendAllForInforming())
                    { SetAllSubordinations(new ModifyAdminSubordinations { IsChecked = true, PositionId = model.Id, SubordinationTypeId = EnumSubordinationTypes.Informing }); }

                    var frontObj = _dictDb.GetPositions(_context, new FilterDictionaryPosition { IDs = new List<int> { model.Id } }).FirstOrDefault();
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryPositions, (int)CommandType, frontObj.Id, frontObj);


                    transaction.Complete();
                }

                return model.Id;
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
            return tmpService.GetSubordinationsSendAllForExecution(_context);
        }

        private bool GetSubordinationsSendAllForInforming()
        {
            var tmpService = DmsResolver.Current.Get<ISettings>();
            return tmpService.GetSubordinationsSendAllForInforming(_context);
        }
    }
}