using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using System.Collections.Generic;


namespace BL.Logic.DictionaryCore
{
    public class BaseDictionaryAgentClientCompanyCommand : BaseDictionaryCommand
    {
        private AddAgentClientCompany Model { get { return GetModel<AddAgentClientCompany>(); } }

        public override bool CanBeDisplayed(int CompanyId) => true;

        public override bool CanExecute()
        {

            _adminService.VerifyAccess(_context, CommandType, false);

            Model.Name?.Trim();

            //if (_dictDb.ExistsAgents(_context, new FilterDictionaryAgent()
            //{
            //    NameExact = Model.Name,
            //    NotContainsIDs = new List<int> { Model.Id }
            //}))
            //{
            //    throw new DictionaryAgentNameNotUnique(Model.Name);
            //}
            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}