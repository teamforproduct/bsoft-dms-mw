using System;
using System.Transactions;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.Common;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.CrossCutting.Helpers;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore.AgentEmployee
{
    public class ModifyAgentPeoplePassportCommand : BaseDictionaryCommand
    {
        private ModifyAgentPeoplePassport Model { get { return GetModel<ModifyAgentPeoplePassport>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            Model.PassportSerial?.Trim();

            // Обрезаю время для даты рождения и даты получения паспорта
            if (Model.PassportDate.HasValue) Model.PassportDate = Model.PassportDate?.Date;

            // Если указаны необязательные паспортные данные, проверяю нет ли таких уже
            if (!string.IsNullOrEmpty(Model.PassportSerial + Model.PassportNumber))
            {
                var filterPerson = new FilterDictionaryAgentPerson
                {
                    PassportSerialExact = Model.PassportSerial,
                    PassportNumberExact = Model.PassportNumber,
                    NotContainsIDs = new List<int> { Model.Id },
                };

                if (_dictDb.ExistsAgentPersons(_context, filterPerson))
                {
                    throw new DictionaryAgentPeoplePassportNotUnique(Model.PassportSerial, Model.PassportNumber);
                }
            }

            return true;
        }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var item = new InternalDictionaryAgentEmployee(Model);

                CommonDocumentUtilities.SetLastChange(_context, item);

                _dictDb.UpdateAgentPeoplePassport(_context, item);

                transaction.Complete();
            }
            return null;
        }
    }
}
