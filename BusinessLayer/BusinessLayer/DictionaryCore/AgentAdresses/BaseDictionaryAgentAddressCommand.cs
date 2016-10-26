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
        protected ModifyDictionaryAgentAddress Model
        {
            get
            {
                if (!(_param is ModifyDictionaryAgentAddress))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryAgentAddress)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType,false,true);

            Model.PostCode?.Trim();

            var spr = _dictDb.GetAgentAddresses(_context, Model.AgentId, new FilterDictionaryAgentAddress
            {
                AgentId = Model.AgentId,
                NotContainsIDs = new List<int> { Model.Id },
                AddressTypeId = new List<int> { Model.AddressTypeId },
            });
            if (spr.Count() != 0)
            {
                throw new DictionaryAgentAddressTypeNotUnique();
            }

            spr = _dictDb.GetAgentAddresses(_context, Model.AgentId, new FilterDictionaryAgentAddress
            {
                AgentId = Model.AgentId,
                NotContainsIDs = new List<int> { Model.Id },
                PostCodeExact = Model.PostCode,
                AddressExact = Model.Address,
            });
            if (spr.Count() != 0)
            {
                throw new DictionaryAgentAddressNameNotUnique(Model.PostCode, Model.Address);
            }
            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}
