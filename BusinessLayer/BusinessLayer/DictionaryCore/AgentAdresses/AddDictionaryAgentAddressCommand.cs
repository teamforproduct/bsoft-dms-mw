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
    class AddDictionaryAgentAddressCommand : BaseDictionaryAgentAddressCommand
    {
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
