using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class BaseDictionaryAgentEmployeeCommand : BaseDictionaryCommand
    {
        private AddAgentEmployee Model { get { return GetModel<AddAgentEmployee>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            if (string.IsNullOrEmpty(Model.Name)) throw new NameRequired();

            Model.Name?.Trim();
            Model.TaxCode?.Trim();

            // вычисляю табельный номер. если не передан
            if (Model.PersonnelNumber < 1)
            {
                Model.PersonnelNumber = _dictDb.GetAgentEmployeePersonnelNumber(_context);
            }

            // Обрезаю время для даты рождения и даты получения паспорта
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
