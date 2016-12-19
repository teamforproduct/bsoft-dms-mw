using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryAgentPersonCommand : BaseDictionaryAgentPersonCommand
    {
        private ModifyAgentPerson Model { get { return GetModel<ModifyAgentPerson>(); } }

        public override object Execute()
        {
            try
            {
                var newPerson = new InternalDictionaryAgentPerson(Model);
                CommonDocumentUtilities.SetLastChange(_context, newPerson);
                _dictDb.UpdateAgentPerson(_context, newPerson);


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
