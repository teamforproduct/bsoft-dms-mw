using System;
using System.Collections.Generic;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryContactCommand : BaseDictionaryContactCommand
    {
        private ModifyAgentContact Model { get { return GetModel<ModifyAgentContact>(); } }

        public override object Execute()
        {
            try
            {
                var newContact = new InternalDictionaryContact(Model);
                CommonDocumentUtilities.SetLastChange(_context, newContact);
                _dictDb.UpdateContact(_context, newContact);
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
