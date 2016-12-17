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
    // TODO Delete v3
    public class ModifyDictionaryAgentUserLanguageCommand : BaseDictionaryAgentUserCommand
    {
        public override object Execute()
        {
            try
            {
                var item = new InternalDictionaryAgentUser(Model);

                CommonDocumentUtilities.SetLastChange(_context, item);

                _dictDb.SetAgentUserLanguage(_context, item);
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
