using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class BaseDictionaryAgentEmployeeCommand : BaseDictionaryCommand
    {
        private AddAgentEmployee Model { get { return GetModel<AddAgentEmployee>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            Model.Name?.Trim();
            Model.PassportSerial?.Trim();
            Model.TaxCode?.Trim();

            // вычисляю табельный номер. если не передан
            if (Model.PersonnelNumber < 1)
            {
                Model.PersonnelNumber = _dictDb.GetAgentEmployeePersonnelNumber(_context);
            }

            // Обрезаю время для даты рождения и даты получения паспорта
            if (Model.PassportDate.HasValue) Model.PassportDate = Model.PassportDate?.Date;

            if (Model.BirthDate.HasValue) Model.BirthDate = Model.BirthDate?.Date;


            //if (_dictDb.ExistsAgents(_context, new FilterDictionaryAgent()
            //{
            //    NameExact = Model.Name,
            //    NotContainsIDs = new List<int> { Model.Id }
            //}))
            //{
            //    throw new DictionaryAgentNameNotUnique(Model.Name);
            //}

            var filterEmployee = new FilterDictionaryAgentEmployee()
            {
                PersonnelNumber = Model.PersonnelNumber,
            };

            if (TypeModelIs<ModifyAgentEmployee>())
            { filterEmployee.NotContainsIDs = new List<int> { GetModel<ModifyAgentEmployee>().Id }; }

            if (_dictDb.ExistsAgentEmployees(_context, filterEmployee))
            {
                throw new DictionaryAgentEmployeePersonnelNumberNotUnique(Model.PersonnelNumber);
            }

            // Если указаны необязательные паспортные данные, проверяю нет ли таких уже
            if (!string.IsNullOrEmpty(Model.PassportSerial + Model.PassportNumber))
            {
                var filterPerson = new FilterDictionaryAgentPerson
                {
                    PassportSerialExact = Model.PassportSerial,
                    PassportNumberExact = Model.PassportNumber,
                };

                if (TypeModelIs<ModifyAgentEmployee>())
                { filterPerson.NotContainsIDs = new List<int> { GetModel<ModifyAgentEmployee>().Id }; }

                if (_dictDb.ExistsAgentPersons(_context, filterPerson))
                {
                    throw new DictionaryAgentEmployeePassportNotUnique(Model.PassportSerial, Model.PassportNumber);
                }
            }

            // Если указан необязательный ИНН, проверяю нет ли такого уже
            if (!string.IsNullOrEmpty(Model.TaxCode))
            {
                var filterPerson = new FilterDictionaryAgentPerson
                {
                    TaxCodeExact = Model.TaxCode,
                };

                if (TypeModelIs<ModifyAgentEmployee>())
                { filterPerson.NotContainsIDs = new List<int> { GetModel<ModifyAgentEmployee>().Id }; }

                if (_dictDb.ExistsAgentPersons(_context, filterPerson))
                {
                    throw new DictionaryAgentEmployeeTaxCodeNotUnique(Model.TaxCode);
                }
            }

            //добавить проверку на FullName

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}
