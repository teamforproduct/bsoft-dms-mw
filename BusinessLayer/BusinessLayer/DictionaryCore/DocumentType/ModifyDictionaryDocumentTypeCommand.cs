using System;
using System.Collections.Generic;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryDocumentTypeCommand : BaseDictionaryDocumentTypeCommand
    {
        private ModifyDocumentType Model { get { return GetModel<ModifyDocumentType>(); } }

        public override object Execute()
        {
            try
            {
                var newDocType = new InternalDictionaryDocumentType(Model);
                CommonDocumentUtilities.SetLastChange(_context, newDocType);
                _dictDb.UpdateDocumentType(_context, newDocType);
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