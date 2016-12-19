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
    public class ModifyDictionaryAgentAddressCommand : BaseDictionaryAgentAddressCommand
    {
        private ModifyAgentAddress Model { get { return GetModel<ModifyAgentAddress>(); } }

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
