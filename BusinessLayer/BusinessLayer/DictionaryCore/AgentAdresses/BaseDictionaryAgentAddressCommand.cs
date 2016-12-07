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
    public class BaseDictionaryAgentAddressCommand : BaseDictionaryCommand
    {
        private AddDictionaryAgentAddress Model { get { return GetModel<AddDictionaryAgentAddress>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false, true);

            Model.PostCode?.Trim();

            //1

            var filter = new FilterDictionaryAgentAddress
            {
                AgentIDs = new List<int> { Model.AgentId },
                AddressTypeIDs = new List<int> { Model.AddressTypeId },
            };

            if (TypeModelIs<ModifyDictionaryAgentAddress>())
            { filter.NotContainsIDs = new List<int> { GetModel<ModifyDictionaryAgentAddress>().Id }; }

            var spr = _dictDb.GetAgentAddresses(_context, filter);

            if (spr.Count() != 0) throw new DictionaryAddressTypeNotUnique();

            //2

            filter = new FilterDictionaryAgentAddress
            {
                AgentIDs = new List<int> { Model.AgentId },
                PostCodeExact = Model.PostCode,
                AddressExact = Model.Address,
            };

            if (TypeModelIs<ModifyDictionaryAgentAddress>())
            { filter.NotContainsIDs = new List<int> { GetModel<ModifyDictionaryAgentAddress>().Id }; }

            spr = _dictDb.GetAgentAddresses(_context, filter);

            if (spr.Count() != 0) throw new DictionaryAddressNameNotUnique(Model.PostCode, Model.Address);


            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}
