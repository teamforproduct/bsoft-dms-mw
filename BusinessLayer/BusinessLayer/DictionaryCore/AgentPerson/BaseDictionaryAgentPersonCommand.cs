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
        private AddAgentPerson Model { get { return GetModel<AddAgentPerson>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false, true);

            Model.Name?.Trim();
            Model.TaxCode?.Trim();

            // Обрезаю время для даты рождения и даты получения паспорта
            if (Model.BirthDate.HasValue) Model.BirthDate = Model.BirthDate?.Date;

            //if (_dictDb.ExistsAgents(_context, new FilterDictionaryAgent
            //{
            //    NameExact = Model.Name,
            //    NotContainsIDs = new List<int> { Model.Id }
            //}))
            //{
            //    throw new DictionaryAgentNameNotUnique(Model.Name);
            //}

            // Если указан необязательный ИНН, проверяю нет ли такого уже
            if (!string.IsNullOrEmpty(Model.TaxCode))
            {
                var filter = new FilterDictionaryAgentPerson
                {
                    TaxCodeExact = Model.TaxCode,
                };

                if (TypeModelIs<ModifyAgentPerson>())
                { filter.NotContainsIDs = new List<int> { GetModel<ModifyAgentPerson>().Id }; }

                if (_dictDb.ExistsAgentPersons(_context, filter))
                { throw new DictionaryAgentPersonTaxCodeNotUnique(Model.TaxCode); }
            }
            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }


    }
}
