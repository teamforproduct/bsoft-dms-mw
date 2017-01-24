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
    public class ModifyCustomDictionaryCommand : BaseCustomDictionaryCommand
    {
        private ModifyCustomDictionary Model { get { return GetModel<ModifyCustomDictionary>(); } }

        public override object Execute()
        {
            try
            {
                var newItem = new InternalCustomDictionary(Model);
                CommonDocumentUtilities.SetLastChange(_context, newItem);
                _dictDb.UpdateCustomDictionary(_context, newItem);
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