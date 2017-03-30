using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using System;


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