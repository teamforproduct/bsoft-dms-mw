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
    public class BaseDictionaryAgentPersonCommand : BaseDictionaryCommand
    {
        protected ModifyDictionaryAgentPerson Model
        {
            get
            {
                if (!(_param is ModifyDictionaryAgentPerson))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryAgentPerson)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false, true);

            Model.Name?.Trim();
            Model.PassportSerial?.Trim();
            Model.TaxCode?.Trim();

            // Обрезаю время для даты рождения и даты получения паспорта
            if (Model.PassportDate.HasValue) Model.PassportDate = Model.PassportDate?.Date;

            if (Model.BirthDate.HasValue) Model.BirthDate = Model.BirthDate?.Date;

            if (_dictDb.ExistsAgents(_context, new FilterDictionaryAgent
            {
                NameExact = Model.Name,
                NotContainsIDs = new List<int> { Model.Id }
            }))
            {
                throw new DictionaryAgentNameNotUnique(Model.Name);
            }

            // Если указаны необязательные паспортные данные, проверяю нет ли таких уже
            if (!string.IsNullOrEmpty(Model.PassportSerial + Model.PassportNumber))
            {
                if (_dictDb.ExistsAgentPersons(_context, new FilterDictionaryAgentPerson
                {
                    PassportSerialExact = Model.PassportSerial,
                    PassportNumberExact = Model.PassportNumber,
                    NotContainsIDs = new List<int> { Model.Id }
                }))
                { throw new DictionaryAgentPersonPassportNotUnique(Model.PassportSerial, Model.PassportNumber); }
            }

            // Если указан необязательный ИНН, проверяю нет ли такого уже
            if (!string.IsNullOrEmpty(Model.TaxCode))
            {
                if (_dictDb.ExistsAgentPersons(_context, new FilterDictionaryAgentPerson
                {
                    TaxCodeExact = Model.TaxCode,
                    NotContainsIDs = new List<int> { Model.Id }
                }))
                { throw new DictionaryAgentPersonTaxCodeNotUnique(Model.TaxCode); }
            }
            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }


    }
}
