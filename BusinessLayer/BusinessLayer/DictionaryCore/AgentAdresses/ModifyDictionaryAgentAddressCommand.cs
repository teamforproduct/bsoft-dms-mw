using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;


namespace BL.Logic.DictionaryCore.AgentAdresses 
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
            var spr = _dictDb.GetDictionaryAgentAddresses(_context,
                new FilterDictionaryAgentAddress {
                    PostCode = Model.PostCode,
                    Address = Model.Address,
                    AddressTypeId = new List<int> { Model.AddressTypeId } ,
                    AgentId = new List<int> { Model.AgentId }
                    });
            if (spr != null)
            {
                throw new DictionaryRecordNotUnique();
            }

            _admin.VerifyAccess(_context, CommandType);

            return true;
        }

        public override object Execute()
        {
            try
            {
                var newAddr = new InternalDictionaryAgentAddress
                {
                    Id = Model.Id,
                    AgentId=Model.AgentId,
                    AddressTypeID=Model.AddressTypeId,
                    PostCode=Model.PostCode,
                    Address = Model.Address,
                    IsActive = Model.IsActive
                };
                CommonDocumentUtilities.SetLastChange(_context, newAddr);
                _dictDb.UpdateDictionaryAgentAddress(_context, newAddr);
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
