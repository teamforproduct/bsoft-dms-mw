using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;


namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryAgentCommand : BaseDictionaryAgentCommand
    {

        public override object Execute()
        {
            try
            {
                var newAgent = new InternalDictionaryAgent(Model);
                CommonDocumentUtilities.SetLastChange(_context, newAgent);
                _dictDb.UpdateAgent(_context, newAgent);
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
