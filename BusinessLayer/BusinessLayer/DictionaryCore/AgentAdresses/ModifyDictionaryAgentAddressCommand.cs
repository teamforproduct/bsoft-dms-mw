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
    public class ModifyDictionaryAgentAddressCommand : BaseDictionaryCommand
    {
        private ModifyDictionaryAgentAddress Model
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

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
           

            _admin.VerifyAccess(_context, CommandType,false,true);
            var spr = _dictDb.GetAgentAddresses(_context, Model.AgentId, new FilterDictionaryAgentAddress
            {
                PostCodeExact = Model.PostCode,
                AddressExact = Model.Address,
                AddressTypeId = new List<int> { Model.AddressTypeId },
                AgentId = Model.AgentId,
                IsActive=Model.IsActive,
                NotContainsIDs = new List<int> { Model.Id }
            });
            if (spr.Count() != 0)
            {
                throw new DictionaryRecordNotUnique();
            }
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newAddr = new InternalDictionaryAgentAddress(Model);
                CommonDocumentUtilities.SetLastChange(_context, newAddr);
                _dictDb.UpdateAgentAddress(_context, newAddr);
            }
            catch (DictionaryRecordWasNotFound)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseError(ex);
            }
            return null;
        }
    }
}
