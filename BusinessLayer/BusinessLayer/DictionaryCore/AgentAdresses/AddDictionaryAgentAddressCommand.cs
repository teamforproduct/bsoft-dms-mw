﻿using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    class AddDictionaryAgentAddressCommand : BaseDictionaryCommand
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
            _admin.VerifyAccess(_context, CommandType, false);
            var spr = _dictDb.GetAgentAddresses(_context, Model.AgentId,new FilterDictionaryAgentAddress
            {
                PostCodeExact = Model.PostCode,
                AddressExact = Model.Address,
                AddressTypeId = new List<int> { Model.AddressTypeId },
                AgentId = Model.AgentId
            });
            if (spr.Count() != 0)
            {
                throw new DictionaryRecordNotUnique();
            }

            _admin.VerifyAccess(_context, CommandType,false,true);
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newAddr = new InternalDictionaryAgentAddress(Model);
                CommonDocumentUtilities.SetLastChange(_context, newAddr);
                return _dictDb.AddAgentAddress(_context, newAddr);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
